using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace game_InfinityHex.UI
{
    public class Hexagon : Control
    {
        private IBrush? FillColor;
        public Hexagon()
        {
            FillColor = null;
        }
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            double width = Bounds.Width;
            double height = Bounds.Height;
            double halfWidth = width / 2;
            double topHeight = height / 4;
            double buttomHeight = topHeight * 3;

            // Define hexagon points (top flat)
            Point[] points = {
                new(halfWidth, 0),
                new(width, topHeight),
                new(width, buttomHeight),
                new(halfWidth, height),
                new(0, topHeight * 3),
                new(0, buttomHeight),
            };

            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(points[0], true);
                for (int i = 1; i < points.Length; i++)
                    ctx.LineTo(points[i]);
                ctx.EndFigure(true);
            }
            if (FillColor != null)
            {
                context.DrawGeometry(FillColor, null, geometry);
            }
        }
        public void SetFillColor(IBrush? color)
        {
            FillColor = color;
            InvalidateVisual(); // Request a redraw to apply the new fill color
        }
    }

    public class CoupledHexagon : Hexagon
    {
        private Hex.Block coloredBlock;
        private readonly ColorManager colorManager;
        public CoupledHexagon(Hex.Hex coordinate, ColorManager colorManager)
        {
            this.colorManager = colorManager;
            this.coloredBlock = new Hex.Block(coordinate);
        }
        public CoupledHexagon(Hex.Block coloredBlock, ColorManager colorManager)
        {
            this.colorManager = colorManager;
            this.coloredBlock = coloredBlock;
        }
        private void UpdateFilledColor()
        {
            SetFillColor(colorManager.InterpretColor(coloredBlock.Color()));
        }
        public void ChangeBlock(Hex.Block newBlock)
        {
            coloredBlock = newBlock;
            UpdateFilledColor();
        }
        public void ChangeBlockColor(int newColorIndex)
        {
            coloredBlock.SetColor(newColorIndex);
            UpdateFilledColor();
        }
    }
}