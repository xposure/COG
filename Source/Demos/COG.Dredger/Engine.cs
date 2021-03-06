﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using COG.Assets;
using COG.Framework;
using COG.Graphics;
using OpenTK;
using OpenTK.Graphics;

namespace COG.Dredger
{
    //reference link http://www.javased.com/?source_dir=TeraSpout/src/main/java/org/terasology/game/modes/StateMainMenu.java
    //https://github.com/RexMorgan/Heliocentricity/tree/master/src/Heliocentricity.Common

    public abstract class GameState
    {
        protected Engine m_engine;

        public virtual void LoadResources() { }
        public virtual void UnloadResources() { }
        public virtual void Exit() { }
        public virtual void Initialize(Engine engine)
        {
            m_engine = engine;
        }

        public abstract void Update(double dt);
        public abstract void Render(double dt);
    }

    public class Engine : Registry
    {
        private static Engine g_engine;
        public static Engine Instance { get { return g_engine; } }

        public Engine()
        {
            g_engine = this;
        }

        private Config m_config;
        private GameWindow m_gameWindow;
        private AssetManager m_assets;
        private ProgramManager m_programs;
        private Stack<GameState> m_states;
        private List<Action> m_pendingStateChanges;

        private bool m_isRunning = false;

        public bool IsRunning { get { return m_isRunning; } }
        public ProgramManager Programs { get { return m_programs; } }
        public AssetManager Assets { get { return m_assets; } }
        public RegistryManager Registry { get { return m_registry; } }
        public GameWindow GameWindow { get { return m_gameWindow; } }
        public GameState CurrentState
        {
            get
            {
                if (m_states.Count == 0)
                    return null;
                return m_states.Peek();
            }
        }

        public void Run(GameState initialState)
        {
            m_isRunning = true;
            Initialize();

            ChangeState(initialState);

            m_gameWindow.Visible = true;

            ProcessStateChanges();


            var sw = Stopwatch.StartNew();
            var lastFrame = 0.0;
            //var skipped = false;
            while (m_isRunning && !m_gameWindow.IsExiting)
            {
                var current = sw.Elapsed.TotalSeconds;
                var dt = current - lastFrame;
                //if( dt < )
                //dt = 1f / 60f;

                Update(dt);

                if (m_gameWindow.IsExiting)
                    break;

                //if (skipped || dt < 0.032)
                //{
                    Render(dt);
                //    skipped = false;
                //}
                //else
                //{
                //    skipped = true;
                //}

                lastFrame = current;
                System.Threading.Thread.Sleep(1);
            }

            m_isRunning = false;

            Cleanup();
        }

        public void Stop(string message)
        {
            m_isRunning = false;
        }

        public void PushState(GameState state)
        {
            m_pendingStateChanges.Add(() =>
            {
                m_states.Push(state);
                state.Initialize(this);
            });
        }

        public void ChangeState(GameState state)
        {
            m_pendingStateChanges.Add(() =>
            {
                PurgeStates();

                m_states.Push(state);
                state.Initialize(this);
                state.LoadResources();
            });
        }

        public void PopState()
        {
            m_pendingStateChanges.Add(() =>
            {
                var state = m_states.Pop();
                state.Exit();
            });
        }

        private void PurgeStates()
        {
            if (m_states.Count > 0)
            {
                var current = m_states.Pop();
                current.UnloadResources();
                current.Exit();

                while (m_states.Count > 0)
                {
                    current = m_states.Pop();
                    //resources we're already unloaded
                    current.Exit();
                }
            }
        }

        private void Update(double dt)
        {
            var state = CurrentState;
            if (state == null)
            {
                m_isRunning = false;
                return;
            }

            state.Update(dt);

            m_gameWindow.ProcessEvents();
        }

        private void Render(double dt)
        {
            var state = CurrentState;
            if (state == null)
            {
                m_isRunning = false;
                return;
            }

            state.Render(dt);

            // Swap buffers
            m_gameWindow.SwapBuffers();

            ProcessStateChanges();
        }

        private void ProcessStateChanges()
        {
            if (m_pendingStateChanges.Count > 0)
            {
                foreach (var stateUpdate in m_pendingStateChanges)
                    stateUpdate();

                m_pendingStateChanges.Clear();
            }
        }

        private void Initialize()
        {
            m_states = new Stack<GameState>();
            m_pendingStateChanges = new List<Action>();

            //setup registry
            var m_registry = new RegistryManager();

            //setup config
            m_config = new Config();
            m_config.Load();
            m_registry.Add(m_config.Module.CreateUri("config"), m_config);

            //add engine to the registry
            m_registry.Add(m_config.Module.CreateUri("engine"), this);

            InitializeManagers();

            InitializeRenderer();
        }

        private void InitializeManagers()
        {
            m_assets = new AssetManager();
            m_assets.InitializeGraphics(m_config.Module.Name);
            m_assets.InitializeFonts();

            m_programs = m_assets.InitializePrograms(m_config.Module.Name);


            m_assets.AddAssetSource(new DirectorySource(m_config.Module.Name, "content"));
        }

        private void InitializeRenderer()
        {
            InitializeOpenTK();
            InitializeDisplay();
        }

        private void InitializeDisplay()
        {
            m_gameWindow = new GameWindow(m_config.WindowWidth, m_config.WindowHeight,
                    GraphicsMode.Default,
                    m_config.Module.Name,
                    GameWindowFlags.Default,
                    DisplayDevice.Default,
                    3, 3,
                    GraphicsContextFlags.Default
                );
            m_gameWindow.VSync = VSyncMode.Off;
        }

        private void InitializeOpenTK()
        {
            OpenTK.Toolkit.Init();
        }

        private void Cleanup()
        {
            PurgeStates();

            m_programs.Dispose();
            m_assets.Dispose();

            m_gameWindow.Dispose();
            m_gameWindow = null;
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            m_registry.Dispose();
            m_registry = null;

            m_config = null; //registry will dispose object
        }
    }
}
