using System;
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
                new(0, buttomHeight),
                new(0, topHeight),
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
        public CoupledHexagon(Hex.Hex coordinate, int colorIndex, ColorManager colorManager)
        {
            this.colorManager = colorManager;
            this.coloredBlock = new Hex.Block(coordinate);
            ChangeBlockColor(colorIndex);
        }
        public CoupledHexagon(Hex.Hex coordinate, ColorManager colorManager)
        {
            this.colorManager = colorManager;
            this.coloredBlock = new Hex.Block(coordinate);
        }
        public CoupledHexagon(Hex.Block coloredBlock, ColorManager colorManager)
        {
            this.colorManager = colorManager;
            this.coloredBlock = coloredBlock;
            UpdateFilledColor();
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
        public double GetX()
        {
            return coloredBlock.X;
        }
        public double GetY()
        {
            return coloredBlock.Y;
        }
    }

    public class HexagonGrid : Canvas
    {
        private const double SinOf60 = 0.866025403784439; // The value of sin(60 degrees)
        private Core.IHexPrintable? backend;
        private readonly ColorManager colorManager;
        public HexagonGrid(Core.IHexPrintable? hexPrintable, ThemeManager themeManager)
        {
            // Initialize internal trackers
            backend = hexPrintable;
            colorManager = new(themeManager);
            Background = themeManager.FetchBrush("Background_Color");
            // Initialize hexagons
            InitializeHexagonsIfNotNull();
        }

        public void SetHexPrintable(Core.IHexPrintable? hexPrintable)
        {
            backend = hexPrintable;
            InitializeHexagonsIfNotNull();
        }

        public void InitializeHexagonsIfNotNull()
        {
            if (backend != null)
            {
                // Get the blocks from the backend
                Hex.Block[] blocks = backend.GetBlocks();
                Children.Clear();

                // Create hexagon based on the blocks
                for (int i = 0; i < blocks.Length; i++)
                {
                    Children.Add(new CoupledHexagon(blocks[i], colorManager));
                }
                InvalidateArrange(); // Request a layout update to arrange the hexagons
            }
        }
        public void UpdateLayout(double width, double height)
        {
            // Calculate the radius based on the width and height
            double min = Math.Min(width * SinOf60, height);
            double count = backend?.GetRadius() ?? 1;
            double radius = min / (count * 2);
            foreach (var child in Children)
            {
                if (child is CoupledHexagon hexagon)
                {
                    // hexagon.UpdateFilledColor();
                    hexagon.Height = radius * 2;
                    hexagon.Width = hexagon.Height * SinOf60;
                    // Calculate the position of the hexagon
                    // Set the position of the hexagon
                    Canvas.SetLeft(hexagon, hexagon.GetX() * radius * 2);
                    Canvas.SetTop(hexagon, hexagon.GetY() * radius * 2);
                }
            }
        }
    }
}