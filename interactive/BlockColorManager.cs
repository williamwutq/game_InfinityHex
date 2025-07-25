using System;
using Avalonia.Media;

namespace game_InfinityHex.UI;

/// <summary>
/// Manages color interpretation for blocks using a specified <see cref="ThemeManager"/>.
/// Provides methods to retrieve brushes for block colors based on their index or block instance.
/// </summary>
public class ColorManager
{
    /// <summary>
    /// Gets the default <see cref="ColorManager"/> instance using the default <see cref="ThemeManager"/>.
    /// </summary>
    public static ColorManager DefaultManager { get; } = new ColorManager(ThemeManager.DefaultManager);
    private readonly ThemeManager themeManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorManager"/> class with the specified <see cref="ThemeManager"/>.
    /// </summary>
    /// <param name="themeManager">The theme manager used for color interpretation.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="themeManager"/> is null.</exception>
    public ColorManager(ThemeManager themeManager)
    {
        ArgumentNullException.ThrowIfNull(themeManager);
        this.themeManager = themeManager;
    }

    /// <summary>
    /// Prepares a color key string for the given block color index.
    /// </summary>
    /// <param name="index">The color index of the block.</param>
    /// <returns>A string key used to fetch the corresponding brush from the theme.</returns>
    private static String PrepareColorKey(int index)
    {
        return $"{index}_Indexed_Block_Color";
    }
    /// <summary>
    /// Interprets the color for a block based on its color index.
    /// </summary>
    /// <param name="index">The color index of the block.</param>
    /// <returns>An <see cref="IBrush"/> representing the block's color.</returns>
    public IBrush InterpretColor(int index)
    {
        return themeManager.CurrentTheme.FetchBrush(PrepareColorKey(index));
    }
    /// <summary>
    /// Interprets the color for the specified block.
    /// </summary>
    /// <param name="block">The block whose color is to be interpreted.</param>
    /// <returns>An <see cref="IBrush"/> representing the block's color.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="block"/> is null.</exception>
    public IBrush InterpretColor(Hex.Block block)
    {
        ArgumentNullException.ThrowIfNull(block);
        return InterpretColor(block.Color());
    }
}