using Avalonia.Controls;

namespace game_InfinityHex.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (!string.IsNullOrEmpty(ProgramInfo.GameVersion?.ToString()))
                Title = $"{ProgramInfo.GameName} Version {ProgramInfo.GameVersion}";
            else
                Title = ProgramInfo.GameName;
            InitializeComponent();
        }
    }
}