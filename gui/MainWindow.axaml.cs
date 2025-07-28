using Avalonia;
using Avalonia.Controls;
using Engine;
using Interactive;

namespace game_InfinityHex.UI
{
    public partial class MainWindow : Window
    {
        private HexagonGrid mainControl;

        public MainWindow()
        {
            KeyboardListener.InitializeDefaultListener(this, new Core.DirectionManager());
            Background = ThemeManager.DefaultManager.FetchBrush("Background_Color");

            MinWidth = 400;
            MinHeight = 400;
            Width = MinWidth;
            Height = MinHeight;

            Title = !string.IsNullOrEmpty(ProgramInfo.GameVersion?.ToString())
                ? $"{ProgramInfo.GameName} Version {ProgramInfo.GameVersion}"
                : ProgramInfo.GameName;

            // Initialize with LaunchPanel
            HexEngine hexEngine = new HexEngine();
            KeyboardListener.DefaultListener?.AttatchEscapeEventHandler(hexEngine.ResetEngine);
            KeyboardListener.DefaultListener?.Start();
            hexEngine.SetDirectionManager(KeyboardListener.DefaultListener?.GetDirectionManager());
            mainControl = new HexagonGrid(hexEngine, ThemeManager.DefaultManager);
            Content = mainControl;
            LayoutUpdated += (sender, e) =>
            {
                if (Bounds.Width > 0 && Bounds.Height > 0)
                {
                    mainControl.UpdateLayout(Bounds.Width, Bounds.Height);
                }
            };

            InitializeComponent();
            Program.Run(hexEngine);
        }
        public void ChangeControl(Control newControl)
        {
            // mainControl = newControl;
            Content = mainControl;
        }
    }
}