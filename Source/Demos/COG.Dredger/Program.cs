using System;

namespace COG.Dredger
{
    public class MainMenu : GameState
    {
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
