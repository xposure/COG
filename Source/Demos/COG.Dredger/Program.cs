using System;
using COG.Graphics;

namespace COG.Dredger
{
    public class MainMenu : GameState
    {
        private Texture2D m_texture;
        private Program m_program;

        private int vertexArrayID, vertexBuffer, uvBuffer, textureID;

        public override void LoadResources()
        {
            base.LoadResources();
            m_texture = m_engine.Assets.LoadAsset<Texture2D, TextureData2D>("dredger:texture:uvtemplate");
            m_program = m_engine.Assets.LoadAsset<Program, ProgramData>("dredger:program:simple");
        }

        public override void UnloadResources()
        {
            base.UnloadResources();

            m_texture.Dispose();
            m_program.Dispose();
        }

        public override void Update(double dt)
        {
            Console.WriteLine("update");
            ProcessKeyboard();
            ProcessMouse();
        }

        public override void Render(double dt)
        {
            Console.WriteLine("render");
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
