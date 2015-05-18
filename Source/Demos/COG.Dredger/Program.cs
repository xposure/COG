using System;

namespace COG.Dredger
{
    public class MainMenu : GameState
    {
        private Engine m_engine;

        public override void Initialize(Engine engine)
        {
            m_engine = engine;
        }

        public override void Unload()
        {
        }

        public override void Update(double dt)
        {
            Console.WriteLine("update");
        }

        public override void Render(double dt)
        {
            Console.WriteLine("render");
        }
    }

    class Program
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
