using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace game_InfinityHex.UI
{
    public partial class TitlePanel : Border
    {
        public TitlePanel()
        {
            Background = ColorManager.FetchColor("TitlePanelBackgroundColor");
            Padding = new Thickness(1);
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }
        public void UpdateLayout(Size containerSize)
        {
            // _adding: 1/32
            double pad = containerSize.Width / 32;
            Padding = new Thickness(pad);

            // Height = 1/8 of height and full width
            Height = containerSize.Height / 8;
            Width = containerSize.Width;
        }
    }
}