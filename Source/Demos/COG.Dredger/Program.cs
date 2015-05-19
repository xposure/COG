using System;
using COG.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace COG.Dredger
{
    public class MainMenu : GameState
    {
        private Texture2D m_texture;
        private Program m_program;

        private int vertexArrayID, vertexBuffer, uvBuffer;

        public override void LoadResources()
        {
            base.LoadResources();
            m_texture = m_engine.Assets.LoadAsset<Texture2D, TextureData2D>("dredger:texture:uvtemplate");
            m_program = m_engine.Assets.LoadAsset<Program, ProgramData>("dredger:program:simple");


            vertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayID);

            // Our vertices. Tree consecutive floats give a 3D vertex; Three consecutive vertices give a triangle.
            // A cube has 6 faces with 2 triangles each, so this makes 6*2=12 triangles, and 12*3 vertices
            var g_vertex_buffer_data = new[] {
                    -1.0f,-1.0f,-1.0f, // triangle 1 : begin
                    -1.0f,-1.0f, 1.0f,
                    -1.0f, 1.0f, 1.0f, // triangle 1 : end
                    1.0f, 1.0f,-1.0f, // triangle 2 : begin
                    -1.0f,-1.0f,-1.0f,
                    -1.0f, 1.0f,-1.0f, // triangle 2 : end
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
                    1.0f,-1.0f, 1.0f
                };


            // This will identify our vertex buffer
            // Generate 1 buffer, put the resulting identifier in vertexbuffer
           vertexBuffer = GL.GenBuffer();

            // The following commands will talk about our 'vertexbuffer' buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

            // Compute the size of our array
            var vertexBufferSize = new IntPtr(sizeof(float) * g_vertex_buffer_data.Length);

            // Give our vertices to OpenGL
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBufferSize, g_vertex_buffer_data,
                            BufferUsageHint.StaticDraw);

            // Two UV coordinatesfor each vertex. They were created with Blender. You'll learn shortly how to do this yourself.
            var g_uv_buffer_data = new[] {
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

            uvBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, uvBuffer);

            var uvBufferSize = new IntPtr(sizeof(float) * g_uv_buffer_data.Length);
            GL.BufferData(BufferTarget.ArrayBuffer, uvBufferSize, g_uv_buffer_data, BufferUsageHint.StaticDraw);


            GL.ClearColor(1f, 1f, 1f, 1f);
        }

        public override void UnloadResources()
        {
            base.UnloadResources();

            m_texture.Dispose();
            m_program.Dispose();
        }

        public override void Update(double dt)
        {
            rotation += dt;
            ProcessKeyboard();
            ProcessMouse();
        }

        private double rotation = 0;
        public override void Render(double dt)
        {
            // Enable depth test
            GL.Enable(EnableCap.DepthTest);
            // Accept fragment if it closer to the camera than the former one
            GL.DepthFunc(DepthFunction.Less);
            //Console.WriteLine("render");

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
            // Our ModelViewProjection : multiplication of our 3 matrices
            var MVP = Projection * View * Model;
            MVP = Model * View * Projection;

            // Get a handle for our "MVP" uniform.
            // Only at initialisation time.
            var MatrixID = GL.GetUniformLocation(m_program.ProgramID, "MVP");

            // Send our transformation to the currently bound shader,
            // in the "MVP" uniform
            // For each model you render, since the MVP will be different (at least the M part)
            //GL.UseProgram(programID);
            m_program.Bind();
            GL.UniformMatrix4(MatrixID, false, ref MVP);

            m_program.Bind();
            m_texture.Bind();

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(
                0,                             // attribute 0. No particular reason for 0, 
                // but must match the layout in the shader.
                3,                             // size
                VertexAttribPointerType.Float, // type
                false,                         // normalized?
                0,                             // stride
                0                              // array buffer offset
            );


            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, uvBuffer);
            GL.VertexAttribPointer(
                1,                              // attribute index
                2,                              // size
                VertexAttribPointerType.Float,  // type
                false,                          // normalized?
                0,                              // stride
                0                               // offset
            );

            // Draw the triangle !
            GL.DrawArrays(PrimitiveType.Triangles, 0, 12 * 3); // 12*3 indices starting at 0 -> 12 triangles -> 6 squares

            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(0);

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

    class App
    {
        [STAThread]
        static void Main(string[] args)
        {
            //new ConsoleListener();
            using (var engine = new Engine())
            {
                engine.Run(new MainMenu());
            }
        }
    }
}
