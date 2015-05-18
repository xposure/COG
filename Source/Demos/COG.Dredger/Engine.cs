using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using COG.Framework;
using OpenTK;
using OpenTK.Graphics;

namespace COG.Dredger
{
    public abstract class GameState
    {
        public abstract void Initialize(Engine engine);
        public abstract void Unload();
        public abstract void Update(double dt);
        public abstract void Render(double dt);
    }

    public class Engine : Registry
    {
        private Config m_config;
        private GameWindow m_gameWindow;
        private Stack<GameState> m_states;
        private List<Action> m_pendingStateChanges;

        private bool m_isRunning = false;

        public bool IsRunning { get { return m_isRunning; } }
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

            m_states.Push(initialState);
            m_gameWindow.Visible = true;

            var sw = Stopwatch.StartNew();
            var lastFrame = 0.0;
            var skipped = false;
            while (m_isRunning && !m_gameWindow.IsExiting)
            {
                var current = sw.Elapsed.TotalSeconds;
                var dt = current - lastFrame;

                Update(dt);

                if (m_gameWindow.IsExiting)
                    break;

                if (skipped || dt > 0.032)
                {
                    Render(dt);
                    skipped = false;
                }
                else
                {
                    skipped = true;
                }

                lastFrame = current;
            }

            m_isRunning = false;

            Shutdown();
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
                while (m_states.Count > 0)
                {
                    var current = m_states.Pop();
                    current.Unload();
                }
                
                m_states.Push(state);
                state.Initialize(this);
            });
        }

        public void PopState()
        {
            m_pendingStateChanges.Add(() =>
            {
                var state = m_states.Pop();
                state.Unload();
            });
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
            m_registry.Add(m_config.Module.CreateUri("config"), m_config);

            //add engine to the registry
            m_registry.Add(m_config.Module.CreateUri("engine"), this);

            InitializeRenderer();
        }

        private void InitializeRenderer()
        {
            InitializeOpenTK();
            InitializeDisplay();
        }

        private void InitializeDisplay()
        {
            m_gameWindow = new GameWindow(1024, 768,
                    GraphicsMode.Default,
                    m_config.Module.Name,
                    GameWindowFlags.Default,
                    DisplayDevice.Default,
                    3, 3,
                    GraphicsContextFlags.Default
                );
        }

        private void InitializeOpenTK()
        {
            OpenTK.Toolkit.Init();
        }

        private void Shutdown()
        {
            //uninit resources first

            m_gameWindow.Dispose();
            m_gameWindow = null;
        }

        private void ChangeState(GameState newState)
        {

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
