using System;

namespace Interactive
{
    /// <summary>
    /// A background listener that reads keyboard input and issues directional commands
    /// to a shared <see cref="DirectionManager"/> instance.
    /// 
    /// This class is designed to run on a separate thread and interpret specific keys
    /// as directional rotation inputs or no-operations. It supports common arrow keys
    /// as well as alternative keys for ergonomic or layout flexibility.
    ///
    /// Mapped keys:
    /// - Left/Decrement: ←, A, Q, U, J
    /// - Right/Increment: →, D, E, O, L
    /// - Noop: ↑, W, I
    /// - Escape: Stops the listener
    /// </summary>
    public class KeyboardListener
    {
        private readonly Core.DirectionManager directionManager;
        private readonly System.Threading.CancellationTokenSource cts = new();
        /// <summary>
        /// Creates a new keyboard direction listener that processes key input on a background thread.
        /// </summary>
        /// <param name="manager">The shared DirectionManager to act upon.</param>
        public KeyboardListener(Core.DirectionManager manager)
        {
            directionManager = manager ?? throw new ArgumentNullException("Input DirectionManager must be initialized and not null");
        }
        /// <summary>
        /// Starts listening to keyboard inputs on a background thread.
        /// </summary>
        public void Start()
        {
            System.Threading.Tasks.Task.Run(() => ListenLoop(cts.Token), cts.Token);
        }
        /// <summary>
        /// Stops the background keyboard listener.
        /// </summary>
        public void Stop()
        {
            cts.Cancel();
        }
        private void ListenLoop(System.Threading.CancellationToken token)
        {
            Console.WriteLine("Keyboard listener started. Press Esc to exit.");
            while (!token.IsCancellationRequested)
            {
                if (!Console.KeyAvailable)
                {
                    System.Threading.Thread.Sleep(2);
                    continue;
                }

                var key = Console.ReadKey(intercept: true).Key;

                switch (key)
                {
                    // Decrement keys: ←, A, Q, U, J
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                    case ConsoleKey.Q:
                    case ConsoleKey.U:
                    case ConsoleKey.J:
                        directionManager.Decrement();
                        Console.WriteLine("← (Decrement) → " + directionManager.GetOffsetIndex());
                        break;

                    // Increment keys: →, D, E, O, L
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                    case ConsoleKey.E:
                    case ConsoleKey.O:
                    case ConsoleKey.L:
                        directionManager.Increment();
                        break;

                    // No-op keys: ↑, W, I
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                    case ConsoleKey.I:
                        directionManager.Noop();
                        break;

                    // Exit key: Esc
                    case ConsoleKey.Escape:
                        Console.WriteLine("Esc Pressed.");
                        Stop();
                        break;
                }
            }
            Console.WriteLine("Keyboard listener stopped.");
        }
    }
}