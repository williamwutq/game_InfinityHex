using System;
using System.Threading;
using Engine;

namespace game_InfinityHex.engine
{
    public class BackendManager
    {
        public static BackendManager DefaultManager = new BackendManager();
        private volatile bool isRunning = false;
        private HexEngine engine;
        private Thread? runningThread;
        private Action? keyboardStart;
        private Action? keyboardStop;
        public BackendManager()
        {
            engine = new HexEngine();
        }
        public BackendManager(HexEngine engine)
        {
            this.engine = engine;
        }
        public void AttatchKeyboardListener(Interactive.KeyboardListener listener)
        {
            if (listener != null)
            {
                listener.AttatchEscapeEventHandler(ShutDown);
                keyboardStart = listener.Start;
                keyboardStop = listener.Stop;
                engine.SetDirectionManager(listener.GetDirectionManager());
            }
        }

        public void Start()
        {
            Run();
            keyboardStart?.Invoke();
        }
        public void Stop()
        {
            keyboardStop?.Invoke();
            ShutDown();
        }
        public void Run()
        {
            if (runningThread != null && runningThread.IsAlive || isRunning)
            {
                return; // Already running
            }
            isRunning = true;
            runningThread = new Thread(() =>
            {
                while (isRunning)
                {
                    Thread.Sleep(40);
                    engine.Move();
                }
            });
            runningThread.IsBackground = true;
            runningThread.Start();
        }
        public void ShutDown()
        {
            isRunning = false;
            if (runningThread != null && runningThread.IsAlive)
            {
                runningThread = null;
            }
            engine.ResetEngine();
        }
        public HexEngine Engine()
        {
            return engine;
        }
    }
}