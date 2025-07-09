using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace game_InfinityHex.UI
{
    public class AuthorPanel : Grid
    {
        private readonly StackPanel stackLeft;
        private readonly StackPanel stackRight;
        private readonly TextBlock authorTextA;
        private readonly TextBlock authorTextWW;
        private readonly TextBlock authorTextGame;
        private readonly TextBlock copyrightText;
        public AuthorPanel()
        {
            Background = ThemeManager.DefaultManager.FetchBrush("Color");
            ColumnDefinitions = new ColumnDefinitions("*,*");
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Bottom;
            Margin = new Thickness(0, 0, 0, 0);

            stackLeft = new StackPanel()
            {
                Background = ThemeManager.DefaultManager.FetchBrush("Color"),
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            stackRight = new StackPanel()
            {
                Background = ThemeManager.DefaultManager.FetchBrush("Color"),
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            authorTextA = new TextBlock()
            {
                Text = "  A ",
                FontFamily = ThemeManager.DefaultManager.FetchFont("Author_AuthorPanel_Text_Font"),
                Foreground = ThemeManager.DefaultManager.FetchBrush("Author_AuthorPanel_Text_Color"),
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 20,
            };
            authorTextWW = new TextBlock()
            {
                Text = "W.W",
                FontFamily = ThemeManager.DefaultManager.FetchFont("WW_AuthorPanel_Text_Font"),
                Foreground = ThemeManager.DefaultManager.FetchBrush("WW_AuthorPanel_Text_Color"),
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 20,
            };
            authorTextGame = new TextBlock()
            {
                Text = " Game  ",
                FontFamily = ThemeManager.DefaultManager.FetchFont("Author_AuthorPanel_Text_Font"),
                Foreground = ThemeManager.DefaultManager.FetchBrush("Author_AuthorPanel_Text_Color"),
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 20,
            };
            copyrightText = new TextBlock()
            {
                Text = "  ©2025 William Wu  ",
                FontFamily = ThemeManager.DefaultManager.FetchFont("Author_AuthorPanel_Text_Font"),
                Foreground = ThemeManager.DefaultManager.FetchBrush("Author_AuthorPanel_Text_Color"),
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Normal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                FontSize = 20,
            };
            stackLeft.Children.Add(authorTextA);
            stackLeft.Children.Add(authorTextWW);
            stackLeft.Children.Add(authorTextGame);
            stackRight.Children.Add(copyrightText);
            Children.Add(stackLeft);
            Children.Add(stackRight);
            SetColumn(stackLeft, 0);
            SetColumn(stackRight, 1);
        }
        public void UpdateLayout(Size containerSize)
        {
            double launchAuthorSize = Math.Min(containerSize.Height, containerSize.Width * 2.25);
            double fontSize = launchAuthorSize / 40;

            Height = fontSize * 1.5;
            Width = containerSize.Width;
            authorTextA.FontSize = fontSize;
            authorTextWW.FontSize = fontSize;
            authorTextGame.FontSize = fontSize;
            copyrightText.FontSize = fontSize;
        }
    }
}