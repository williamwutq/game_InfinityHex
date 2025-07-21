using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;

namespace game_InfinityHex.UI
{
    public partial class LaunchButton : Button
    {
        public LaunchButton(String text, Action? action)
        {
            Background = ThemeManager.DefaultManager.FetchBrush("Launch_Button_Background_Color");
            Foreground = ThemeManager.DefaultManager.FetchBrush("Launch_Button_Text_Color");
            FontFamily = ThemeManager.DefaultManager.FetchFont("Launch_Button_Text_Font");
            FontStyle = FontStyle.Normal;
            FontWeight = FontWeight.Bold;
            FontSize = 16;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Content = new TextBlock
            {
                Text = text,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            if (action != null) Click += (sender, e) => action();
        }
        public void SetSize(double height)
        {
            Width = height * 3;
            Height = height;
            FontSize = height * 2 / 3;
            CornerRadius = new Avalonia.CornerRadius(height / 3);
            Margin = new Avalonia.Thickness(height / 4, height / 8, height / 4, height / 8);
        }
    }
}