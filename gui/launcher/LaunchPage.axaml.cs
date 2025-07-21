using Avalonia;
using Avalonia.Controls;
using System;

namespace game_InfinityHex.UI
{
    public class LaunchPage : UserControl
    {
        private readonly TitlePanel titlePanel = new TitlePanel();
        private readonly HintsPanel hintsPanel = new HintsPanel();
        private readonly AuthorPanel authorPanel = new AuthorPanel();
        private readonly LaunchContent launchContent = new LaunchContent();
        private readonly Panel rootPanel = new Grid();

        public LaunchPage()
        {
            Background = ThemeManager.DefaultManager.FetchBrush("LaunchPanel_Background_Color");

            Content = rootPanel;
            rootPanel.Children.Add(titlePanel);
            rootPanel.Children.Add(hintsPanel);
            rootPanel.Children.Add(authorPanel);
            rootPanel.Children.Add(launchContent);

            LayoutUpdated += OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            if (Bounds.Width > 0 && Bounds.Height > 0)
            {
                var size = new Size(Bounds.Width, Bounds.Height);
                titlePanel.UpdateLayout(size);
                hintsPanel.UpdateLayout(size);
                authorPanel.UpdateLayout(size);
                launchContent.UpdateLayout(size);
            }
        }
    }
}
