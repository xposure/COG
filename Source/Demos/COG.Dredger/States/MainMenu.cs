using System;
using COG.Dredger.Logic;
using COG.Dredger.Rendering;
using COG.Framework;
using COG.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;


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
        private Volume hover;
        private DynamicMesh m_mesh;
        private StreamMesh m_mesh2;
        private ParticleEmitter m_particles;
        private Volume m_volume;
        private VolumeGenerator m_vgen;
        private ChunkManager m_chunks;
        private DefaultCamera m_worldCamera;
        //private DefaultCamera m_guiCamera;
        private Map m_map = new Map(2);
        private bool use_camera = false;

        private Atma.Font m_font;

        public override void Initialize(Engine engine)
        {
            base.Initialize(engine);

            engine.GameWindow.CursorVisible = false;

            m_vgen = new VolumeGenerator();
            m_vgen.enableAO = true;
            m_vgen.enableGreedy = true;
            m_worldCamera = new DefaultCamera();
            m_map.Generate(new FlatGenerator());

            //m_map.Generate(new SimpleGenerator());
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


            vertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayID);

            var decl = new VertexDeclaration();
            decl.AddElement(3, VertexAttribPointerType.Float, VertexElementSemantic.Position);
            decl.AddElement(2, VertexAttribPointerType.Float, VertexElementSemantic.TexCoord0);

            m_mesh = new DynamicMesh(decl);
            m_mesh2 = new StreamMesh(decl);

            m_vgen.shape = Shape.DirtWall;
            m_vgen.width = 16;
            m_vgen.height = 16;
            m_vgen.depth = 16;
            m_vgen.scale = 0.45f;
            m_volume = m_vgen.GenerateVolume();

            m_chunks = new ChunkManager();
            m_chunks.Initialize(m_volume);


            hover = SurfaceExtractor.makeVoxels(0, 0, 0,
              new int[] { 0, 0, 0 },
              new int[] { 1, 1, 1 },
                  (i, j, k) =>
                  {
                      return 0xffffff;
                  }
              );

            hover.PrepareMesh();
            SurfaceExtractor.GenerateMesh(hover, centered: false);

            GenerateCube();

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1f);

            previous = current = Mouse.GetState();

            m_worldCamera.FieldOfView = 0.785398163f;
            m_worldCamera.AspectRatio = 4.0f / 3.0f;
            m_worldCamera.Near = 0.1f;
            m_worldCamera.Far = 1000f;
            m_worldCamera.Position = new Vector3(6, 10, 6);
            m_worldCamera.Position = new Vector3(0, 10, 0);
            //m_camera.LookAt(new Vector3(0, 0,0 ));
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



            for (var i = 0; i < 108; i++)
            {
                if (g_all_buffer_data[i] == -1f)
                    g_all_buffer_data[i] = 0;
            }

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

            if (hover)
                hover.Dispose();

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
            m_map.Dispose();
        }

        float x = 16;
        double counter;
        public override void Update(double dt)
        {
            counter += dt;
            rotation += dt;


            if (rotation > System.Math.PI * 2)
                rotation -= System.Math.PI * 2;


            //m_camera.LookAt(new Vector3(0, 0, 0));
            m_worldCamera.Update((float)dt);
            m_worldCamera.UpdateUniforms(m_engine.Programs);


            //m_particles.Update(dt);

            if (ProcessKeyboard((float)dt))
            {

            }
            ProcessMouse((float)dt);

        }

        private Vector3 start, end;
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

            //m_mesh2.Render(m_program);
            //m_volume.RenderOpaque(m_opaqueChunkProgram);
            //m_volume.RenderAlpha(m_opaqueChunkProgram);
            //m_chunks.RenderOpaque(m_opaqueChunkProgram);
            m_opaqueChunkProgram.SetUniformVec("tint", new Vector3(1, 1, 1));
            m_opaqueChunkProgram.SetUniformFloat("alpha", 1f);
            m_map.Render(m_opaqueChunkProgram);

            foreach (var p in GridRayTracer.raytrace(start.X, start.Y, start.Z, end.X, end.Y, end.Z))
            {
                //Console.WriteLine("{0} - {1}", (int)counter, p);

                var block = m_map.GetBlock((int)p.X, (int)p.Y, (int)p.Z);
                if (block.IsEmpty)
                    continue;


                m_opaqueChunkProgram.SetUniformVec("tint", new Vector3(1, 1, 0));
                m_opaqueChunkProgram.SetUniformFloat("alpha", 0.5f);
                GL.DepthFunc(DepthFunction.Lequal);
                GL.DepthMask(false);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


                var scale = 0.002f;
                var m = Matrix4.CreateScale(1 + scale) * Matrix4.CreateTranslation(p - Vector3.One * (scale / 2f));
                hover.RenderOpaque(m_opaqueChunkProgram, m);

                GL.DepthMask(true);
                GL.Disable(EnableCap.Blend);

                break;
            }


            //foreach (var p in GridRayTracer.raytrace(start.X, start.Y, start.Z, end.X, end.Y, end.Z))
            //    hover.RenderOpaque(m_opaqueChunkProgram, p);
            {
                GL.DepthMask(false);
                GL.DepthFunc(DepthFunction.Always);
                var scale = 0.5f;
                var m = Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(end - Vector3.One * (scale / 2f));

                m_opaqueChunkProgram.SetUniformVec("tint", new Vector3(1, 0, 0));
                m_opaqueChunkProgram.SetUniformFloat("alpha", 0.75f);
                hover.RenderOpaque(m_opaqueChunkProgram, m);
            }

            //var len = 100;
            //var start = m_camera.Position;
            //var dir = m_camera.Direction.Normalized();

            ////var m1 = m_camera.ProjectionMatrix.Inverted();
            ////var m2 = m_camera.ViewMatrix.Inverted();
            ////var mvp = m_camera.ViewMatrix * m_camera.ProjectionMatrix;
            ////var e = m_camera.Position + dir * len;5

            ////var end = Vector3.Transform(e, m2 * m1); 
            //var end = start + dir * len;

            //var ray = m_camera.CameraToRay(end);

            //end = ray.Direction * len + ray.Origin;
            ////Console.WriteLine("{0} - {1}", end, ray.Direction * len + ray.Origin);
            //Console.Clear();
            //for (var i = 0; i < 100; i++)
            //{
            //    var p = m_camera.Position + m_camera.Direction * i;
            //    p.X = (int)p.X;
            //    p.Y = (int)p.Y;
            //    p.Z = (int)p.Z;

            //    var block = m_map.GetBlock((int)p.X, (int)p.Y, (int)p.Z);
            //    if (block.IsEmpty)
            //        continue;

            //    hover.RenderOpaque(m_opaqueChunkProgram, p);
            //    break;
            //}


            //GL.Disable(EnableCap.DepthTest);

            //hover.RenderOpaque(m_opaqueChunkProgram, m_camera.Position + m_camera.Direction * 100 + new Vector3(-0.5f, -0.5f, -0.5f));

            //hover.RenderOpaque(m_opaqueChunkProgram, new Vector3(0, 0, 0));
            //hover.RenderOpaque(m_opaqueChunkProgram, new Vector3(0, 0, 31));
            //hover.RenderOpaque(m_opaqueChunkProgram, new Vector3(31, 0, 0));
            //hover.RenderOpaque(m_opaqueChunkProgram, new Vector3(31, 0, 31));



            //var m = Matrix4.CreateTranslation(Vector3.Zero) * Matrix4.CreateScale(1f);
            //m_opaqueChunkProgram.SetUniformMatrix4("model", m);
            //m_mesh.Render(m_opaqueChunkProgram);

            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //m_chunks.renderAlpha(m_opaqueChunkProgram);
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
            if (m_engine.GameWindow.Focused)
            {
                var keyboard = OpenTK.Input.Keyboard.GetState();
                if (keyboard[OpenTK.Input.Key.Escape])
                    m_engine.Stop("User pressed escape from main menu");

                if (keyboard[OpenTK.Input.Key.A])
                    x -= (float)delta;

                if (keyboard[OpenTK.Input.Key.D])
                    x += (float)delta;

                if (keyboard[Key.PageUp])
                {
                    m_vgen.scale += delta * 0.01f;
                    m_vgen.GenerateVolume();
                }
                else if (keyboard[Key.PageDown])
                {
                    m_vgen.scale -= delta * 0.01f;
                    m_vgen.GenerateVolume();
                }

                if (keyboard[OpenTK.Input.Key.C])
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
            }
            return false;
        }


        private bool m_hasFocus = false;
        private MouseState previous, current;
        private float yaw = 0;
        private float pitch = 0;

        private void SetMouseCenter()
        {
            var mx = m_engine.GameWindow.Width / 2 + m_engine.GameWindow.X;
            var my = m_engine.GameWindow.Height / 2 + m_engine.GameWindow.Y;
            Mouse.SetPosition(mx, my);
        }

        private void ProcessMouse(float delta)
        {
            if (m_engine.GameWindow.Focused)
            {
                if (!m_hasFocus)
                {
                    SetMouseCenter();
                    m_hasFocus = true;
                    return;
                }

                current = OpenTK.Input.Mouse.GetState();
                var dx = (previous.X - current.X) *  delta;
                var dy = (previous.Y - current.Y) * delta;

                //var pitch = (float)Math.Sin(m_camera.Direction.Y);

                yaw += dx;
                pitch += dy;

                pitch = Utility.Clamp(pitch, 1.5f, -1.5f);
                yaw = Utility.WrapAngle(yaw);

                var qyaw = Quaternion.FromAxisAngle(Vector3.UnitY, yaw);
                var qpitch = Quaternion.FromAxisAngle(Vector3.UnitX, pitch);

                var p = Vector3.Transform(-Vector3.UnitZ, qyaw * qpitch);
                p.Normalize();

                m_worldCamera.Direction = p;

                //Console.WriteLine("{0}:{1} - {2}:{3} - {4}:{5}", previous.X, previous.Y, current.X, current.Y, dx, dy);

                SetMouseCenter();

                previous = current;

                //if (current.LeftButton == ButtonState.Pressed)
                {
                    start = m_worldCamera.Position;
                    end = m_worldCamera.Position + m_worldCamera.Direction * 100;
                    //Console.Clear();
                }
                //Console.WriteLine("{0}:{1}:{2}", yaw, pitch, roll);

            }
            else
            {
                m_hasFocus = false;
            }
        }
    }



}
