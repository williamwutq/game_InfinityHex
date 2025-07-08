using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace game_InfinityHex.UI
{
    public partial class HintsPanel : Border
    {
        private static readonly HintsManager globalHintsManager = new([
            "Consider HappyHex?",
            "Try HappyHex, another game",
            "Just start the game",
            "What if InfinityHex...",
            "What if InfinityHex... was sad",
            "What if InfinityHex... was bad",
            "What if InfinityHex... was hard",
            "What if InfinityHex... was old",
            "Color means nothing to me",
            "JSON JSON Customize me!",
            "Wanting a custom theme?",
            "Dark theme best theme",
            "Well this is still beta, to say",
            "Hello World! First C# project",
            "I might be a fan of this game",
            "What are you thinking?",
            "Simply click start to start",
            "Write a theme if you want",
            "I tried, I said",
            "Everyday I play this game",
            "Everyday my life is better",
            "Everyday is a blessing",
            "What is the coolest feature?",
            "Ohhh is it really that boring?",
        ]);

        private readonly Border internalBorder;
        private readonly TextBlock titleText;
        public HintsPanel()
        {
            Background = ThemeManager.DefaultManager.FetchBrush("Background_Color");
            Padding = new Thickness(1);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new Thickness(0, 0, 0, 0);

            internalBorder = new Border
            {
                Background = ThemeManager.DefaultManager.FetchBrush("TitlePanel_Background_Color"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            titleText = new TextBlock
            {
                Text = globalHintsManager.GetHint(),
                FontFamily = ThemeManager.DefaultManager.FetchFont("TitlePanel_Text_Font"),
                FontWeight = FontWeight.Regular,
                FontStyle = FontStyle.Italic,
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
            double pad = Math.Min(containerSize.Width, containerSize.Height) / 128;
            Padding = new Thickness(pad);

            // Height = 1/8 of height and full width
            Height = containerSize.Height / 32;
            Margin = new Thickness(0, Height * 3, 0, 0);
            Width = containerSize.Width;
            internalBorder.Height = Height - pad * 2;
            internalBorder.Width = Width - pad * 2;
            internalBorder.CornerRadius = new CornerRadius(internalBorder.Height / 4);
           
            titleText.FontSize = internalBorder.Height / 1.5;
        }
    }
}