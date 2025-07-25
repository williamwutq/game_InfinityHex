using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace game_InfinityHex.UI
{
    public class Hexagon : Control
    {
        private const double gapRatio = 0.05;
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
            double halfHeight = height / 2;
            double leftWidth = width * gapRatio;
            double rightWidth = width - leftWidth;
            double buttomHeight = height * gapRatio;
            double topHeight = height - buttomHeight;
            double heightAdjustment = halfHeight * (1 - gapRatio) / 2;
            double upperHeight = halfHeight + heightAdjustment;
            double lowerHeight = halfHeight - heightAdjustment;

            // Define hexagon points (top flat)
            Point[] points = {
                new(halfWidth, buttomHeight),
                new(rightWidth, lowerHeight),
                new(rightWidth, upperHeight),
                new(halfWidth, topHeight),
                new(leftWidth, upperHeight),
                new(leftWidth, lowerHeight),
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
            double rawRadius = backend?.GetRadius() ?? 1;
            double widthRatio = (rawRadius * 2 - 1) * SinOf60;
            double heightRatio = rawRadius * 1.5 - 0.5;
            double diameter = Math.Min(width / widthRatio, height / heightRatio);
            double radius = diameter * 0.5;
            // Calculate the centering offset
            double leftOffset = width * 0.5 - radius * SinOf60;
            double topOffset = height * 0.5 - radius;
            foreach (var child in Children)
            {
                if (child is CoupledHexagon hexagon)
                {
                    // hexagon.UpdateFilledColor();
                    hexagon.Height = radius * 2;
                    hexagon.Width = hexagon.Height * SinOf60;
                    // Calculate the position of the hexagon
                    // Set the position of the hexagon
                    Canvas.SetLeft(hexagon, hexagon.GetX() * diameter + leftOffset);
                    Canvas.SetTop(hexagon, hexagon.GetY() * diameter + topOffset);
                }
            }
        }
    }
}