using System;
using Avalonia.Controls;

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
        private static bool InConsole = !Console.IsInputRedirected && Environment.UserInteractive;
        private readonly Window? window;
        private readonly Core.DirectionManager directionManager;
        private readonly System.Threading.CancellationTokenSource cts = new();
        /// <summary>
        /// Suppresses console input by redirecting it to an empty string reader.
        /// This is useful when the application is running in a GUI context and console input should not interfere with the UI.
        /// </summary>
        public static void SuppressConsole()
        {
            if (InConsole)
            {
                // Disable console input
                Console.SetIn(new System.IO.StringReader(string.Empty));
                InConsole = false;
            }
        }
        /// <summary>
        /// Creates a new keyboard direction listener that processes key input on a background thread.
        /// </summary>
        /// <param name="manager">The shared DirectionManager to act upon.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="manager"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the listener is not running in a console environment.</exception>
        public KeyboardListener(Core.DirectionManager manager)
        {
            directionManager = manager ?? throw new ArgumentNullException("Input DirectionManager must be initialized and not null");
            if (!InConsole) throw new InvalidOperationException("KeyboardListener is designed to run in a console environment. Please run the application in a console to use this feature.");
            window = null;
        }
        /// <summary>
        /// Creates a new keyboard direction listener that processes key input on a background thread.
        /// </summary>
        /// <param name="window">The main window to interact with, if needed.</param>
        /// <param name="manager">The shared DirectionManager to act upon.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="window"/> or <paramref name="manager"/> is null.</exception>
        public KeyboardListener(Window window, Core.DirectionManager manager)
        {
            directionManager = manager ?? throw new ArgumentNullException("Input DirectionManager must be initialized and not null");
            this.window = window ?? throw new ArgumentNullException("Window must be initialized and not null");
            AttachAvaloniaKeyHandlers();
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
        /// <summary>
        /// Attaches Avalonia key event handlers to the window if window is not null and not in console.
        /// This allows the listener to respond to key events in the Avalonia UI context.
        /// </summary>
        public void AttachAvaloniaKeyHandlers()
        {
            if (window != null && !InConsole)
            {
                window.KeyDown += OnKeyDown;
            }
        }

        private void OnKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                // Decrement keys: ←, A, Q, U, J
                case Avalonia.Input.Key.Left:
                case Avalonia.Input.Key.A:
                case Avalonia.Input.Key.Q:
                case Avalonia.Input.Key.U:
                case Avalonia.Input.Key.J:
                    directionManager.Move(false);
                    break;

                // Increment keys: →, D, E, O, L
                case Avalonia.Input.Key.Right:
                case Avalonia.Input.Key.D:
                case Avalonia.Input.Key.E:
                case Avalonia.Input.Key.O:
                case Avalonia.Input.Key.L:
                    directionManager.Move(true);
                    break;

                // No-op keys: ↑, W, I
                case Avalonia.Input.Key.Up:
                case Avalonia.Input.Key.W:
                case Avalonia.Input.Key.I:
                    directionManager.Noop();
                    break;

                // Exit key: Esc
                case Avalonia.Input.Key.Escape:
                    Console.WriteLine("Esc Pressed.");
                    Stop();
                    break;
            }
        }
        private void ListenLoop(System.Threading.CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (InConsole)
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
                            directionManager.Move(false);
                            break;

                        // Increment keys: →, D, E, O, L
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                        case ConsoleKey.E:
                        case ConsoleKey.O:
                        case ConsoleKey.L:
                            directionManager.Move(true);
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
            }
        }
    }
}