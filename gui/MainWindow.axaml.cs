using Avalonia;
using Avalonia.Controls;
using System;

namespace game_InfinityHex.UI
{
    public partial class MainWindow : Window
    {
        private readonly TitlePanel titlePanel = new TitlePanel();
        private readonly HintsPanel hintsPanel = new HintsPanel();
        private readonly AuthorPanel authorPanel = new AuthorPanel();
        private readonly LaunchContent launchContent = new LaunchContent();
        public MainWindow()
        {
            Background = ThemeManager.DefaultManager.FetchBrush("Background_Color");
            MinWidth = 400;
            MinHeight = 400;
            Width = MinWidth;
            Height = MinHeight;
            if (!string.IsNullOrEmpty(ProgramInfo.GameVersion?.ToString()))
                Title = $"{ProgramInfo.GameName} Version {ProgramInfo.GameVersion}";
            else
                Title = ProgramInfo.GameName;

            Panel WindowRoot = new Grid();
            Content = WindowRoot;
            WindowRoot.Children.Add(titlePanel);
            WindowRoot.Children.Add(hintsPanel);
            WindowRoot.Children.Add(authorPanel);
            WindowRoot.Children.Add(launchContent);

            LayoutUpdated += OnLayoutUpdated;

            InitializeComponent();
        }
        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            if (Bounds.Width > 0 && Bounds.Height > 0)
            {
                titlePanel.UpdateLayout(new Size(Bounds.Width, Bounds.Height));
                hintsPanel.UpdateLayout(new Size(Bounds.Width, Bounds.Height));
                authorPanel.UpdateLayout(new Size(Bounds.Width, Bounds.Height));
                launchContent.UpdateLayout(new Size(Bounds.Width, Bounds.Height));
            }
        }
    }
}