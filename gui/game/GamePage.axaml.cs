using Avalonia.Controls;
using System;

namespace game_InfinityHex.UI
{
    public class GamePage : UserControl
    {
        private readonly HexagonGrid gameGrid;
        public GamePage()
        {
            Background = ThemeManager.DefaultManager.FetchBrush("GamePanel_Background_Color");
            gameGrid = new HexagonGrid(engine.BackendManager.DefaultManager.Engine(), ThemeManager.DefaultManager);
            LayoutUpdated += OnLayoutUpdated;
            Content = gameGrid;
        }
        private void OnLayoutUpdated(object? sender, EventArgs e)
        {
            if (Bounds.Width > 0 && Bounds.Height > 0)
            {
                gameGrid.UpdateLayout(Bounds.Width, Bounds.Height);
            }
        }
    }
}