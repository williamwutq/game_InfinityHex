using Avalonia.Media;
using System.Collections.Generic;

namespace game_InfinityHex.UI;

/// <summary>
/// An enum contain all global themes for the game. Each item in the enum represent a theme.
/// </summary>
public enum Theme
{
    /// <summary>
    /// The default color theme
    /// </summary>
    /// <since>0.0.0</since>
    Default
}

/// <summary>
/// Manages themed color brushes for the application. 
/// Allows centralized access to named colors that vary by theme.
/// </summary>
public static partial class ColorManager
{
    /// <summary>
    /// The currently selected global theme. Affects all color fetches.
    /// </summary>
    public static Theme CurrentTheme { get; set; } = Theme.Default;
    private static readonly Dictionary<Theme, Dictionary<string, IBrush>> ThemeColors = new()
    {
        [Theme.Default] = new Dictionary<string, IBrush>
        {
            ["DefaultBackgroundColor"] = Brushes.White,
            ["DefaultBlockColor"] = Brushes.LightBlue,
            ["DefaultTextColor"] = Brushes.Black,
            ["DefaultLineColor"] = Brushes.DarkGray,
            ["DefaultShadowColor"] = Brushes.LightGray,
            ["TitlePanelBackgroundColor"] = Brushes.LightBlue,
        }
    };
    /// <summary>
    /// Gets the brush for the given key in the current theme.
    /// </summary>
    public static IBrush FetchColor(string target)
    {
        Dictionary<string, IBrush> themeMap = ThemeColors.TryGetValue(CurrentTheme, out var dict)
            ? dict : ThemeColors[Theme.Default];

        return themeMap.TryGetValue(target, out var color)
            ? color
            : Brushes.Transparent; // fallback to transparent if not found
    }
}