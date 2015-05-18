﻿using System;

namespace COG.Dredger
{
    public class MainMenu : GameState
    {
        public override void Update(double dt)
        {
            Console.WriteLine("update");
            ProcessKeyboard();
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
