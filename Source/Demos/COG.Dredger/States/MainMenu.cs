using System;
using COG.Dredger.Logic;
using COG.Dredger.Rendering;
using COG.Framework;
using COG.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;


namespace COG.Dredger.States
{
    public class ParticleEmitter : DisposableObject
    {
        public struct Particle
        {
            public float life;
            public float decay;
            public byte r, g, b, x;
            public float speed;
            public Vector2 size;
            public Vector3 position;
            public Vector3 direction;
        }

        private SpriteRenderer m_renderer;

        private int m_particleIndex = 0;
        private Particle[] m_particles = new Particle[100000];
        private float timer = 1f;

        public ParticleEmitter()
        {
            m_renderer = new SpriteRenderer();
        }

        public void Update(double dt)
        {
            int newparticles = (int)(dt * 10000.0);
            if (newparticles > (int)(0.016f * 10000.0))
                newparticles = (int)(0.016f * 10000.0);

            while (newparticles-- > 0)
            {
                if (m_particleIndex < m_particles.Length)
                {
                    //if (rnd.Next(1000) < 10)
                    {
                        var particle = m_particles[m_particleIndex];
                        particle.life = 1;
                        particle.speed = Random.Range(0.5f, 1f);
                        particle.r = (byte)Random.Range(0, 255);
                        particle.g = (byte)Random.Range(0, 255);
                        particle.b = (byte)Random.Range(0, 255);
                        particle.speed = Random.Range(0f, 2f);
                        particle.direction = Random.Range(-Vector3.One, Vector3.One);
                        particle.position = Random.Range(-Vector3.One, Vector3.One) / 40f;
                        particle.size = Random.Range(Vector2.One / 4f, Vector2.One / 2f) * 0.15f;

                        m_particles[m_particleIndex] = particle;
                        m_particleIndex++;
                    }
                }

            }

            var fdt = (float)dt;
            for (var i = 0; i < m_particleIndex; i++)
            {
                m_particles[i].life -= fdt;

                if (m_particles[i].life <= 0)
                    m_particles[i--] = m_particles[--m_particleIndex];
                else
                {
                    m_particles[i].position = m_particles[i].position + (m_particles[i].direction * m_particles[i].speed * fdt);
                }
            }

            timer -= (float)dt;
            if (timer <= 0)
            {
                timer += 1f;
                Console.WriteLine("Particles: {0}", m_particleIndex);
            }

        }

        public void Render(Program program, Texture2D texture, double dt)
        {
            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            for (var i = 0; i < m_particleIndex; i++)
            {
                var p = m_particles[i];
                var sprite = Sprite.Create(texture, p.position.X, p.position.Y, p.position.X + p.size.X, p.position.Y - p.size.Y);
                sprite.SetDepth(p.position.Z);
                sprite.SetColor(new Color(p.life, p.r / 255f, p.g / 255f, p.b / 255f));

                m_renderer.AddQuad(sprite);
            }

            m_renderer.Render(program);
        }

        protected override void DisposedUnmanaged()
        {
            base.DisposedUnmanaged();

            if (m_renderer)
                m_renderer.Dispose();
        }

    }

    public class MainMenu : GameState
    {
        private Texture2D m_texture;
        private SpriteRenderer m_spriteRenderer;
        private Program m_program;
        private Program m_colorProgram;
        private Program m_spriteProgram;
        private Program m_opaqueChunkProgram;

        private int vertexArrayID;
        private DynamicMesh m_mesh;
        private StreamMesh m_mesh2;
        private ParticleEmitter m_particles;
        private AutoVertexUniformMatrix4 m_view, m_projection, m_viewProjection;
        private Volume m_volume;
        private VolumeGenerator m_vgen;
        private ChunkManager m_chunks;
        private Camera m_camera;
        private bool use_camera = false;

        private Atma.Font m_font;

        public override void Initialize(Engine engine)
        {
            base.Initialize(engine);

            m_view = engine.Programs.CreateAutoUniformMatrix4("view");
            m_projection = engine.Programs.CreateAutoUniformMatrix4("projection");
            m_viewProjection = engine.Programs.CreateAutoUniformMatrix4("viewProjection");

            m_vgen = new VolumeGenerator();
            m_vgen.enableAO = true;
            m_vgen.enableGreedy = true;
            m_camera = new Camera();
        }

        public override void LoadResources()
        {
            base.LoadResources();
            m_spriteRenderer = new SpriteRenderer();
            m_particles = new ParticleEmitter();

            m_texture = TextureData2D.CreateMetaball(10, TextureData2D.CircleFalloff, TextureData2D.ColorWhite);
            m_program = m_engine.Assets.LoadProgram("dredger:program:simple");
            m_colorProgram = m_engine.Assets.LoadProgram("dredger:program:color");
            m_opaqueChunkProgram = m_engine.Assets.LoadProgram("dredger:program:opaqueChunk");
            m_spriteProgram = m_engine.Assets.LoadProgram("dredger:program:sprite");
            m_font = m_engine.Assets.LoadFont("dredger:font:arial");

            m_chunks = new ChunkManager();
            m_chunks.Initialize();

            vertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayID);

            var decl = new VertexDeclaration();
            decl.AddElement(3, VertexAttribPointerType.Float, VertexElementSemantic.Position);
            decl.AddElement(2, VertexAttribPointerType.Float, VertexElementSemantic.TexCoord0);

            m_mesh = new DynamicMesh(decl);
            m_mesh2 = new StreamMesh(decl);

            m_volume = m_vgen.GenerateVolume();

            GenerateCube();

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1f);
        }

        private void GenerateCube()
        {

            // Our vertices. Tree consecutive floats give a 3D vertex; Three consecutive vertices give a triangle.
            // A cube has 6 faces with 2 triangles each, so this makes 6*2=12 triangles, and 12*3 vertices
            var g_all_buffer_data = new[] {
                    -1.0f,-1.0f,-1.0f, // triangle 1 : begin  //bl
                    -1.0f,-1.0f, 1.0f,                        //    
                    -1.0f, 1.0f, 1.0f, // triangle 1 : end    //
                    1.0f, 1.0f,-1.0f, // triangle 2 : begin   //
                    -1.0f,-1.0f,-1.0f,                        //
                    -1.0f, 1.0f,-1.0f, // triangle 2 : end    //

                    1.0f,-1.0f, 1.0f,
                    -1.0f,-1.0f,-1.0f,
                    1.0f,-1.0f,-1.0f,
                    1.0f, 1.0f,-1.0f,
                    1.0f,-1.0f,-1.0f,
                    -1.0f,-1.0f,-1.0f,
                    
                    -1.0f,-1.0f,-1.0f,
                    -1.0f, 1.0f, 1.0f,
                    -1.0f, 1.0f,-1.0f,
                    1.0f,-1.0f, 1.0f,
                    -1.0f,-1.0f, 1.0f,
                    -1.0f,-1.0f,-1.0f,
                    
                    -1.0f, 1.0f, 1.0f,
                    -1.0f,-1.0f, 1.0f,
                    1.0f,-1.0f, 1.0f,
                    1.0f, 1.0f, 1.0f,
                    1.0f,-1.0f,-1.0f,                    
                    1.0f, 1.0f,-1.0f,
                    
                    1.0f,-1.0f,-1.0f,
                    1.0f, 1.0f, 1.0f,
                    1.0f,-1.0f, 1.0f,
                    1.0f, 1.0f, 1.0f,
                    1.0f, 1.0f,-1.0f,
                    -1.0f, 1.0f,-1.0f,
                    
                    1.0f, 1.0f, 1.0f,
                    -1.0f, 1.0f,-1.0f,
                    -1.0f, 1.0f, 1.0f,
                    1.0f, 1.0f, 1.0f,
                    -1.0f, 1.0f, 1.0f,
                    1.0f,-1.0f, 1.0f,
                    
                    0.000059f, 1.0f-0.000004f,
                    0.000103f, 1.0f-0.336048f,
                    0.335973f, 1.0f-0.335903f,
                    1.000023f, 1.0f-0.000013f,
                    0.667979f, 1.0f-0.335851f,
                    0.999958f, 1.0f-0.336064f,
                    0.667979f, 1.0f-0.335851f,
                    0.336024f, 1.0f-0.671877f,
                    0.667969f, 1.0f-0.671889f,
                    1.000023f, 1.0f-0.000013f,
                    0.668104f, 1.0f-0.000013f,
                    0.667979f, 1.0f-0.335851f,
                    0.000059f, 1.0f-0.000004f,
                    0.335973f, 1.0f-0.335903f,
                    0.336098f, 1.0f-0.000071f,
                    0.667979f, 1.0f-0.335851f,
                    0.335973f, 1.0f-0.335903f,
                    0.336024f, 1.0f-0.671877f,
                    1.000004f, 1.0f-0.671847f,
                    0.999958f, 1.0f-0.336064f,
                    0.667979f, 1.0f-0.335851f,
                    0.668104f, 1.0f-0.000013f,
                    0.335973f, 1.0f-0.335903f,
                    0.667979f, 1.0f-0.335851f,
                    0.335973f, 1.0f-0.335903f,
                    0.668104f, 1.0f-0.000013f,
                    0.336098f, 1.0f-0.000071f,
                    0.000103f, 1.0f-0.336048f,
                    0.000004f, 1.0f-0.671870f,
                    0.336024f, 1.0f-0.671877f,
                    0.000103f, 1.0f-0.336048f,
                    0.336024f, 1.0f-0.671877f,
                    0.335973f, 1.0f-0.335903f,
                    0.667969f, 1.0f-0.671889f,
                    1.000004f, 1.0f-0.671847f,
                    0.667979f, 1.0f-0.335851f

                };

            m_mesh.Begin();

            for (var i = 0; i < 36; i++)
            {
                var index = i * 5;
                var vindex = i * 3;
                var uindex = i * 2 + (12 * 3 * 3);

                m_mesh.Position(g_all_buffer_data[vindex], g_all_buffer_data[vindex + 1], g_all_buffer_data[vindex + 2]);
                m_mesh.TextureCoord(g_all_buffer_data[uindex], g_all_buffer_data[uindex + 1]);

            }

            m_mesh.End(BufferUsageHint.StaticDraw);
        }

        public override void UnloadResources()
        {
            base.UnloadResources();

            if (m_font)
                m_font.Dispose();

            if (m_volume)
                m_volume.Dispose();

            m_chunks.Dispose();
            m_particles.Dispose();
            m_spriteRenderer.Dispose();
            m_mesh.Dispose();
            m_texture.Dispose();
            m_program.Dispose();
            m_spriteProgram.Dispose();
            m_mesh2.Dispose();
        }

        float x = 0;
        public override void Update(double dt)
        {
            rotation += dt;


            if (rotation > System.Math.PI * 2)
                rotation -= System.Math.PI * 2;

            m_camera.FieldOfView = 0.785398163f;
            m_camera.AspectRatio = 4.0f / 3.0f;
            m_camera.Near = 0.1f;
            m_camera.Far = 100f;
            m_camera.Position = new Vector3(x, 10, 3);
            m_camera.LookAt(new Vector3(16f, 0f, 16f));


            m_view.SetValue(m_camera.ViewMatrix);
            m_projection.SetValue(m_camera.ProjectionMatrix);
            m_viewProjection.SetValue(m_camera.ViewMatrix * m_camera.ProjectionMatrix);

            //m_particles.Update(dt);

            if (ProcessKeyboard((float)dt))
            {
                      }
            ProcessMouse();
        }

        private double rotation = 0;
        public override void Render(double dt)
        {
            //GenerateCube();
            // Enable depth test
            //GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.DepthTest);
            // Accept fragment if it closer to the camera than the former one
            GL.DepthFunc(DepthFunction.Less);

            GL.ClearColor(0.5f, 0.5f, 1f, 1f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);


            //m_mesh.Render(m_program);
            //m_volume.RenderOpaque(m_opaqueChunkProgram);
            //m_volume.RenderAlpha(m_opaqueChunkProgram);
            m_chunks.RenderOpaque(m_opaqueChunkProgram);
            m_chunks.renderAlpha(m_opaqueChunkProgram);

            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //var sprite1 = Sprite.Create(m_texture, -1, 1, 1, -1);
            //sprite1.SetColor(Color.White);

            //m_spriteRenderer.AddQuad(sprite1);
            m_texture.Bind();
            //m_spriteRenderer.DrawText(m_font, Vector2.Zero, "DDD");
            //m_spriteRenderer.Render(m_spriteProgram);

  

            //m_particles.Render(m_spriteProgram, m_texture, dt);
        }

        private bool c_down = false;
        private bool ProcessKeyboard(float delta)
        {
            var keyboard = OpenTK.Input.Keyboard.GetState();
            if (keyboard[OpenTK.Input.Key.Escape])
                m_engine.Stop("User pressed escape from main menu");

            if (keyboard[OpenTK.Input.Key.A])
                x -= (float)delta;

            if (keyboard[OpenTK.Input.Key.D])
                x += (float)delta;

            if (keyboard[OpenTK.Input.Key.C] )
            {
                if (!c_down)
                {
                    use_camera = !use_camera;
                    c_down = true;
                    return true;
                }
            }
            else 
                c_down = false;

            return false;
        }

        private void ProcessMouse()
        {
            var mouse = OpenTK.Input.Mouse.GetState();
        }
    }

}
