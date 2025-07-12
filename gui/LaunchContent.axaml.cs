using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;

namespace game_InfinityHex.UI
{
    partial class LaunchContent : StackPanel
    {
        private readonly LaunchButton startButton;
        public LaunchContent()
        {
            Orientation = Orientation.Vertical;
            Background = ThemeManager.DefaultManager.FetchBrush("Color");
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Bottom;
            Spacing = 16;
            startButton = new LaunchButton("START", null);

            this.Children.Add(startButton);
        }
        public void UpdateLayout(Size containerSize)
        {
            Width = containerSize.Width;
            Height = containerSize.Height / 40 * 33;
            Margin = new Thickness(0, 0, 0, containerSize.Height / 20);
        }
    }
}