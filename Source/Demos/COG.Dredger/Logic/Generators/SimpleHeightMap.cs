using COG.LibNoise;
using COG.LibNoise.Generator;
using COG.LibNoise.Operator;
namespace COG.Dredger.Logic
{
    public partial class Generators
    {
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