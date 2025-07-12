using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;

namespace game_InfinityHex.UI
{
    partial class LaunchContent : StackPanel
    {
        private readonly LaunchButton[] buttons;
        private const double PaddingRatio = 0.15; // 15% total
        public LaunchContent()
        {
            Orientation = Orientation.Vertical;
            Background = ThemeManager.DefaultManager.FetchBrush("Color");
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Bottom;
            Spacing = 16;
            buttons = [
                new LaunchButton("START", null),
            ];

            this.Children.AddRange(buttons);
        }
        public void UpdateLayout(Size containerSize)
        {
            // Set self size
            Width = containerSize.Width;
            double expectedHeight = containerSize.Height / 40 * 33;
            Margin = new Thickness(0, 0, 0, containerSize.Height / 20);
            // Decide children button size
            double availableSpace = expectedHeight * (1 - PaddingRatio);
            double spacingCount = buttons.Length + (buttons.Length - 1) * 0.25; // 25% spacing in between buttons
            double buttonHeight = availableSpace / spacingCount;
            // Protect against oversize
            if (buttonHeight > containerSize.Height / 12)
            {
                buttonHeight = containerSize.Height / 12;
            }
            // Calculate lower padding in addition to the author panel space
            double totalPadding = (expectedHeight - spacingCount * buttonHeight) / 2;
            // Calculate the height
            Height = expectedHeight - totalPadding;
            foreach (LaunchButton button in buttons)
            {
                button.SetSize(buttonHeight);
            }
        }
    }
}