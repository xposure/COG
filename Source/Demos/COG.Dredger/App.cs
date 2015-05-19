using System;
using COG.Dredger.States;

namespace COG.Dredger
{
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
