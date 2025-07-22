using Avalonia;
using Avalonia.Controls;
using Engine;

namespace game_InfinityHex.UI
{
    public partial class MainWindow : Window
    {
        private Control mainControl;

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

            InitializeComponent();
        }
        public void ChangeControl(Control newControl)
        {
            mainControl = newControl;
            Content = mainControl;
        }
    }
}