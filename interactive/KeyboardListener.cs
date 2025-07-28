using System;
using Avalonia.Controls;

namespace Interactive
{
    /// <summary>
    /// A background listener that reads keyboard input and issues directional commands
    /// to a shared <see cref="DirectionManager"/> instance.
    /// <para>
    /// This class is designed to run on a separate thread and interpret specific keys
    /// as directional rotation inputs or no-operations. It supports common arrow keys
    /// as well as alternative keys for ergonomic or layout flexibility.
    /// </para>
    /// Mapped keys
    /// <list type="bullet">
    /// <item>Left/Decrement: ←, A, Q, U, J</item>
    /// <item>Right/Increment: →, D, E, O, L</item>
    /// <item>Noop: ↑, W, I</item>
    /// <item>Escape: Quit the game, or custom action implemented with <see cref="EscapeEventHandler"/></item>
    /// </list>
    /// </summary>
    public class KeyboardListener
    {
        private static bool InConsole = !Console.IsInputRedirected && Environment.UserInteractive;
        public static KeyboardListener? DefaultListener { get; private set; }
        private readonly Window? window;
        private readonly Core.DirectionManager directionManager;
        private readonly System.Threading.CancellationTokenSource cts = new();
        /// <summary>
        /// Initializes the default keyboard listener with the provided window and direction manager. This is only used for GUI applications.
        /// If a default listener already exists, it will not be re-initialized.
        /// </summary>
        /// <param name="window">The main window to interact with, if needed.</param>
        /// <param name="directionManager">The shared DirectionManager to act upon.</param>
        public static void InitializeDefaultListener(Window window, Core.DirectionManager directionManager)
        {
            if (DefaultListener == null)
            {
                DefaultListener = new KeyboardListener(window, directionManager);
                DefaultListener.Start();
            }
        }
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

        /// <summary>
        /// Returns the current DirectionManager instance. This is guaranteed to be non-null.
        /// </summary>
        /// <returns>The current DirectionManager instance used for this listener.</returns>
        public Core.DirectionManager GetDirectionManager()
        {
            return directionManager;
        }
        
        /// <summary>
        /// Handles the event of pressing the Escape key.
        /// </summary>
        /// <remarks>
        /// This method is used to trigger an escape event, which can be used to exit the game or perform other actions.
        /// It is designed to be attached to an event handler that responds to the Escape key being pressed.
        /// </remarks>
        public delegate void EscapeEventHandler();
        /// <summary>
        /// The internal event handler for the Escape key.
        /// This handler is invoked when the Escape key is pressed, allowing for custom actions to be performed.
        /// </summary>
        private EscapeEventHandler? escapeEventHandler;
        /// <summary>
        /// Attaches an event handler for the Escape key.
        /// This allows the listener to respond to Escape key presses with a custom action.
        /// </summary>
        /// <param name="handler">The event handler to attach for Escape key presses.</param>
        public void AttatchEscapeEventHandler(EscapeEventHandler handler) {
            escapeEventHandler = handler;
        }
        /// <summary>
        /// Detaches the Escape key event handler.
        /// This stops the listener from responding to Escape key presses.
        /// </summary>
        public void DetachEscapeEventHandler()
        {
            escapeEventHandler = null;
        }

        /// <summary>
        /// Called when a key is pressed in the Avalonia window.
        /// It interprets the key and issues commands to the DirectionManager.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The key event arguments containing the pressed key.</param>
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
                    escapeEventHandler?.Invoke();
                    break;
            }
        }
        /// <summary>
        /// The main loop that listens for key inputs in a console environment.
        /// It processes key inputs and issues commands to the DirectionManager.
        /// </summary>
        /// <param name="token">A cancellation token to stop the loop when requested.</param>
        /// <remarks>
        /// This method runs in a loop until the cancellation token is requested only if the listener is in a console environment.
        /// It checks for key availability in the console and processes the key inputs accordingly.
        /// </remarks>
        /// <exception cref="ThreadInterruptedException">Thrown if the thread is interrupted while sleeping.</exception>
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
                            escapeEventHandler?.Invoke();
                            break;
                    }
                }
                else break;
            }
        }
    }
}