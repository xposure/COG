using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Dredger.World;
using COG.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace COG.Dredger.Logic
{
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

    public class Surface
    {
        public static int Extract(DynamicMesh mesh, Voxel[] data, int xw, int zw, int yw)
        {
            mesh.Begin();

            var dims = new[] { xw, yw, zw };
            var mask = new byte[xw * zw];
            var maskLayout = new MaskLayout[xw * zw];
            var hasNeighbors = false;
            var disableAO = false;
            var disableGreedyMeshing = false;
            var neighborOffset = hasNeighbors ? 1 : 0;
            var vertex_count = 0;

            var f = new Func<int, int, int, byte>((i, j, k) =>
            {
                if (i < 0 || j < 0 || k < 0 || i >= dims[0] || j >= dims[1] || k >= dims[2])
                    return 0;

                return data[(k * zw * yw) + (i * xw) + j].m_byte;
                //return (z * Config.MAP_COLUMN_SIZE * Config.MAP_COLUMN_SIZE) + (x * Config.MAP_COLUMN_SIZE);
                //return data[i + dims[0] * (j + dims[1] * k)].m_byte;
                //return data[i + dims[0] * (j + dims[1] * k)].m_byte;
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
                    mask = new byte[dims[u] * dims[v]];
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
                            var a = (byte)(0 <= x[d] ? f(x[0], x[1], x[2]) : 0);
                            var b = (byte)(x[d] < dims[d] - 1 ? f(x[0] + q[0], x[1] + q[1], x[2] + q[2]) : 0);

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
                    for (j = 0; j < dims[v] - neighborOffset; ++j)
                    {
                        for (i = 0; i < dims[u] - neighborOffset; )
                        {
                            var c = mask[n];
                            if (c != 0)
                            {
                                var a = maskLayout[n];
                                if (hasNeighbors && (j == 0 || i == 0 || j == dims[v] - 1 || i == dims[u] - 1))
                                {
                                    mask[n] = 0;
                                    maskLayout[n].data = 4095u;
                                    ++i; ++n;

                                    continue;
                                }
                                else
                                    if (disableGreedyMeshing)
                                    {
                                        w = 1;
                                        h = 1;
                                    }
                                    else
                                    {
                                        //Compute width
                                        for (w = 1; c == mask[n + w] && (a.data == (maskLayout[n + w].data)) && i + w < dims[u] - neighborOffset; ++w)
                                        {
                                        }
                                        //Compute height (this is slightly awkward
                                        var done = false;
                                        for (h = 1; j + h < dims[v] - neighborOffset; ++h)
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

                                //var vertices = s.vertices;
                                //var vertex_count = s.vertices.Count;
                                //if (centered)
                                //{
                                //    //This vert generation code will make the 0,0,0 be the center of the mesh in worldspace
                                //    s.vertices.Add(new Vector3(x[0], x[1], x[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                //    s.vertices.Add(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                //    s.vertices.Add(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                //    s.vertices.Add(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]) - new Vector3(dims[0], dims[1], dims[2]) / 2f);
                                //}
                                //else
                                //{
                                    ////This vert generation code will make the edge of the mesh at 0,0,0

                                var position = new[] { 
                                    new Vector3(x[0], x[1], x[2]) ,
                                    new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                    new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                    new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2])
                                };

                                //    mesh.Position(new Vector3(x[0], x[1], x[2]));
                                //    mesh.Position(new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]));
                                //    mesh.Position(new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]));
                                //    mesh.Position(new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]));


                                //var cr = ((c >> 16) & 0xff) / 255f;
                                //var cg = ((c >> 8) & 0xff) / 255f;
                                //var cb = (c & 0xff) / 255f;
                                var desc = VoxelDescriptor.Find(c);
                                //var block = MapBlock.GetType(c);
                                var aouvs = new float[4];
                                float[] AOcurve = new float[] { 0.0f, 0.6f, 0.8f, 1.0f };
                                for (var o = 0; o < 4; ++o)
                                {
                                    var ao = disableAO ? 1f : (maskLayout[n].GetOcclusion(o) / 3f);
                                    mesh.Position(position[o]);
                                    mesh.TextureCoord(new Vector2(ao, 0));
                                    mesh.Color(desc.Color);
                                }

                                if (flip)
                                {
                                    mesh.Index((ushort)(vertex_count + 1));
                                    mesh.Index((ushort)(vertex_count + 2));
                                    mesh.Index((ushort)(vertex_count + 3));
                                    mesh.Index((ushort)(vertex_count + 1));
                                    mesh.Index((ushort)(vertex_count + 3));
                                    mesh.Index((ushort)(vertex_count));
                                    //s.faces.AddRange(new int[] { vertex_count + 1, vertex_count + 2, vertex_count + 3, vertex_count + 1, vertex_count + 3, vertex_count });
                                }
                                else
                                {
                                    mesh.Index((ushort)(vertex_count));
                                    mesh.Index((ushort)(vertex_count + 1));
                                    mesh.Index((ushort)(vertex_count + 2));
                                    mesh.Index((ushort)(vertex_count));
                                    mesh.Index((ushort)(vertex_count + 2));
                                    mesh.Index((ushort)(vertex_count + 3));
                                    //s.faces.AddRange(new int[] { vertex_count, vertex_count + 1, vertex_count + 2, vertex_count, vertex_count + 2, vertex_count + 3 });
                                }
                                vertex_count += 4;

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

            mesh.End(BufferUsageHint.DynamicDraw);
            return vertex_count;

        }
    }
}
