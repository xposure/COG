using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        // this is the default palette of voxel colors (the RGBA chunk is only included if the palette is differe)
        private static ushort[] voxColors = new ushort[] { 32767, 25599, 19455, 13311, 7167, 1023, 32543, 25375, 19231, 13087, 6943, 799, 32351, 25183, 
            19039, 12895, 6751, 607, 32159, 24991, 18847, 12703, 6559, 415, 31967, 24799, 18655, 12511, 6367, 223, 31775, 24607, 18463, 12319, 6175, 31, 
            32760, 25592, 19448, 13304, 7160, 1016, 32536, 25368, 19224, 13080, 6936, 792, 32344, 25176, 19032, 12888, 6744, 600, 32152, 24984, 18840, 
            12696, 6552, 408, 31960, 24792, 18648, 12504, 6360, 216, 31768, 24600, 18456, 12312, 6168, 24, 32754, 25586, 19442, 13298, 7154, 1010, 32530, 
            25362, 19218, 13074, 6930, 786, 32338, 25170, 19026, 12882, 6738, 594, 32146, 24978, 18834, 12690, 6546, 402, 31954, 24786, 18642, 12498, 6354, 
            210, 31762, 24594, 18450, 12306, 6162, 18, 32748, 25580, 19436, 13292, 7148, 1004, 32524, 25356, 19212, 13068, 6924, 780, 32332, 25164, 19020, 
            12876, 6732, 588, 32140, 24972, 18828, 12684, 6540, 396, 31948, 24780, 18636, 12492, 6348, 204, 31756, 24588, 18444, 12300, 6156, 12, 32742, 
            25574, 19430, 13286, 7142, 998, 32518, 25350, 19206, 13062, 6918, 774, 32326, 25158, 19014, 12870, 6726, 582, 32134, 24966, 18822, 12678, 6534, 
            390, 31942, 24774, 18630, 12486, 6342, 198, 31750, 24582, 18438, 12294, 6150, 6, 32736, 25568, 19424, 13280, 7136, 992, 32512, 25344, 19200, 
            13056, 6912, 768, 32320, 25152, 19008, 12864, 6720, 576, 32128, 24960, 18816, 12672, 6528, 384, 31936, 24768, 18624, 12480, 6336, 192, 31744, 
            24576, 18432, 12288, 6144, 28, 26, 22, 20, 16, 14, 10, 8, 4, 2, 896, 832, 704, 640, 512, 448, 320, 256, 128, 64, 28672, 26624, 22528, 20480, 
            16384, 14336, 10240, 8192, 4096, 2048, 29596, 27482, 23254, 21140, 16912, 14798, 10570, 8456, 4228, 2114, 1  };

        private struct MagicaVoxelData
        {
            public byte x;
            public byte y;
            public byte z;
            public byte color;

            public MagicaVoxelData(BinaryReader stream, bool subsample)
            {
                x = (byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
                y = (byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
                z = (byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
                color = stream.ReadByte();
            }
        }

        /// <summary>
        /// Load a MagicaVoxel .vox format file into the custom ushort[] structure that we use for voxel chunks.
        /// </summary>
        /// <param name="stream">An open BinaryReader stream that is the .vox file.</param>
        /// <param name="overrideColors">Optional color lookup table for converting RGB values into my internal engine color format.</param>
        /// <returns>The voxel chunk data for the MagicaVoxel .vox file.</returns>
        private static ushort[] FromMagica(BinaryReader stream)
        {
            // check out http://voxel.codeplex.com/wikipage?title=VOX%20Format&referringTitle=Home for the file format used below
            // we're going to return a voxel chunk worth of data
            ushort[] data = new ushort[32 * 128 * 32];
            ushort[] colors = null;
            MagicaVoxelData[] voxelData = null;

            string magic = new string(stream.ReadChars(4));
            int version = stream.ReadInt32();

            // a MagicaVoxel .vox file starts with a 'magic' 4 character 'VOX ' identifier
            if (magic == "VOX ")
            {
                int sizex = 0, sizey = 0, sizez = 0;
                bool subsample = false;

                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    // each chunk has an ID, size and child chunks
                    char[] chunkId = stream.ReadChars(4);
                    int chunkSize = stream.ReadInt32();
                    int childChunks = stream.ReadInt32();
                    string chunkName = new string(chunkId);

                    // there are only 2 chunks we only care about, and they are SIZE and XYZI
                    if (chunkName == "SIZE")
                    {
                        sizex = stream.ReadInt32();
                        sizey = stream.ReadInt32();
                        sizez = stream.ReadInt32();

                        if (sizex > 32 || sizey > 32) subsample = true;

                        stream.ReadBytes(chunkSize - 4 * 3);
                    }
                    else if (chunkName == "XYZI")
                    {
                        // XYZI contains n voxels
                        int numVoxels = stream.ReadInt32();
                        int div = (subsample ? 2 : 1);

                        // each voxel has x, y, z and color index values
                        voxelData = new MagicaVoxelData[numVoxels];
                        for (int i = 0; i < voxelData.Length; i++)
                            voxelData[i] = new MagicaVoxelData(stream, subsample);
                    }
                    else if (chunkName == "RGBA")
                    {
                        colors = new ushort[256];

                        for (int i = 0; i < 256; i++)
                        {
                            byte r = stream.ReadByte();
                            byte g = stream.ReadByte();
                            byte b = stream.ReadByte();
                            byte a = stream.ReadByte();

                            // convert RGBA to our custom voxel format (16 bits, 0RRR RRGG GGGB BBBB)
                            colors[i] = (ushort)(((r & 0x1f) << 10) | ((g & 0x1f) << 5) | (b & 0x1f));
                        }
                    }
                    else stream.ReadBytes(chunkSize);   // read any excess bytes
                }

                if (voxelData.Length == 0) return data; // failed to read any valid voxel data

                // now push the voxel data into our voxel chunk structure
                for (int i = 0; i < voxelData.Length; i++)
                {
                    // do not store this voxel if it lies out of range of the voxel chunk (32x128x32)
                    if (voxelData[i].x > 31 || voxelData[i].y > 31 || voxelData[i].z > 127) continue;

                    // use the voxColors array by default, or overrideColor if it is available
                    int voxel = (voxelData[i].x + voxelData[i].z * 32 + voxelData[i].y * 32 * 128);
                    data[voxel] = (colors == null ? voxColors[voxelData[i].color - 1] : colors[voxelData[i].color - 1]);
                }
            }

            return data;
        }
    }
}
