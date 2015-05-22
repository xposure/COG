using COG.LibNoise;
using COG.LibNoise.Generator;
using COG.LibNoise.Operator;
namespace COG.Dredger.Logic
{
    public partial class Generators
    {
        public static System.Func<int, int, uint> SimpleHeight(int cx, int cy, int cz, int width, int depth, float scale, NoiseType noise = NoiseType.Perlin, int sealevel = 8)
        {
            ModuleBase moduleBase;
            switch (noise)
            {
                case NoiseType.Billow:
                    moduleBase = new Billow();
                    break;

                case NoiseType.RiggedMultifractal:
                    moduleBase = new RiggedMultifractal();
                    break;

                case NoiseType.Voronoi:
                    moduleBase = new Voronoi();
                    break;

                case NoiseType.Mix:
                    Perlin perlin = new Perlin();
                    RiggedMultifractal rigged = new RiggedMultifractal();
                    moduleBase = new Add(perlin, rigged);
                    break;

                default:
                    moduleBase = new Perlin();
                    break;

            }

            var data = new uint[width, depth];
            for (var x = 0; x < width; ++x)
            {
                for (var z = 0; z < depth; ++z)
                {
                    var wx = cx * width + x;
                    var wz = cz * depth + z;
                    var l0 = (moduleBase.GetValue(wx * scale, wz * scale, 0.95f) +1f) / 2f;
                    if (l0 < 0f)
                    {

                    }

                    l0 *= 16;

                    //l0 += moduleBase.GetValue(wx * (scale * scale) + 100, wz * (scale * scale) + 100, 1.15f) * 4f;
                    //l0 += moduleBase.GetValue(wx * (scale * 2) + 1000, wz * (scale * 2) + 1000, 1.15f) * 4f;
                    data[x, z] = (uint)l0;
                    //data[x, z] += (uint)(moduleBase.GetValue(wx * 0.015f, wz * 0.015f, 0.15f) * 1.4 * moduleBase.GetValue(wx * 0.025f, wz * 0.025f, 0.45f) * 32 + 8);
                }
            }

            return (i, k) =>
            {
                return data[i, k];
            };
        }


        public static System.Func<int, int, int, uint> GenerateHeight(int cx, int cy, int cz, int width, int depth, NoiseType noise = NoiseType.Perlin, int sealevel = 8)
        {
            ModuleBase moduleBase;
            switch (noise)
            {
                case NoiseType.Billow:
                    moduleBase = new Billow();
                    break;

                case NoiseType.RiggedMultifractal:
                    moduleBase = new RiggedMultifractal();
                    break;

                case NoiseType.Voronoi:
                    moduleBase = new Voronoi();
                    break;

                case NoiseType.Mix:
                    Perlin perlin = new Perlin();
                    RiggedMultifractal rigged = new RiggedMultifractal();
                    moduleBase = new Add(perlin, rigged);
                    break;

                default:
                    moduleBase = new Perlin();
                    break;

            }

            var data = new uint[width, depth];
            for (var x = 0; x < width; ++x)
            {
                for (var z = 0; z < depth; ++z)
                {
                    var wx = cx * width + x;
                    var wz = cz * depth + z;
                    data[x, z] = (uint)(moduleBase.GetValue(wx * 0.035f, wz * 0.035f, 0.95f) * 2 + 8);
                    data[x, z] += (uint)(moduleBase.GetValue(wx * 0.015f, wz * 0.015f, 0.15f) * moduleBase.GetValue(wx * 0.025f, wz * 0.025f, 0.45f) * 7 + 2);
                }
            }

            return (i, j, k) =>
            {
                if (data[i, k] > j)
                {
                    if (sealevel > j)
                        return 0xD6D68Eu;

                    var terrainNoise = Random.Range(0f, 1f);
                    if (terrainNoise < 0.005f)
                        return 0x88A552u;
                    else if (terrainNoise < 0.01f)
                        return 0x9CCB6Bu;

                    return 0x5F9E35u;
                }
                else if (sealevel > j)
                    return 0x177B0E5u;


                return 0;
            };
        }

    }
}