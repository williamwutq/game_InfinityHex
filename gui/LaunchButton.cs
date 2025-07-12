using Avalonia.Controls;
using Avalonia.Layout;
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
            FontSize = 16;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Content = text;
            if (action != null) Click += (sender, e) => action();
        }
        public void SetSize(double height)
        {
            Width = height * 6;
            Height = height;
            FontSize = height / 2;
            CornerRadius = new Avalonia.CornerRadius(height / 2);
        }
    }
}