using System;
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
            public float color;
            public float speed;
            public Vector2 size;
            public Vector3 position;
            public Vector3 direction;
        }

        private Random rnd = new Random();
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
                        particle.speed = rnd.Next(50, 100) / 100f;
                        particle.color = rnd.Next(500, 1000) / 1000f;
                        particle.speed = (float)rnd.NextDouble() * 5;
                        particle.direction = new Vector3(rnd.Next(-1000, 1000), rnd.Next(-1000, 1000), rnd.Next(-1000, 1000));
                        particle.position = new Vector3(0, 0, 0);
                        //particle.position = new Vector2(rnd.Next(-1000, 1000), rnd.Next(-1000, 1000));
                        particle.size = new Vector2(rnd.Next(500, 1000), rnd.Next(500, 1000));

                        particle.direction.Normalize();
                        //particle.position.Normalize();
                        particle.size.Normalize();
                        particle.size *= 0.15f;

                        m_particles[m_particleIndex] = particle;
                        m_particleIndex++;
                    }
                }

            }

            var fdt = (float)dt;
            for (var i = 0; i < m_particleIndex; i++)
            {
                m_particles[i].life -= fdt ;

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

        public void Render(Texture2D texture, double dt)
        {
            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.Disable(EnableCap.DepthTest);
            for (var i = 0; i < m_particleIndex; i++)
            {
                var p = m_particles[i];
                var sprite = Sprite.Create(texture, p.position.X, p.position.Y, p.position.X + p.size.X, p.position.Y - p.size.Y);
                sprite.SetDepth(p.position.Z);
                sprite.SetColor(new Color(p.life, 0.5f, 0.5f, 0.5f));

                m_renderer.AddQuad(sprite);
            }
            m_renderer.Render();
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
        private Program m_spriteProgram;

        private int vertexArrayID;
        private DynamicMesh m_mesh;
        private StreamMesh m_mesh2;
        private ParticleEmitter m_particles;

        public override void LoadResources()
        {
            base.LoadResources();
            m_spriteRenderer = new SpriteRenderer();
            m_particles = new ParticleEmitter();

            m_texture = m_engine.Assets.LoadTexture("dredger:texture:uvtemplate");
            m_program = m_engine.Assets.LoadProgram("dredger:program:simple");
            m_spriteProgram = m_engine.Assets.LoadProgram("dredger:program:sprite");

            vertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayID);



            var decl = new VertexDeclaration();
            decl.AddElement(3, VertexAttribPointerType.Float, VertexElementSemantic.Position);
            decl.AddElement(2, VertexAttribPointerType.Float, VertexElementSemantic.TexCoord0);

            m_mesh = new DynamicMesh(decl);
            m_mesh2 = new StreamMesh(decl);

            GenerateCube();

            GL.ClearColor(1f, 1f, 1f, 1f);
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

            m_particles.Dispose();
            m_spriteRenderer.Dispose();
            m_mesh.Dispose();
            m_texture.Dispose();
            m_program.Dispose();
            m_spriteProgram.Dispose();
            m_mesh2.Dispose();
        }

        public override void Update(double dt)
        {
            rotation += dt;

            if (rotation > System.Math.PI * 2)
                rotation -= System.Math.PI * 2;

            m_particles.Update(dt);

            ProcessKeyboard();
            ProcessMouse();
        }

        private double rotation = 0;
        public override void Render(double dt)
        {
            //GenerateCube();
            // Enable depth test
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            // Accept fragment if it closer to the camera than the former one
            GL.DepthFunc(DepthFunction.Less);

            //GL.ClearColor(0,0,0,0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            // Projection matrix : 45° Field of View, 4:3 ratio, display range : 0.1 unit <-> 100 units
            var Projection = Matrix4.CreatePerspectiveFieldOfView(0.785398163f, 4.0f / 3.0f, 0.1f, 100.0f);
            // Camera matrix
            var View = Matrix4.LookAt(
                    new Vector3(4, 3, 3), // Camera is at (4,3,3), in world space
                    new Vector3(0, 0, 0), // and looks at the origin
                    new Vector3(0, 1, 0) // head is up (set to 0,-1,0 to look upside-down
                );
            // Model matrix : an identity matrix (model will be at the origin)
            var Model = Matrix4.CreateRotationY((float)rotation);
            //var Model = Matrix4.Identity;
            // Our ModelViewProjection : multiplication of our 3 matrices

            var MVP = Projection * View * Model;
            MVP = Model * View * Projection;



            //Console.WriteLine(Projection);
            //Console.WriteLine(Projection2);
            //Console.WriteLine();
            //Console.WriteLine(View * Model);
            //Console.WriteLine(View2 * Model2);
            //Console.WriteLine();
            //Console.WriteLine(MVP);
            //Console.WriteLine(MVP2);
            //MVP2 = MVP2.Transpose();
            // Get a handle for our "MVP" uniform.
            // Only at initialisation time.
            //var MatrixID = GL.GetUniformLocation(m_program.ProgramID, "MVP");


            // Send our transformation to the currently bound shader,
            // in the "MVP" uniform
            // For each model you render, since the MVP will be different (at least the M part)
            //GL.UseProgram(programID);
            m_program.Bind();
            m_program.SetUniformMatrix4("MVP", MVP);
            //GL.UniformMatrix4(MatrixID, false, ref MVP);
            //GL.Uniform1()

            //m_program.Bind();
            m_texture.Bind();

            //m_mesh.Render();

            //MVP = View * Projection;
            //GL.UniformMatrix4(MatrixID, false, ref MVP);
            //MatrixID = GL.GetUniformLocation(m_spriteProgram.ProgramID, "MVP");
            m_spriteProgram.Bind();
            m_spriteProgram.SetUniformMatrix4("MVP", MVP);

            var sprite1 = Sprite.Create(m_texture, -1, 1, 1, -1);
            sprite1.SetColor(Color.White);

            //m_spriteRenderer.AddQuad(sprite1);
            //m_spriteRenderer.Render();

            m_particles.Render(m_texture, dt);

        }

        private void ProcessKeyboard()
        {
            var keyboard = OpenTK.Input.Keyboard.GetState();
            if (keyboard[OpenTK.Input.Key.Escape])
                m_engine.Stop("User pressed escape from main menu");
        }

        private void ProcessMouse()
        {
            var mouse = OpenTK.Input.Mouse.GetState();
        }
    }

}
