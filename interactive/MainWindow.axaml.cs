using Avalonia;
using Avalonia.Controls;
using System;

namespace game_InfinityHex.UI
{
    public partial class MainWindow : Window
    {
        private readonly TitlePanel TitlePanel = new TitlePanel();
        public MainWindow()
        {
            Background = ColorManager.FetchColor("DefaultBackgroundColor");
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
            WindowRoot.Children.Add(TitlePanel);

            LayoutUpdated += OnLayoutUpdated;

            InitializeComponent();
        }
        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            if (Bounds.Width > 0 && Bounds.Height > 0)
            {
                TitlePanel.UpdateLayout(new Size(Bounds.Width, Bounds.Height));
            }
        }
    }
}