using Avalonia;
using Avalonia.Controls;
using Engine;
using game_InfinityHex.engine;
using Interactive;

namespace game_InfinityHex.UI
{
    public partial class MainWindow : Window
    {
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

            // Initialize with LaunchPanel
            var mainControl = new HexagonGrid(BackendManager.DefaultManager.Engine(), ThemeManager.DefaultManager);
            Content = mainControl;
            LayoutUpdated += (sender, e) =>
            {
                if (Bounds.Width > 0 && Bounds.Height > 0)
                {
                    mainControl.UpdateLayout(Bounds.Width, Bounds.Height);
                }
            };

            InitializeComponent();
            BackendManager.DefaultManager.Run();
        }
        public void ChangeControl(Control newControl)
        {
            Content = newControl;
        }
    }
}