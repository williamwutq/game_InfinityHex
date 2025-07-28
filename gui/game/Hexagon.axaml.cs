using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Core;

namespace game_InfinityHex.UI
{
    /// <summary>
    /// Represents a hexagon shape used in the game UI.
    /// This class inherits from <see cref="Control"/> and provides methods to render a hexagon with a specified fill color.
    /// </summary>
    /// <remarks>
    /// The hexagon is rendered with a top-flat orientation, and the fill color can be set using the <see cref="SetFillColor(IBrush?)"/> method.
    /// The hexagon's dimensions are determined by the bounds of the control, and it is centered and filled within its allocated space.
    /// During rendering, the hexagon is drawn with a gap of 5% of its width and height to create a visually appealing layout and to distinguish it from adjacent hexagons.
    /// </remarks>
    public class Hexagon : Control
    {
        private const double gapRatio = 0.05;
        private IBrush? FillColor;
        /// <summary>
        /// Initializes a new instance of the <see cref="Hexagon"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the hexagon with no fill color.
        /// The fill color can be set later using the <see cref="SetFillColor(IBrush?)"/> method.
        /// </remarks>
        public Hexagon()
        {
            FillColor = null;
        }
        /// <summary>
        /// Renders the hexagon shape using the specified fill color.
        /// </summary>
        /// <param name="context">The drawing context used to render the hexagon.</param>
        /// <remarks>
        /// This method calculates the points of the hexagon based on the control's bounds and draws the hexagon using the specified fill color.
        /// The hexagon is rendered with a top-flat orientation, and the points are calculated to ensure it fits within the control's dimensions.
        /// The hexagon is centered within the control, and its size is adjusted based on the bounds of the control.
        /// </remarks>
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
        /// <summary>
        /// Sets the fill color of the hexagon. This will invalidate the visual to apply the new color.
        /// </summary>
        /// <param name="color">The brush to use as the fill color for the hexagon.</param>
        public void SetFillColor(IBrush? color)
        {
            FillColor = color;
            InvalidateVisual(); // Request a redraw to apply the new fill color
        }
    }

    /// <summary>
    /// This class extends from <see cref="Hexagon"/> and is used to represent a hexagon in the game that is coupled with a block.
    /// It is used to display the hexagon in the UI and to change its color.
    /// </summary>
    public class CoupledHexagon : Hexagon
    {
        private Hex.Block coloredBlock;
        private readonly ColorManager colorManager;
        /// <summary>
        /// Initializes a new instance of the CoupledHexagon class with a specified coordinate and color index.
        /// </summary>
        /// <param name="coordinate">The coordinate of the hexagon.</param>
        /// <param name="colorIndex">The color index of the hexagon.</param>
        /// <param name="colorManager">The color manager used to interpret the color index.</param>
        /// <remarks>
        /// This constructor initializes the hexagon with a specified coordinate and color index,
        /// and sets the fill color based on the color index using the provided color manager.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="colorManager"/> or <paramref name="coordinate"/> is null.</exception>
        public CoupledHexagon(Hex.Hex coordinate, int colorIndex, ColorManager colorManager)
        {
            ArgumentNullException.ThrowIfNull(colorManager);
            ArgumentNullException.ThrowIfNull(coordinate);
            this.colorManager = colorManager;
            this.coloredBlock = new Hex.Block(coordinate);
            ChangeBlockColor(colorIndex);
        }
        /// <summary>
        /// Initializes a new instance of the CoupledHexagon class with a specified coordinate and color manager.
        /// </summary>
        /// <param name="coordinate">The coordinate of the hexagon.</param>
        /// <param name="colorManager">The color manager used to interpret the color of the block.</param>
        /// <remarks>
        /// This constructor initializes the hexagon with a specified coordinate with the inital color index of -1 (unoccupied),
        /// and sets the fill color based on the block's color using the provided color manager. This allows the hexagon to be coupled with a specific block,
        /// enabling dynamic updates to its color and appearance, while ensuring constant coordinate reference for rendering. The internal block prevents potential rendering issues
        /// when the underlying block may be modified, merged, or garbage collected by backend mechanisms.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="colorManager"/> or <paramref name="coordinate"/> is null.</exception>
        public CoupledHexagon(Hex.Hex coordinate, ColorManager colorManager)
        {
            ArgumentNullException.ThrowIfNull(colorManager);
            ArgumentNullException.ThrowIfNull(coordinate);
            this.colorManager = colorManager;
            this.coloredBlock = new Hex.Block(coordinate);
        }
        /// <summary>
        /// Initializes a new instance of the CoupledHexagon class with a specified colored block.
        /// </summary>
        /// <param name="coloredBlock">The colored block associated with the hexagon.</param>
        /// <param name="colorManager">The color manager used to interpret the color of the block.</param>
        /// <remarks>
        /// This constructor initializes the hexagon by cloning the provided colored block to ensure it has its own copy,
        /// and sets the fill color based on the block's color using the provided color manager.
        /// This allows the hexagon to be coupled with a specific block, enabling dynamic updates to its color and appearance,
        /// while ensuring that the referenced block alway exist when called by the rendering thread when the underlying block may be modified, 
        /// merged, or garbage collected by backend mechanisms, preventing potential rendering issues.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="colorManager"/> or <paramref name="coloredBlock"/> is null.</exception>
        public CoupledHexagon(Hex.Block coloredBlock, ColorManager colorManager)
        {
            ArgumentNullException.ThrowIfNull(colorManager);
            ArgumentNullException.ThrowIfNull(coloredBlock);
            this.colorManager = colorManager;
            this.coloredBlock = coloredBlock.Clone();
            UpdateFilledColor();
        }
        /// <summary>
        /// Updates the fill color of the hexagon based on the color of the associated block.
        /// </summary>
        /// <remarks>
        /// This method retrieves the color index of the associated block and interprets it using the color manager,
        /// then sets the fill color of the hexagon accordingly. This methid is called during initialization and
        /// whenever the block color changes.
        /// </remarks>
        public void UpdateFilledColor()
        {
            SetFillColor(colorManager.InterpretColor(coloredBlock.Color()));
        }
        /// <summary>
        /// Changes the associated block of the hexagon. This also updates the fill color of the hexagon.
        /// </summary>
        /// <param name="newBlock">The new block to associate with the hexagon.</param>
        /// <remarks>
        /// This method clones the provided block and updates the internal reference to the colored block.
        /// All fields of the block are copied, including the coordinate and color index. If the coordinate layout is changed, this may lead to unexpected behavior.
        /// In addition, creation of a new block and sparse reference may degrade performance due to object creation and poor memory locality.
        /// For safer usage, prefer <see cref="ChangeBlockColor(int)"/> to change the color of the block instead of replacing the block.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newBlock"/> is null.</exception>
        public void ChangeBlock(Hex.Block newBlock)
        {
            ArgumentNullException.ThrowIfNull(newBlock);
            coloredBlock = newBlock.Clone();
            UpdateFilledColor();
        }
        /// <summary>
        /// Changes the color of the associated block and updates the fill color of the hexagon.
        /// </summary>
        /// <param name="newColorIndex">The new color index to set for the block.</param>
        /// <remarks>
        /// This method is considered safer than directly update block because it does not update the reference used by the renderer,
        /// which may potentially lead to references that will be modified, merged, or garbage collected by backend mechanisms.
        /// </remarks>
        public void ChangeBlockColor(int newColorIndex)
        {
            coloredBlock.SetColor(newColorIndex);
            UpdateFilledColor();
        }
        /// <summary>
        /// Gets the x coordinate of the hexagon. See <see cref="Hex.Hex.X"/> for implementation details about the coordinate system.
        /// </summary>
        /// <returns>The Cartesian X coordinate of the hexagon.</returns>
        public double GetX()
        {
            return coloredBlock.X;
        }
        /// <summary>
        /// Gets the y coordinate of the hexagon. See <see cref="Hex.Hex.Y"/> for implementation details about the coordinate system.
        /// </summary>
        /// <returns>The Cartesian Y coordinate of the hexagon.</returns>
        /// <remarks>
        public double GetY()
        {
            return coloredBlock.Y;
        }
    }
    /// <summary>
    /// Represents a hexagon grid control that displays hexagonal blocks in a grid layout.
    /// This control is used to render the hexagonal grid in the game UI, allowing for dynamic updates and interactions.
    /// It inherits from <see cref="Canvas"/> to allow for custom layout and rendering of hexagonal shapes.
    /// </summary>
    public class HexagonGrid : Canvas
    {
        private const double SinOf60 = 0.866025403784439; // The value of sin(60 degrees)
        private Core.IHexPrintable? backend;
        private readonly ColorManager colorManager;
        /// <summary>
        /// Initializes a new instance of the HexagonGrid class with a specified backend and theme manager.
        /// </summary>
        /// <param name="hexPrintable">The backend that provides the hexagon grid data.</param>
        /// <param name="themeManager">The theme manager used to fetch colors and styles.</param>
        /// <remarks>
        /// This constructor initializes the hexagon grid with a specified backend that implements <see cref="Core.IHexPrintable"/>,
        /// and sets the background color using the theme manager. It also initializes the internal color manager to handle color interpretations.
        /// The hexagon grid is designed to display the hexagonal blocks in a grid layout, and it will update its layout based on the bounds of the control.
        /// </remarks>
        public HexagonGrid(Core.IHexPrintable? hexPrintable, ThemeManager themeManager)
        {
            // Initialize internal trackers
            colorManager = new(themeManager);
            Background = themeManager.FetchBrush("Background_Color");
            SetHexPrintable(hexPrintable);
        }

        /// <summary>
        /// Detaches the hex printable from the hexagon grid, removing the event subscription and clearing the backend reference.
        /// This method is used to clean up the hexagon grid when it is no longer needed or when the backend is changed.
        /// </summary>
        public void DetatchHexPrintable()
        {
            if (backend != null)
            {
                backend.OnHexRender -= OnBackendUpdated;
                backend = null;
            }
        }
        /// <summary>
        /// Sets the backend for the hexagon grid and subscribes to the OnHexRender event.
        /// This method updates the hexagon grid whenever the backend triggers a render event.
        /// </summary>
        /// <param name="hexPrintable">The hex printable object containing the grid data to render.</param>
        /// <remarks>
        /// This method sets the backend for the hexagon grid, allowing it to receive updates from the backend.
        /// If the current thread is not the UI thread, it dispatches the update to the UI thread to ensure thread safety.
        /// For rendering, it subscribes to the OnHexRender event of the backend, which will trigger the hexagon grid
        /// to update its layout and colors whenever the backend is updated. Each time only when the grid is updated,
        /// it will recolor the hexagons using <see cref="Hexagon.ChangeBlockColor"/> method based on the current state
        /// of the backend. If the grid size is changed, it will reinitialize the hexagons to match the new size using
        /// the <see cref="InitializeHexagonsIfNotNull"/> method.
        /// </remarks>
        public void SetHexPrintable(IHexPrintable? hexPrintable)
        {
            backend = hexPrintable;
            if (backend != null)
            {
                backend.OnHexRender += OnBackendUpdated;
            }
            InitializeHexagonsIfNotNull();
        }
        /// <summary>
        /// Updates the hexagon grid blocks based on the backend's hex printable data if the backend is updated.
        /// This method is called whenever the backend triggers a render update event.
        /// </summary>
        /// <param name="hexPrintable">The hex printable object containing the updated grid data.</param>
        private void OnBackendUpdated(IHexPrintable hexPrintable)
        {
            // Check if this is running on the UI thread
            if (!Dispatcher.UIThread.CheckAccess())
            {
                // If not on UI thread, dispatch to UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    UpdateHexagons();
                });
            }
            else
            {
                // Already on UI thread, execute directly
                UpdateHexagons();
            }
            void UpdateHexagons()
            {
                if (hexPrintable.IsGridUpdated())
                {
                    Hex.Block[] blocks = hexPrintable.GetBlocks();
                    if (blocks.Length != Children.Count)
                    {
                        // If the number of blocks has changed, clear and reinitialize the hexagons
                        InitializeHexagonsIfNotNull();
                    }
                    for (int i = 0; i < blocks.Length; i++)
                    {
                        if (Children[i] is CoupledHexagon hexagon)
                        {
                            hexagon.ChangeBlockColor(blocks[i].Color()); // This is safer than changing the blocks directly
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Initializes the hexagons in the grid based on the blocks from the backend.
        /// This method clears the existing children and creates new hexagons based on the blocks provided by the backend.
        /// </summary>
        /// <remarks>
        /// This method is called when the backend is set or when the hexagons need to be initialized or reinitialized.
        /// It ensures that the hexagons are created based on the current state of the backend's block array.
        /// </remarks>
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
        /// <summary>
        /// Updates the layout of the hexagon grid based on the current bounds of the control.
        /// This method calculates the radius of the hexagons based on the control's width and height,
        /// and positions each hexagon accordingly.
        /// </summary>
        /// <param name="width">The width of the control.</param>
        /// <param name="height">The height of the control.</param>
        /// <remarks>
        /// This method is called whenever the layout of the control is updated, ensuring that the hexagons are properly sized and positioned within the control.
        /// The hexagons are arranged in a grid pattern, with each hexagon's position calculated based on its coordinate in the hexagonal grid.
        /// The hexagons are centered within the control, and their size is adjusted based on the control's dimensions.
        /// </remarks>
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