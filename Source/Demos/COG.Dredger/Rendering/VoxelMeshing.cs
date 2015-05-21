using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Framework;
using COG.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace COG.Dredger.Rendering
{
    public class Dimensions
    {
        public int[] size;

        public Dimensions(int[] size)
        {
            this.size = size;
        }

        public int this[int index] { get { return size[index]; } }
    }

    public class Volume : DisposableObject
    {
        public DynamicMesh opaqueMesh;
        public DynamicMesh waterMesh;
        public Dimensions dims;
        public int X, Y, Z;

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            if (opaqueMesh)
                opaqueMesh.Dispose();

            if (waterMesh)
                waterMesh.Dispose();

        }

        private uint[] data;
        public Volume(int x, int y, int z, uint[] data, Dimensions dims)
        {
            X = x;
            Y = y;
            Z = z;
            this.data = data;
            this.dims = dims;
        }

        public uint this[int index]
        {
            get
            {
                if (index < 0 || index > data.Length - 1)
                    return 0;
                return data[index];
            }
        }

        public uint this[int i, int j, int k]
        {
            get
            {
                var index = i + dims[0] * (j + dims[1] * k);
                if (index < 0 || index > data.Length - 1)
                    return 0;
                return data[index];
            }
            set
            {
                data[i + dims[0] * (j + dims[1] * k)] = value;
            }
        }

        public int Width { get { return dims[0]; } }
        public int Height { get { return dims[1]; } }
        public int Depth { get { return dims[2]; } }

        public void PrepareMesh()
        {
            if (opaqueMesh == null)
            {
                opaqueMesh = new DynamicMesh(VertexPositionTextureColor.VertexDeclaration);
                //opaqueMesh = new DynamicMesh(VertexPositionColor.VertexDeclaration);
            }
            //else
            //{
            //    opaqueMesh.Clear();
            //}

            if (waterMesh == null)
            {
                waterMesh = new DynamicMesh(VertexPositionTextureColor.VertexDeclaration);
                //waterMesh = new DynamicMesh(VertexPositionColor.VertexDeclaration);
            }
            //else
            //{
            //    waterMesh.Clear();
            //}
        }

        public void UpdateMesh()
        {
            this.PrepareMesh();
            SurfaceExtractor.GenerateMesh(this);
            SurfaceExtractor.GenerateWaterMesh(this);

            //opaqueMesh.RecalculateNormals();
            //waterMesh.RecalculateNormals();
            //mesh.UploadMeshData(true);


            //mesh.SetIndices()
        }

        public void RenderOpaque(Program program)
        {
            var p = new Vector3(X, 0, Z);
            var m = Matrix4.CreateTranslation(p / 8f);

            program.SetUniformMatrix4("model", m);
            opaqueMesh.Render(program);
        }

        public void RenderAlpha(Program program)
        {
            var p = new Vector3(X, 0, Z);
            var m = Matrix4.CreateTranslation(p / 8f);
            
            program.SetUniformMatrix4("model", m);
            waterMesh.Render(program);
        }
    }
    public class SurfaceExtractor
    {
        static uint[] mask = new uint[4096];
        static MaskLayout[] maskLayout = new MaskLayout[4096];

        public static List<Vector3> vertices = new List<Vector3>(65536);
        public static List<int> faces = new List<int>(65536);
        public static List<Vector2> uvs = new List<Vector2>(65536);
        public static List<Vector3> normals = new List<Vector3>(65536);
        public static List<Color> colors = new List<Color>(65536);


        public static Volume makeVoxels(int x, int y, int z, int[] l, int[] h, Func<int, int, int, uint> f)
        {
            int[] d = { h[0] - l[0], h[1] - l[1], h[2] - l[2] };
            uint[] v = new uint[d[0] * d[1] * d[2]];
            int n = 0;
            for (var k = l[2]; k < h[2]; ++k)
                for (var j = l[1]; j < h[1]; ++j)
                    for (var i = l[0]; i < h[0]; ++i, ++n)
                    {
                        v[n] = f(i, j, k);
                    }

            return new Volume(x, y, z, v, new Dimensions(d));
        }

        public struct MaskLayout
        {
            private const int BACKFACE_BIT = 31;
            private const int FLIPFACE_BIT = 30;
            //occlusion = 0 - 11
            //flip = 12
            public uint data;

            public void Reset()
            {
                data = 0;
            }

            public bool FlipFace
            {
                get { return (data & (1u << FLIPFACE_BIT)) > 0u; }
                set
                {
                    if (value)
                        data |= (1u << FLIPFACE_BIT);
                    else
                        data &= ((1u << FLIPFACE_BIT) ^ 0xffffffffu);
                }
            }

            public bool BackFace
            {
                get { return (data & (1u << BACKFACE_BIT)) > 0u; }
                set
                {
                    if (value)
                        data |= (1u << BACKFACE_BIT);
                    else
                        data &= ((1u << BACKFACE_BIT) ^ 0xffffffffu);
                }
            }

            public void SetOcclusion(int vert, uint count)
            {
                data |= ((count & 3u) << (vert * 3));
            }

            public uint GetOcclusion(int vert)
            {
                return (data >> (vert * 3)) & 3u;
            }
        }

        public static float Pack(Vector2 input, int precision = 4096)
        {
            Vector2 output = input;
            output.X = Utility.Floor(output.X * (precision - 1));
            output.Y = Utility.Floor(output.Y * (precision - 1));

            return (output.X * precision) + output.Y;
        }

        public static float Pack(float x, float y, int precision = 4096)
        {
            return Pack(new Vector2(x, y), precision);
        }

        public static Vector2 Unpack(float input, int precision = 4096)
        {
            Vector2 output = Vector2.Zero;

            output.Y = input % precision;
            output.X = Utility.Floor(input / precision);

            return output / (precision - 1);
        }

        public static int GenerateMesh(Volume volume, bool centered = false, bool disableGreedyMeshing = false, bool disableAO = false)
        {
            vertices.Clear();
            faces.Clear();
            uvs.Clear();
            normals.Clear();
            colors.Clear();
            var dims = volume.dims;

            var f = new Func<int, int, int, uint>((i, j, k) =>
            {
                if (i < 0 || j < 0 || k < 0 || i >= dims[0] || j >= dims[1] || k >= dims[2])
                    return 0;

                var r = volume[i + dims[0] * (j + dims[1] * k)];
                if (r > 0 && (r & 0x1000000u) > 0u)
                {
                    r = 0;
                }
                //1 + dims[0] * (1 + dims[1] * 1)
                //i    w    j   h    k
                //1 + 16 * (1 + 16 * 1)
                return r;
                //return 0;
            });

            //Sweep over 3-axes

            for (var d = 0; d < 3; ++d)
            {
                int i, j, k, l, w, h
                  , u = (d + 1) % 3
                  , v = (d + 2) % 3;
                int[] x = { 0, 0, 0 };
                int[] q = { 0, 0, 0 };
                int[,] posArea = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
                int[,] negArea = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

                if (mask.Length < dims[u] * dims[v])
                {
                    mask = new uint[dims[u] * dims[v]];
                    maskLayout = new MaskLayout[dims[u] * dims[v]];
                }
                q[d] = 1;

                posArea[0, u] = -1;
                posArea[1, v] = -1;
                posArea[2, u] = 1;
                posArea[3, v] = 1;

                negArea[0, v] = -1;
                negArea[1, u] = -1;
                negArea[2, v] = 1;
                negArea[3, u] = 1;

                for (x[d] = -1; x[d] < dims[d]; )
                {
                    //Compute mask
                    //TODO: Test if the AOMASK should be created outside of the block mask
                    //the aomask generation might be causing a cache miss per loop
                    var n = 0;
                    for (x[v] = 0; x[v] < dims[v]; ++x[v])
                    {
                        for (x[u] = 0; x[u] < dims[u]; ++x[u], ++n)
                        {
                            //int a = 0;
                            //if (x[d] < 0 && cm != null)
                            //    a = cm.GetBlock(volume.X + x[0], volume.Y + x[1], volume.Z + x[2]);
                            //else
                            var a = (0u <= x[d] ? f(x[0], x[1], x[2]) : 0u);
                            var b = (x[d] < dims[d] - 1 ? f(x[0] + q[0], x[1] + q[1], x[2] + q[2]) : 0u);

                            maskLayout[n].data = 0u;
                            if ((a != 0) == (b != 0))
                            {
                                mask[n] = 0;
                            }
                            else if (a != 0)
                            {
                                mask[n] = a;
                            }
                            else
                            {
                                maskLayout[n].BackFace = true;
                                mask[n] = b;
                            }

                            if (disableAO || mask[n] == 0)
                            {
                                //maskLayout[n].data = 4095u;
                            }
                            else
                            {
                                uint side1 = 0, side2 = 0, corner = 0;
                                var neighbors = new uint[4];

                                for (var t = 0; t < 4; ++t)
                                {
                                    var tt = (t + 1) % 4;

                                    if (a != 0)
                                    {
                                        side1 = ((x[d] < dims[d] - 1 ? f(x[0] + q[0] + posArea[t, 0], x[1] + q[1] + posArea[t, 1], x[2] + q[2] + posArea[t, 2]) : 0u) > 0u ? 1u : 0u);
                                        side2 = ((x[d] < dims[d] - 1 ? f(x[0] + q[0] + posArea[tt, 0], x[1] + q[1] + posArea[tt, 1], x[2] + q[2] + posArea[tt, 2]) : 0u) > 0u ? 1u : 0u);
                                    }
                                    else
                                    {
                                        side1 = ((x[d] < dims[d] - 1 ? f(x[0] + negArea[t, 0], x[1] + negArea[t, 1], x[2] + negArea[t, 2]) : 0u) > 0u ? 1u : 0u);
                                        side2 = ((x[d] < dims[d] - 1 ? f(x[0] + negArea[tt, 0], x[1] + negArea[tt, 1], x[2] + negArea[tt, 2]) : 0u) > 0u ? 1u : 0u);
                                    }

                                    if (side1 > 0 && side2 > 0)
                                    {
                                        neighbors[t] = 0;
                                    }
                                    else
                                    {
                                        if (a != 0)
                                        {
                                            corner = ((x[d] < dims[d] - 1u ? f(x[0] + q[0] + posArea[t, 0] + posArea[tt, 0], x[1] + q[1] + posArea[t, 1] + posArea[tt, 1], x[2] + q[2] + posArea[t, 2] + posArea[tt, 2]) : 0u) > 0u ? 1u : 0u);
                                        }
                                        else
                                        {
                                            corner = ((x[d] < dims[d] - 1u ? f(x[0] + negArea[t, 0] + negArea[tt, 0], x[1] + negArea[t, 1] + negArea[tt, 1], x[2] + negArea[t, 2] + negArea[tt, 2]) : 0u) > 0u ? 1u : 0u);
                                        }
                                        neighbors[t] = 3u - (side1 + side2 + corner);
                                    }

                                    maskLayout[n].SetOcclusion(t, neighbors[t]);
                                }

                                uint a00 = neighbors[0], a01 = neighbors[1], a11 = neighbors[2], a10 = neighbors[3];
                                if (a00 + a11 == a10 + a01)
                                    maskLayout[n].FlipFace = Math.Max(a00, a11) < Math.Max(a10, a01);
                                else if (a00 + a11 < a10 + a01)
                                    maskLayout[n].FlipFace = true;


                            }
                        }
                    }
                    //Increment x[d]
                    ++x[d];
                    //Generate mesh for mask using lexicographic ordering
                    n = 0;
                    for (j = 0; j < dims[v]; ++j)
                    {
                        for (i = 0; i < dims[u]; )
                        {
                            var c = mask[n];
                            if (c != 0)
                            {
                                var a = maskLayout[n];
                                if(j ==0 || i == 0 || j == dims[v] - 1 || i == dims[u] - 1)
                                {
                                    mask[n] = 0;
                                    maskLayout[n].data = 4095u;
                                    ++i; ++n;

                                    continue;
                                }else
                                if (disableGreedyMeshing)
                                {
                                    w = 1;
                                    h = 1;
                                }
                                else
                                {
                                    //Compute width
                                    for (w = 1; c == mask[n + w] && (a.data == (maskLayout[n + w].data)) && i + w < dims[u]; ++w)
                                    {
                                    }
                                    //Compute height (this is slightly awkward
                                    var done = false;
                                    for (h = 1; j + h < dims[v]; ++h)
                                    {
                                        for (k = 0; k < w; ++k)
                                        {
                                            if (c != mask[n + k + h * dims[u]] || a.data != maskLayout[n + k + h * dims[u]].data)
                                            {
                                                done = true;
                                                break;
                                            }
                                        }
                                        if (done)
                                        {
                                            break;
                                        }
                                    }
                                }

                                //Add quad
                                x[u] = i; x[v] = j;
                                int[] du = { 0, 0, 0 };
                                int[] dv = { 0, 0, 0 };

                                if (!maskLayout[n].BackFace)
                                {
                                    dv[v] = h;
                                    du[u] = w;
                                }
                                else
                                {
                                    du[v] = h;
                                    dv[u] = w;
                                }

                                var flip = maskLayout[n].FlipFace;

                                var cr = ((c >> 16) & 0xff) / 255f;
                                var cg = ((c >> 8) & 0xff) / 255f;
                                var cb = (c & 0xff) / 255f;

                                var aouvs = new float[4];
                                float[] AOcurve = new float[] { 0.0f, 0.6f, 0.8f, 1.0f };
                                Color[] ugh = new Color[] { 
                                new Color(1,0,0,1),
                                new Color(0,1,0,1),
                                new Color(0,0,1,1),
                                new Color(1,1,1,1)
                            };
                                for (var o = 0; o < 4; ++o)
                                {
                                    //var ao = AOcurve[maskLayout[n].GetOcclusion(o)];
                                    var ao = disableAO ? 1f : (maskLayout[n].GetOcclusion(o) / 3f);
                                    ////var ao = disableAO ? 1f : (maskLayout[n].GetOcclusion(o) / 4f + 0.25f);
                                    ////if (maskLayout[n].GetOcclusion(o) != 3u)
                                    ////    ao = 0.5f;
                                    ////else
                                    ////    ao = 1f;
                                    //var color = new Color(cr * ao, cg * ao, cb * ao, 1);
                                    //colors.Add(color);
                                    ////colors.Add(new Color(1,0,1,1));

                                    colors.Add(new Color(cr, cg, cb));
                                    uvs.Add(new Vector2(ao, 0));
                                    //colors.Add(ugh[maskLayout[n].GetOcclusion(o)]);

                                    //aouvs[o] = ao;
                                }


                                //for (var o = 0; o < 4; ++o)
                                //{
                                //    uvs.Add(new Vector2(Pack(aouvs[0], aouvs[1]), Pack(aouvs[2], aouvs[3])));
                                //}

                                var vertex_count = vertices.Count;
                                if (centered)
                                {
                                    //This vert generation code will make the 0,0,0 be the center of the mesh in worldspace
                                    vertices.Add(new Vector3(x[0], x[1], x[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                    vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                    vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                    vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                }
                                else
                                {
                                    ////This vert generation code will make the edge of the mesh at 0,0,0
                                    vertices.Add(new Vector3(x[0], x[1], x[2]));
                                    vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]));
                                    vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]));
                                    vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]));
                                }

                                //uvs.Add(new Vector2(0, 1));
                                //uvs.Add(new Vector2(1, 1));
                                //uvs.Add(new Vector2(1, 0));
                                //uvs.Add(new Vector2(0, 0));

                                if (flip)
                                    faces.AddRange(new int[] { vertex_count + 1, vertex_count + 2, vertex_count + 3, vertex_count + 1, vertex_count + 3, vertex_count });
                                else
                                    faces.AddRange(new int[] { vertex_count, vertex_count + 1, vertex_count + 2, vertex_count, vertex_count + 2, vertex_count + 3 });

                                //Zero-out mask
                                for (l = 0; l < h; ++l)
                                {
                                    for (k = 0; k < w; ++k)
                                    {
                                        mask[n + k + l * dims[u]] = 0;
                                        maskLayout[n + k + l * dims[u]].data = 4095u;
                                    }
                                }
                                //Increment counters and continue
                                i += w; n += w;
                            }
                            else
                            {
                                ++i; ++n;
                            }
                        }
                    }
                }
            }

            //volume.opaqueMesh.vertices = vertices.ToArray();
            //volume.opaqueMesh.colors = colors.ToArray();
            //volume.opaqueMesh.triangles = faces.ToArray();
            //volume.opaqueMesh.uv = uvs.ToArray();
            //volume.opaqueMesh.RecalculateNormals();

            volume.PrepareMesh();
            var mesh = volume.opaqueMesh;
            mesh.Begin();
            for (var i = 0; i < vertices.Count; i++)
            {
                mesh.Position(vertices[i] / 8f);
                mesh.TextureCoord(uvs[i]);
                mesh.Color(colors[i]);
            }

            for (var i = 0; i < faces.Count; i++)
            {
                mesh.Index((ushort)faces[i]);
            }
            mesh.End(BufferUsageHint.DynamicDraw);

            return vertices.Count;
        }

        public static int GenerateWaterMesh(Volume volume, bool centered = false, bool disableGreedyMeshing = false)
        {
            vertices.Clear();
            faces.Clear();
            uvs.Clear();
            normals.Clear();
            colors.Clear();

            var dims = volume.dims;

            var f = new Func<int, int, int, uint>((i, j, k) =>
            {
                if (i < 0 || j < 0 || k < 0 || i >= dims[0] || j >= dims[1] || k >= dims[2])
                    return 0;

                var r = volume[i + dims[0] * (j + dims[1] * k)];
                if ((r & 0x1000000u) > 0u)
                {
                    return r;
                }
                return 0;
                //1 + dims[0] * (1 + dims[1] * 1)
                //i    w    j   h    k
                //1 + 16 * (1 + 16 * 1)
                //return 0;
            });

            //Sweep over 3-axes

            for (var d = 1; d < 2; ++d)
            {
                int i, j, k, l, w, h
                  , u = (d + 1) % 3
                  , v = (d + 2) % 3;
                int[] x = { 0, 0, 0 };
                int[] q = { 0, 0, 0 };
                int[,] posArea = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
                int[,] negArea = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

                if (mask.Length < dims[u] * dims[v])
                {
                    mask = new uint[dims[u] * dims[v]];
                    maskLayout = new MaskLayout[dims[u] * dims[v]];
                }
                q[d] = 1;

                posArea[0, u] = -1;
                posArea[1, v] = -1;
                posArea[2, u] = 1;
                posArea[3, v] = 1;

                negArea[0, v] = -1;
                negArea[1, u] = -1;
                negArea[2, v] = 1;
                negArea[3, u] = 1;

                for (x[d] = -1; x[d] < dims[d]; )
                {
                    //Compute mask
                    //TODO: Test if the AOMASK should be created outside of the block mask
                    //the aomask generation might be causing a cache miss per loop
                    var n = 0;
                    for (x[v] = 0; x[v] < dims[v]; ++x[v])
                    {
                        for (x[u] = 0; x[u] < dims[u]; ++x[u], ++n)
                        {
                            var a = (0u <= x[d] ? f(x[0], x[1], x[2]) : 0u);

                            if ((a & 0x1000000u) > 0u)
                                mask[n] = a;

                        }
                    }
                    //Increment x[d]
                    ++x[d];
                    //Generate mesh for mask using lexicographic ordering
                    n = 0;
                    for (j = 0; j < dims[v]; ++j)
                    {
                        for (i = 0; i < dims[u]; )
                        {
                            var c = mask[n];
                            if (c != 0)
                            {
                                var a = maskLayout[n];
                                if (disableGreedyMeshing)
                                {
                                    w = 1;
                                    h = 1;
                                }
                                else
                                {
                                    //Compute width
                                    for (w = 1; c == mask[n + w] && (a.data == (maskLayout[n + w].data)) && i + w < dims[u]; ++w)
                                    {
                                    }
                                    //Compute height (this is slightly awkward
                                    var done = false;
                                    for (h = 1; j + h < dims[v]; ++h)
                                    {
                                        for (k = 0; k < w; ++k)
                                        {
                                            if (c != mask[n + k + h * dims[u]] || a.data != maskLayout[n + k + h * dims[u]].data)
                                            {
                                                done = true;
                                                break;
                                            }
                                        }
                                        if (done)
                                        {
                                            break;
                                        }
                                    }
                                }

                                //Add quad
                                x[u] = i; x[v] = j;
                                for (var backface = 0; backface < 2; ++backface)
                                {
                                    int[] du = { 0, 0, 0 };
                                    int[] dv = { 0, 0, 0 };

                                    if (backface == 0)
                                    {
                                        dv[v] = h;
                                        du[u] = w;
                                    }
                                    else
                                    {
                                        du[v] = h;
                                        dv[u] = w;
                                    }

                                    var flip = maskLayout[n].FlipFace;

                                    var cr = ((c >> 16) & 0xff) / 255f;
                                    var cg = ((c >> 8) & 0xff) / 255f;
                                    var cb = (c & 0xff) / 255f;

                                    for (var o = 0; o < 4; ++o)
                                    {
                                        colors.Add(new Color(cr, cg, cb));
                                        uvs.Add(new Vector2(0, 0));
                                    }

                                    var vertex_count = vertices.Count;
                                    var offset = Vector3.Zero;
                                    if (centered)
                                        offset = new Vector3(dims[0], dims[1], dims[2]) / 2f;

                                    //offset += new Vector3(0, 0.5f, 0);
                                    vertices.Add(new Vector3(x[0], x[1], x[2]) - offset);
                                    vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]) - offset);
                                    vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]) - offset);
                                    vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]) - offset);


                                    faces.AddRange(new int[] { vertex_count, vertex_count + 1, vertex_count + 2, vertex_count, vertex_count + 2, vertex_count + 3 });
                                }

                                //Zero-out mask
                                for (l = 0; l < h; ++l)
                                {
                                    for (k = 0; k < w; ++k)
                                    {
                                        mask[n + k + l * dims[u]] = 0;
                                        maskLayout[n + k + l * dims[u]].data = 4095u;
                                    }
                                }
                                //Increment counters and continue
                                i += w; n += w;
                            }
                            else
                            {
                                ++i; ++n;
                            }
                        }
                    }
                }
            }

            //volume.waterMesh.vertices = vertices.ToArray();
            //volume.waterMesh.colors = colors.ToArray();
            //volume.waterMesh.triangles = faces.ToArray();
            //volume.waterMesh.uv = uvs.ToArray();
            //volume.waterMesh.RecalculateNormals();

            volume.PrepareMesh();
            var mesh = volume.waterMesh;
            mesh.Begin();
            for (var i = 0; i < vertices.Count; i++)
            {
                mesh.Position(vertices[i] / 8f);
                mesh.TextureCoord(uvs[i]);
                mesh.Color(colors[i]);
            }

            for (var i = 0; i < faces.Count; i++)
            {
                mesh.Index((ushort)faces[i]);
            }
            mesh.End(BufferUsageHint.DynamicDraw);
            return vertices.Count;
        }

        public static int GenerateMesh2(DynamicMesh mesh, Volume volume, bool centered = false, bool disableGreedyMeshing = false, bool disableAO = false)
        {
            vertices.Clear();
            faces.Clear();
            uvs.Clear();
            normals.Clear();
            colors.Clear();

            var dims = volume.dims;

            var f = new Func<int, int, int, uint>((i, j, k) =>
            {
                if (i < 0 || j < 0 || k < 0 || i >= dims[0] || j >= dims[1] || k >= dims[2])
                    return 0;

                var r = volume[i + dims[0] * (j + dims[1] * k)];
                if (r > 0 && (r & 0x1000000u) > 0u)
                {
                    r = 0;
                }
                return r;
            });

            //Sweep over 3-axes
            for (var d = 0; d < 3; ++d)
            {
                int i, j, k, l, w, h
                  , u = (d + 1) % 3
                  , v = (d + 2) % 3;
                int[] x = { 0, 0, 0 };
                int[] q = { 0, 0, 0 };
                int[,] posArea = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
                int[,] negArea = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

                if (mask.Length < dims[u] * dims[v])
                {
                    mask = new uint[dims[u] * dims[v]];
                    maskLayout = new MaskLayout[dims[u] * dims[v]];
                }
                q[d] = 1;

                posArea[0, u] = -1;
                posArea[1, v] = -1;
                posArea[2, u] = 1;
                posArea[3, v] = 1;

                negArea[0, v] = -1;
                negArea[1, u] = -1;
                negArea[2, v] = 1;
                negArea[3, u] = 1;

                for (x[d] = -1; x[d] < dims[d]; )
                {
                    //Compute mask
                    //TODO: Test if the AOMASK should be created outside of the block mask
                    //the aomask generation might be causing a cache miss per loop
                    var n = 0;
                    for (x[v] = 0; x[v] < dims[v]; ++x[v])
                    {
                        for (x[u] = 0; x[u] < dims[u]; ++x[u], ++n)
                        {
                            //int a = 0;
                            //if (x[d] < 0 && cm != null)
                            //    a = cm.GetBlock(volume.X + x[0], volume.Y + x[1], volume.Z + x[2]);
                            //else
                            var a = (0u <= x[d] ? f(x[0], x[1], x[2]) : 0u);
                            var b = (x[d] < dims[d] - 1 ? f(x[0] + q[0], x[1] + q[1], x[2] + q[2]) : 0u);

                            maskLayout[n].data = 0u;
                            if ((a != 0) == (b != 0))
                            {
                                mask[n] = 0;
                            }
                            else if (a != 0)
                            {
                                mask[n] = a;
                            }
                            else
                            {
                                maskLayout[n].BackFace = true;
                                mask[n] = b;
                            }

                            if (disableAO || mask[n] == 0)
                            {
                                //maskLayout[n].data = 4095u;
                            }
                            else
                            {
                                uint side1 = 0, side2 = 0, corner = 0;
                                var neighbors = new uint[4];

                                for (var t = 0; t < 4; ++t)
                                {
                                    var tt = (t + 1) % 4;

                                    if (a != 0)
                                    {
                                        side1 = ((x[d] < dims[d] - 1 ? f(x[0] + q[0] + posArea[t, 0], x[1] + q[1] + posArea[t, 1], x[2] + q[2] + posArea[t, 2]) : 0u) > 0u ? 1u : 0u);
                                        side2 = ((x[d] < dims[d] - 1 ? f(x[0] + q[0] + posArea[tt, 0], x[1] + q[1] + posArea[tt, 1], x[2] + q[2] + posArea[tt, 2]) : 0u) > 0u ? 1u : 0u);
                                    }
                                    else
                                    {
                                        side1 = ((x[d] < dims[d] - 1 ? f(x[0] + negArea[t, 0], x[1] + negArea[t, 1], x[2] + negArea[t, 2]) : 0u) > 0u ? 1u : 0u);
                                        side2 = ((x[d] < dims[d] - 1 ? f(x[0] + negArea[tt, 0], x[1] + negArea[tt, 1], x[2] + negArea[tt, 2]) : 0u) > 0u ? 1u : 0u);
                                    }

                                    if (side1 > 0 && side2 > 0)
                                    {
                                        neighbors[t] = 0;
                                    }
                                    else
                                    {
                                        if (a != 0)
                                        {
                                            corner = ((x[d] < dims[d] - 1u ? f(x[0] + q[0] + posArea[t, 0] + posArea[tt, 0], x[1] + q[1] + posArea[t, 1] + posArea[tt, 1], x[2] + q[2] + posArea[t, 2] + posArea[tt, 2]) : 0u) > 0u ? 1u : 0u);
                                        }
                                        else
                                        {
                                            corner = ((x[d] < dims[d] - 1u ? f(x[0] + negArea[t, 0] + negArea[tt, 0], x[1] + negArea[t, 1] + negArea[tt, 1], x[2] + negArea[t, 2] + negArea[tt, 2]) : 0u) > 0u ? 1u : 0u);
                                        }
                                        neighbors[t] = 3u - (side1 + side2 + corner);
                                    }

                                    maskLayout[n].SetOcclusion(t, neighbors[t]);
                                }

                                uint a00 = neighbors[0], a01 = neighbors[1], a11 = neighbors[2], a10 = neighbors[3];
                                if (a00 + a11 == a10 + a01)
                                    maskLayout[n].FlipFace = Math.Max(a00, a11) < Math.Max(a10, a01);
                                else if (a00 + a11 < a10 + a01)
                                    maskLayout[n].FlipFace = true;


                            }
                        }
                    }
                    //Increment x[d]
                    ++x[d];
                    //Generate mesh for mask using lexicographic ordering
                    n = 0;
                    for (j = 0; j < dims[v]; ++j)
                    {
                        for (i = 0; i < dims[u]; )
                        {
                            var c = mask[n];
                            if (c != 0)
                            {
                                var a = maskLayout[n];
                                if (disableGreedyMeshing)
                                {
                                    w = 1;
                                    h = 1;
                                }
                                else
                                {
                                    //Compute width
                                    for (w = 1; c == mask[n + w] && (a.data == (maskLayout[n + w].data)) && i + w < dims[u]; ++w)
                                    {
                                    }
                                    //Compute height (this is slightly awkward
                                    var done = false;
                                    for (h = 1; j + h < dims[v]; ++h)
                                    {
                                        for (k = 0; k < w; ++k)
                                        {
                                            if (c != mask[n + k + h * dims[u]] || a.data != maskLayout[n + k + h * dims[u]].data)
                                            {
                                                done = true;
                                                break;
                                            }
                                        }
                                        if (done)
                                        {
                                            break;
                                        }
                                    }
                                }

                                //Add quad
                                x[u] = i; x[v] = j;
                                int[] du = { 0, 0, 0 };
                                int[] dv = { 0, 0, 0 };

                                if (!maskLayout[n].BackFace)
                                {
                                    dv[v] = h;
                                    du[u] = w;
                                }
                                else
                                {
                                    du[v] = h;
                                    dv[u] = w;
                                }

                                var flip = maskLayout[n].FlipFace;

                                var cr = ((c >> 16) & 0xff) / 255f;
                                var cg = ((c >> 8) & 0xff) / 255f;
                                var cb = (c & 0xff) / 255f;

                                var aouvs = new float[4];
                                float[] AOcurve = new float[] { 0.0f, 0.6f, 0.8f, 1.0f };
                                Color[] ugh = new Color[] { 
                                new Color(1,0,0,1),
                                new Color(0,1,0,1),
                                new Color(0,0,1,1),
                                new Color(1,1,1,1)
                            };
                                for (var o = 0; o < 4; ++o)
                                {
                                    //var ao = AOcurve[maskLayout[n].GetOcclusion(o)];
                                    var ao = disableAO ? 1f : (maskLayout[n].GetOcclusion(o) / 3f);
                                    ////var ao = disableAO ? 1f : (maskLayout[n].GetOcclusion(o) / 4f + 0.25f);
                                    ////if (maskLayout[n].GetOcclusion(o) != 3u)
                                    ////    ao = 0.5f;
                                    ////else
                                    ////    ao = 1f;
                                    //var color = new Color(cr * ao, cg * ao, cb * ao, 1);
                                    //colors.Add(color);
                                    ////colors.Add(new Color(1,0,1,1));

                                    colors.Add(new Color(cr, cg, cb));
                                    uvs.Add(new Vector2(ao, 0));
                                    //colors.Add(ugh[maskLayout[n].GetOcclusion(o)]);

                                    //aouvs[o] = ao;
                                }


                                //for (var o = 0; o < 4; ++o)
                                //{
                                //    uvs.Add(new Vector2(Pack(aouvs[0], aouvs[1]), Pack(aouvs[2], aouvs[3])));
                                //}

                                var vertex_count = vertices.Count;
                                if (centered)
                                {
                                    //This vert generation code will make the 0,0,0 be the center of the mesh in worldspace
                                    vertices.Add(new Vector3(x[0], x[1], x[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                    vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                    vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                    vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                }
                                else
                                {
                                    ////This vert generation code will make the edge of the mesh at 0,0,0
                                    vertices.Add(new Vector3(x[0], x[1], x[2]));
                                    vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]));
                                    vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]));
                                    vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]));
                                }

                                //uvs.Add(new Vector2(0, 1));
                                //uvs.Add(new Vector2(1, 1));
                                //uvs.Add(new Vector2(1, 0));
                                //uvs.Add(new Vector2(0, 0));

                                if (flip)
                                    faces.AddRange(new int[] { vertex_count + 1, vertex_count + 2, vertex_count + 3, vertex_count + 1, vertex_count + 3, vertex_count });
                                else
                                    faces.AddRange(new int[] { vertex_count, vertex_count + 1, vertex_count + 2, vertex_count, vertex_count + 2, vertex_count + 3 });

                                //Zero-out mask
                                for (l = 0; l < h; ++l)
                                {
                                    for (k = 0; k < w; ++k)
                                    {
                                        mask[n + k + l * dims[u]] = 0;
                                        maskLayout[n + k + l * dims[u]].data = 4095u;
                                    }
                                }
                                //Increment counters and continue
                                i += w; n += w;
                            }
                            else
                            {
                                ++i; ++n;
                            }
                        }
                    }
                }
            }

            //mesh.PreMesh(vertices.Count);
            //mesh.UpdateVertices(vertices);
            //mesh.UpdateColors(colors);
            //mesh.UpdateIndices(faces);
            //mesh.UpdateUVs(uvs);
            //mesh.PostMesh();

            return vertices.Count;
        }

        public static int GenerateWaterMesh2(DynamicMesh mesh, Volume volume, bool centered = false, bool disableGreedyMeshing = false)
        {
            vertices.Clear();
            faces.Clear();
            uvs.Clear();
            normals.Clear();
            colors.Clear();

            var dims = volume.dims;

            var f = new Func<int, int, int, uint>((i, j, k) =>
            {
                if (i < 0 || j < 0 || k < 0 || i >= dims[0] || j >= dims[1] || k >= dims[2])
                    return 0;

                var r = volume[i + dims[0] * (j + dims[1] * k)];
                if ((r & 0x1000000u) > 0u)
                {
                    return r;
                }
                return 0;
                //1 + dims[0] * (1 + dims[1] * 1)
                //i    w    j   h    k
                //1 + 16 * (1 + 16 * 1)
                //return 0;
            });

            //Sweep over 3-axes

            for (var d = 1; d < 2; ++d)
            {
                int i, j, k, l, w, h
                  , u = (d + 1) % 3
                  , v = (d + 2) % 3;
                int[] x = { 0, 0, 0 };
                int[] q = { 0, 0, 0 };
                int[,] posArea = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
                int[,] negArea = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

                if (mask.Length < dims[u] * dims[v])
                {
                    mask = new uint[dims[u] * dims[v]];
                    maskLayout = new MaskLayout[dims[u] * dims[v]];
                }
                q[d] = 1;

                posArea[0, u] = -1;
                posArea[1, v] = -1;
                posArea[2, u] = 1;
                posArea[3, v] = 1;

                negArea[0, v] = -1;
                negArea[1, u] = -1;
                negArea[2, v] = 1;
                negArea[3, u] = 1;

                for (x[d] = -1; x[d] < dims[d]; )
                {
                    //Compute mask
                    //TODO: Test if the AOMASK should be created outside of the block mask
                    //the aomask generation might be causing a cache miss per loop
                    var n = 0;
                    for (x[v] = 0; x[v] < dims[v]; ++x[v])
                    {
                        for (x[u] = 0; x[u] < dims[u]; ++x[u], ++n)
                        {
                            var a = (0u <= x[d] ? f(x[0], x[1], x[2]) : 0u);

                            if ((a & 0x1000000u) > 0u)
                                mask[n] = a;

                        }
                    }
                    //Increment x[d]
                    ++x[d];
                    //Generate mesh for mask using lexicographic ordering
                    n = 0;
                    for (j = 0; j < dims[v]; ++j)
                    {
                        for (i = 0; i < dims[u]; )
                        {
                            var c = mask[n];
                            if (c != 0)
                            {
                                var a = maskLayout[n];
                                if (disableGreedyMeshing)
                                {
                                    w = 1;
                                    h = 1;
                                }
                                else
                                {
                                    //Compute width
                                    for (w = 1; c == mask[n + w] && (a.data == (maskLayout[n + w].data)) && i + w < dims[u]; ++w)
                                    {
                                    }
                                    //Compute height (this is slightly awkward
                                    var done = false;
                                    for (h = 1; j + h < dims[v]; ++h)
                                    {
                                        for (k = 0; k < w; ++k)
                                        {
                                            if (c != mask[n + k + h * dims[u]] || a.data != maskLayout[n + k + h * dims[u]].data)
                                            {
                                                done = true;
                                                break;
                                            }
                                        }
                                        if (done)
                                        {
                                            break;
                                        }
                                    }
                                }

                                //Add quad
                                x[u] = i; x[v] = j;
                                for (var backface = 0; backface < 2; ++backface)
                                {
                                    int[] du = { 0, 0, 0 };
                                    int[] dv = { 0, 0, 0 };

                                    if (backface == 0)
                                    {
                                        dv[v] = h;
                                        du[u] = w;
                                    }
                                    else
                                    {
                                        du[v] = h;
                                        dv[u] = w;
                                    }

                                    var flip = maskLayout[n].FlipFace;

                                    var cr = ((c >> 16) & 0xff) / 255f;
                                    var cg = ((c >> 8) & 0xff) / 255f;
                                    var cb = (c & 0xff) / 255f;

                                    for (var o = 0; o < 4; ++o)
                                    {
                                        colors.Add(new Color(cr, cg, cb));
                                        uvs.Add(new Vector2(0, 0));
                                    }

                                    var vertex_count = vertices.Count;
                                    var offset = Vector3.Zero;
                                    if (centered)
                                        offset = new Vector3(dims[0], dims[1], dims[2]) / 2f;

                                    //offset += new Vector3(0, 0.5f, 0);
                                    vertices.Add(new Vector3(x[0], x[1], x[2]) - offset);
                                    vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]) - offset);
                                    vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]) - offset);
                                    vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]) - offset);


                                    faces.AddRange(new int[] { vertex_count, vertex_count + 1, vertex_count + 2, vertex_count, vertex_count + 2, vertex_count + 3 });
                                }

                                //Zero-out mask
                                for (l = 0; l < h; ++l)
                                {
                                    for (k = 0; k < w; ++k)
                                    {
                                        mask[n + k + l * dims[u]] = 0;
                                        maskLayout[n + k + l * dims[u]].data = 4095u;
                                    }
                                }
                                //Increment counters and continue
                                i += w; n += w;
                            }
                            else
                            {
                                ++i; ++n;
                            }
                        }
                    }
                }
            }

            //mesh.PreMesh(vertices.Count);
            //mesh.UpdateVertices(vertices);
            //mesh.UpdateColors(colors);
            //mesh.UpdateIndices(faces);
            //mesh.UpdateUVs(uvs);
            //mesh.PostMesh();

            return vertices.Count;
        }

    }

    public enum Shape
    {
        Sphere,
        Noise,
        Test,
        Solid
    }


    public class VolumeGenerator //: MonoBehaviour
    {
        public bool wireframe = false;
        public bool enableGreedy = true;
        public bool enableAO = true;
        public Shape shape = Shape.Sphere;
        public int width = 16;
        public int height = 16;
        public int depth = 16;
        public Color color = Color.White;
        public bool noiseyColor = true;


        public const int size = 8;

        public Volume GenerateVolume()
        {
            System.Func<int, int, int, uint> shape_func;

            switch (shape)
            {
                default:
                case Shape.Sphere:
                    shape_func = new System.Func<int, int, int, uint>((i, j, k) =>
                    {
                        var hs = size / 2;

                        if (i * i + j * j + k * k <= hs * hs)
                        {
                            if (noiseyColor && Random.Range(0f, 1f) < 0.1f)
                            {
                                var c = color * Random.Range(0.8f, 0.9f);
                                return (uint)(color.ToARGB() & 0xffffff);
                            }
                            return (uint)(color.ToARGB() & 0xffffff);
                        }
                        return 0;
                    });
                    break;
                case Shape.Noise:
                    shape_func = new System.Func<int, int, int, uint>((i, j, k) =>
                    {
                        var r = (Random.Range(0, 255) << 16) + (Random.Range(0, 255) << 8) + Random.Range(0, 255);
                        return Random.Range(0f, 1f) < 0.05f ? 0xffffffu : 0u;
                        //return Random.Range(0f, 1f) < 0.01f ? r : 0;
                    });
                    break;
                case Shape.Test:
                    shape_func = new System.Func<int, int, int, uint>((i, j, k) =>
                    {
                        if (k == 0)
                            return 0xffffff;

                        if (i == 0 && j == 0)
                            return 0xffffff;
                        if (i == 1 && j == 1)
                            return 0xffffff;
                        return 0;
                        //var r = (Random.Range(0, 255) << 16) + (Random.Range(0, 255) << 8) + Random.Range(0, 255);
                        //return Random.Range(0f, 1f) < 0.1f ? 0xffffff : 0;
                        //return Random.Range(0f, 1f) < 0.01f ? r : 0;
                    });
                    break;
                case Shape.Solid:
                    shape_func = new System.Func<int, int, int, uint>((i, j, k) =>
                    {
                        var r = (uint)((Random.Range(0, 255) << 16) + (Random.Range(0, 255) << 8) + Random.Range(0, 255));
                        return r;
                    });
                    break;

            }

            var volume = SurfaceExtractor.makeVoxels(0, 0, 0, new int[] { -width, -height, -depth }, new int[] { width, height, depth }, shape_func);

            //chunk = Assets.Source.Voxels.SurfaceExtractor.makeVoxels(new int[] { 0, 0, 0 }, new int[] { 0, 0, 1 }, solid);
            //chunk = Assets.Source.Voxels.SurfaceExtractor.makeVoxels(new int[] { -16, -16, -16 }, new int[] { 16, 16, 16 }, noise);
            //SurfaceExtractor.disableAO = !enableAO;
            //SurfaceExtractor.disableGreedyMeshing = !enableGreedy;
            SurfaceExtractor.GenerateMesh(volume, disableGreedyMeshing: !enableGreedy, disableAO: !enableAO, centered: true);
            return volume;


            //mesh.UploadMeshData(true);


            //mesh.SetIndices()
        }


    }


    //public class ChunkManager //: RenderQueue
    //{
    //    //new code



    //    //old code

    //    public Material opaqueMaterial;
    //    public Material waterMaterial;
    //    public int resolution = 64;
    //    public NoiseType noise = NoiseType.Perlin;
    //    public float zoom = 1f;
    //    public float offset = 0f;
    //    public bool wireframe = false;

    //    public const int GRID_SIZE = 16; // MAX 64
    //    public const int CHUNK_HALFSIZE = 8;
    //    public const int CHUNK_SIZE = CHUNK_HALFSIZE + CHUNK_HALFSIZE; //MAX 64
    //    public const int MAP_DEPTH = 32; //MAX 256;

    //    private Volume[,] chunks = new Volume[GRID_SIZE, GRID_SIZE];

    //    private Volume hover;
    //    private int sealevel = 8;

    //    public void Start()
    //    {
    //        hover = SurfaceExtractor.makeVoxels(null, 0, 0, 0,
    //                    new int[] { 0, 0, 0 },
    //                    new int[] { 1, 1, 1 },
    //                        (i, j, k) =>
    //                        {
    //                            return 0xff0000;
    //                        }
    //                    );

    //        hover.PrepareMesh();
    //        SurfaceExtractor.GenerateMesh(null, hover, centered: true);

    //        for (var x = 0; x < GRID_SIZE; ++x)
    //        {
    //            for (var z = 0; z < GRID_SIZE; ++z)
    //            {
    //                var chunk = SurfaceExtractor.makeVoxels(this, x * CHUNK_SIZE, 0, z * CHUNK_SIZE,
    //                    new int[] { 0, 0, 0 },
    //                    new int[] { CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE },
    //                       Generators.GenerateHeight(x, 0, z, CHUNK_SIZE, CHUNK_SIZE, noise, sealevel)
    //                    );

    //                chunks[x, z] = chunk;
    //            }
    //        }

    //        var opaque = Resources.Load<GameObject>("Prefabs/ChunkOpaque");
    //        var water = Resources.Load<GameObject>("Prefabs/ChunkWater");
    //        for (var x = 0; x < GRID_SIZE; ++x)
    //        {
    //            for (var y = 0; y < GRID_SIZE; ++y)
    //            {
    //                chunks[x, y].UpdateMesh();
    //                {
    //                    var r = Instantiate(opaque, new Vector3(chunks[x, y].X, chunks[x, y].Y, chunks[x, y].Z), Quaternion.identity);
    //                    r.name = string.Format("CHUNK_{0}_{1}_OPAQUE", x, y);
    //                    var rgo = (GameObject)r;
    //                    rgo.transform.parent = this.transform;
    //                    var filter = rgo.GetComponent<MeshFilter>();
    //                    filter.mesh = chunks[x, y].opaqueMesh;
    //                }
    //                {
    //                    var r = Instantiate(water, new Vector3(chunks[x, y].X, chunks[x, y].Y, chunks[x, y].Z), Quaternion.identity);
    //                    r.name = string.Format("CHUNK_{0}_{1}_WATER", x, y);
    //                    var rgo = (GameObject)r;
    //                    rgo.transform.parent = this.transform;
    //                    var filter = rgo.GetComponent<MeshFilter>();
    //                    filter.mesh = chunks[x, y].waterMesh;
    //                }
    //            }
    //        }


    //    }

    //    public void OnGUI()
    //    {
    //        var r = RayCast(Input.mousePosition);
    //        var p = r.direction;
    //        var start = r.origin;
    //        var end = r.direction * 100f + r.origin;

    //        GUI.Label(new Rect(0, 0, 200, 20), start.ToString());
    //        GUI.Label(new Rect(0, 10, 200, 20), end.ToString());

    //        foreach (var rp in GridRayTracer.Trace(start, end))
    //        {
    //            var wx = (int)rp.X;
    //            var wy = (int)rp.Y;
    //            var wz = (int)rp.z;

    //            if (GetBlock(wx, wy, wz) > 0)
    //            {
    //                GUI.Label(new Rect(0, 20, 200, 20), rp.ToString());
    //                return;
    //            }
    //        }

    //        GUI.Label(new Rect(0, 20, 200, 20), "No hits");
    //    }

    //    public uint GetBlock(int wx, int wy, int wz)
    //    {
    //        if (wy < 0 || wy >= CHUNK_SIZE)
    //            return 0;

    //        if (wx < 0 || wx >= CHUNK_SIZE * GRID_SIZE)
    //            return 0;

    //        if (wz < 0 || wz >= CHUNK_SIZE * GRID_SIZE)
    //            return 0;

    //        var cx = wx / CHUNK_SIZE;
    //        var cz = wz / CHUNK_SIZE;
    //        var ox = wx % CHUNK_SIZE;
    //        var oz = wz % CHUNK_SIZE;

    //        var chunk = chunks[cx, cz];
    //        return chunk[ox, wy, oz];
    //    }

    //    public Ray RayCast(Vector3 mp)
    //    {
    //        return Camera.main.ScreenPointToRay(mp);
    //    }

    //    Vector3 start, end;

    //    public override void RenderWater()
    //    {

    //        GL.wireframe = wireframe;
    //        if (waterMaterial != null)
    //        {
    //            for (int pass = 0; pass < waterMaterial.passCount; pass++)
    //            {
    //                waterMaterial.SetPass(pass);
    //                for (var x = 0; x < GRID_SIZE; ++x)
    //                    for (var y = 0; y < GRID_SIZE; ++y)
    //                        chunks[x, y].RenderAlpha();
    //            }
    //        }
    //        GL.wireframe = false;
    //    }

    //    public override void RenderOpaque()
    //    {
    //        //GL.wireframe = wireframe;
    //        //if (opaqueMaterial != null)
    //        //{
    //        //    for (int pass = 0; pass < opaqueMaterial.passCount; pass++)
    //        //    {
    //        //        opaqueMaterial.SetPass(pass);
    //        //        for (var x = 0; x < GRID_SIZE; ++x)
    //        //            for (var y = 0; y < GRID_SIZE; ++y)
    //        //                chunks[x, y].RenderOpaque();
    //        //    }
    //        //}
    //        //GL.wireframe = false;

    //        //if (Input.GetKey(KeyCode.Z))
    //        {
    //            var r = RayCast(Input.mousePosition);
    //            var p = r.direction;
    //            start = r.origin;
    //            end = r.direction * 100f + r.origin;
    //        }

    //        //HACK: hover is using water's shader
    //        foreach (var rp in GridRayTracer.Trace(start, end))
    //        {
    //            var wx = (int)rp.X;
    //            var wy = (int)rp.Y;
    //            var wz = (int)rp.z;
    //            //Graphics.DrawMeshNow(hover.mesh, m);

    //            if (GetBlock(wx, wy, wz) > 0)
    //            {
    //                var m = Matrix4x4.TRS(new Vector3(wx, wy, wz) + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, Vector3.one * 1.1f);
    //                Graphics.DrawMeshNow(hover.opaqueMesh, m);
    //                return;
    //            }
    //        }

    //    }
    //}

}

public static class Random
{
    private static readonly System.Random g_random = new System.Random();
    public static float Range(float min, float max)
    {
        var value = (float)g_random.NextDouble();
        return (value * (max - min)) + min;
    }

    public static int Range(int min, int max)
    {
        var value = (float)g_random.NextDouble();
        return (int)((value * (max - min)) + min);
    }

    public static Vector3 Range(Vector3 min, Vector3 max)
    {
        return new Vector3(
                Range(min.X, max.X),
                Range(min.Y, max.Y),
                Range(min.Z, max.Z)
            );
    }

    public static Vector2 Range(Vector2 min, Vector2 max)
    {
        return new Vector2(
                Range(min.X, max.X),
                Range(min.Y, max.Y)
            );
    }
}
