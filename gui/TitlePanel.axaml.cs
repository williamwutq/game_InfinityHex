using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace game_InfinityHex.UI
{
    public partial class TitlePanel : Border
    {
        private readonly Border internalBorder;
        private readonly TextBlock titleText;
        public TitlePanel()
        {
            Background = ThemeManager.DefaultManager.FetchBrush("Background_Color");
            Padding = new Thickness(1);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;

            internalBorder = new Border
            {
                Background = ThemeManager.DefaultManager.FetchBrush("TitlePanel_Background_Color"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            titleText = new TextBlock
            {
                Text = ProgramInfo.GameName,
                FontFamily = ThemeManager.DefaultManager.FetchFont("TitlePanel_Text_Font"),
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 24,
                Foreground = ThemeManager.DefaultManager.FetchBrush("TitlePanel_Text_Color")
            };
            internalBorder.Child = titleText;
            Child = internalBorder;
        }
        public void UpdateLayout(Size containerSize)
        {
            // padding: 1/32
            double pad = Math.Min(containerSize.Width, containerSize.Height) / 32;
            Padding = new Thickness(pad);

            // Height = 1/8 of height and full width
            Height = containerSize.Height / 8;
            Width = containerSize.Width;
            internalBorder.Height = Height - pad * 2;
            internalBorder.Width = Width - pad * 2;
            internalBorder.CornerRadius = new CornerRadius(internalBorder.Height / 4);
           
            titleText.FontSize = internalBorder.Height / 1.5;
        }
    }
}