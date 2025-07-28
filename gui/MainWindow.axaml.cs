using Avalonia.Controls;
using game_InfinityHex.engine;
using Interactive;

namespace game_InfinityHex.UI
{
    public partial class MainWindow : Window
    {
        public static MainWindow ProjectWindow { get; private set; } = null!;
        public MainWindow()
        {
            KeyboardListener.InitializeDefaultListener(this, new Core.DirectionManager());
#pragma warning disable CS8604 // Possible null reference argument, which is never thown because KeyboardListener.DefaultListener is initialized in InitializeDefaultListener.
            BackendManager.DefaultManager.AttatchKeyboardListener(KeyboardListener.DefaultListener);
#pragma warning restore CS8604
            Background = ThemeManager.DefaultManager.FetchBrush("Background_Color");

            MinWidth = 400;
            MinHeight = 400;
            Width = MinWidth;
            Height = MinHeight;

            Title = !string.IsNullOrEmpty(ProgramInfo.GameVersion?.ToString())
                ? $"{ProgramInfo.GameName} Version {ProgramInfo.GameVersion}"
                : ProgramInfo.GameName;

            // Initialize with LaunchPage
            Content = new LaunchPage();

            InitializeComponent();
            ProjectWindow = this;
        }
        public void ChangeControl(Control newControl)
        {
            Content = newControl;
        }
        public static void ToLaunchPage()
        {
            if (ProjectWindow.Content is LaunchPage) return; // Already on LaunchPage
            if (ProjectWindow.Content is GamePage gamePage)
            {
                gamePage.Cleanup(); // Cleanup the subscriptions used by the previous game page
            }
            ProjectWindow.ChangeControl(new LaunchPage());
        }
        public static void ToGamePage()
        {
            if (ProjectWindow.Content is GamePage gamePage)
            {
                gamePage.Cleanup(); // Cleanup the subscriptions used by the previous game page
            }
            ProjectWindow.ChangeControl(new GamePage());
            BackendManager.DefaultManager.Start();
        }
    }
}