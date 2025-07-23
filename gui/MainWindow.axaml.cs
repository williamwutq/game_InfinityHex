using Avalonia;
using Avalonia.Controls;
using Engine;

namespace game_InfinityHex.UI
{
    public partial class MainWindow : Window
    {
        private HexagonGrid mainControl;

        public MainWindow()
        {
            Background = ThemeManager.DefaultManager.FetchBrush("Background_Color");

            MinWidth = 400;
            MinHeight = 400;
            Width = MinWidth;
            Height = MinHeight;

            Title = !string.IsNullOrEmpty(ProgramInfo.GameVersion?.ToString())
                ? $"{ProgramInfo.GameName} Version {ProgramInfo.GameVersion}"
                : ProgramInfo.GameName;

            // Initialize with LaunchPanel
            mainControl = new HexagonGrid(Program.SetUpBackend(), ThemeManager.DefaultManager);
            Content = mainControl;
            LayoutUpdated += (sender, e) =>
            {
                if (Bounds.Width > 0 && Bounds.Height > 0)
                {
                    mainControl.UpdateLayout(Bounds.Width, Bounds.Height);
                }
            };

            InitializeComponent();
        }
        public void ChangeControl(Control newControl)
        {
            // mainControl = newControl;
            Content = mainControl;
        }
    }
}