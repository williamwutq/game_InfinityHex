using System;
using System.Threading;
using Engine;

namespace game_InfinityHex.engine
{
    /// <summary>
    /// The BackendManager class is responsible for managing the backend operations of the game,
    /// including starting and stopping the HexEngine, managing keyboard listeners, and handling game state.
    /// It runs the HexEngine in a separate thread and provides methods to attach keyboard listeners,
    /// start and stop the backend, and manage the game state.
    /// </summary>
    public class BackendManager
    {
        /// <summary>
        /// Default backend manager instance. This is used to manage the backend engine and its operations.
        /// </summary>
        public static BackendManager DefaultManager = new BackendManager();
        private volatile bool isRunning = false;
        private HexEngine engine;
        private Thread? runningThread;
        private Action? keyboardStart;
        private Action? keyboardStop;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackendManager"/> class with a new <see cref="HexEngine"/>.
        /// </summary>
        public BackendManager()
        {
            engine = new HexEngine();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BackendManager"/> class with a passed in <see cref="HexEngine"/>.
        /// </summary>
        /// <param name="engine">The <see cref="HexEngine"/> instance to manage.</param>
        public BackendManager(HexEngine engine)
        {
            this.engine = engine;
        }
        /// <summary>
        /// Attaches a keyboard listener to the backend manager.
        /// </summary>
        /// <param name="listener">The keyboard listener to attach.</param>
        /// <remarks>
        /// This method sets up the keyboard listener to handle escape events and manages the direction manager of the engine.
        /// If the listener is null, it does nothing.
        /// </remarks>
        public void AttatchKeyboardListener(Interactive.KeyboardListener listener)
        {
            if (listener != null)
            {
                listener.AttatchEscapeEventHandler(EndGame);
                keyboardStart = listener.Start;
                keyboardStop = listener.Stop;
                engine.SetDirectionManager(listener.GetDirectionManager());
            }
        }

        /// <summary>
        /// Starts the backend manager, initializing the engine and starting the keyboard listener if available.
        /// </summary>
        /// <remarks>
        /// This is the entry point for the backend manager to begin operations, and should be called by other components to start the backend.
        /// It will run the engine in a separate thread and invoke the keyboard listener's start method if it is set up.
        /// </remarks>
        public void Start()
        {
            Run();
            keyboardStart?.Invoke();
        }
        /// <summary>
        /// Stops the backend manager, halting the engine and invoking the keyboard listener's stop method if it is set up.
        /// </summary>
        /// <remarks>
        /// This method is used to gracefully stop the backend operations, ensuring that the engine is reset and the keyboard listener is stopped.
        /// It should be called when the application or game is closing or when the backend operations need to be halted.
        /// </remarks>
        public void Stop()
        {
            keyboardStop?.Invoke();
            Shutdown();
        }
        /// <summary>
        /// Sets up the running thread if it is not already running and starts the engine's move operation in a loop.
        /// </summary>
        /// <remarks>
        /// If the backend is not already running, this method will sets the running flag to true and starts a new background thread
        /// that will call the engine's move method every 40 milliseconds. To retrieve the engine instance, use the <see cref="Engine"/> method.
        /// </remarks>
        private void Run()
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
                    engine.Move();
                    Thread.Sleep(40);
                }
            });
            runningThread.IsBackground = true;
            runningThread.Start();
        }
        /// <summary>
        /// Shuts down the backend manager, stopping the running thread and resetting the engine.
        /// </summary>
        /// <remarks>
        /// Sets the running flag to false, checks if the running thread is alive, and if so, sets it to null.
        /// The false running flag will cause the running thread to automatically terminate. 
        /// It also resets the engine to its initial state.
        /// </remarks>
        private void Shutdown()
        {
            isRunning = false;
            if (runningThread != null && runningThread.IsAlive)
            {
                runningThread = null;
            }
            engine.ResetEngine();
        }
        /// <summary>
        /// Gets the current instance of the <see cref="HexEngine"/> managed by this backend manager.
        /// </summary>
        /// <returns>The current <see cref="HexEngine"/> reference.</returns>
        public HexEngine Engine()
        {
            return engine;
        }
        /// <summary>
        /// Ends the game by shutting down the backend manager and returning to the launch page.
        /// </summary>
        private void EndGame()
        {
            Shutdown();
            UI.MainWindow.ToLaunchPage();
        }
    }
}