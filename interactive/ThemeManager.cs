using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace game_InfinityHex.UI;

/// <summary>
/// Represents a theme with optional inheritance and a set of key-value fields.
/// Each field is a color or font setting used throughout the UI.
/// </summary>
public class Theme
{
    /// <summary>
    /// Name of the theme. Used as a unique identifier.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Theme field values, such as color or font definitions.
    /// </summary>
    public Dictionary<string, string> Fields { get; }
    /// <summary>
    /// Constructs a Theme instance.
    /// </summary>
    public Theme(string name, Dictionary<string, string> fields)
    {
        Name = name;
        Fields = fields;
    }

    /// <summary>
    /// Returns a resolved brush from a field key.
    /// The default key is "Color". If you intend to fetch a brush for a different field, specify the key explicitly.
    /// If no color is found, returns <see cref="Brushes.Transparent"/> as a fallback.
    /// </summary>
    public IBrush FetchBrush(string key = "Color")
    {
        foreach (string fallback in GenerateFallbackKeys(key))
        {
            if (Fields.TryGetValue(fallback, out var val))
            {
                if (val != null && Color.TryParse(val, out var color))
                {
                    return new SolidColorBrush(color);
                }
            }
        }
        return Brushes.Transparent;
    }
    /// <summary>
    /// Returns a resolved font family from a field key.
    /// The default key is "Font". If you intend to fetch a font for a different field, specify the key explicitly.
    /// If no font is found, returns <c>new FontFamily("sans-serif")</c> as a fallback.
    /// </summary>
    public FontFamily FetchFont(string key = "Font")
    {
        foreach (string fallback in GenerateFallbackKeys(key))
        {
            if (Fields.TryGetValue(fallback, out var val))
            {
                if (val != null)
                {
                    try
                    {
                        return FontFamily.Parse(val);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                }
            }
        }
        return new FontFamily("sans-serif");
    }
    /// <summary>
    /// Generates fallback keys from a key like A_B_C_D into [A_B_C_D, B_C_D, C_D, D].
    /// </summary>
    private static IEnumerable<string> GenerateFallbackKeys(string key)
    {
        var parts = key.Split('_');
        for (int i = 0; i < parts.Length; i++)
        {
            yield return string.Join('_', parts.Skip(i));
        }
    }
    /// <summary>
    /// Returns a debug string representation of the theme, showing inheritance and merged fields.
    /// </summary>
    public override string ToString()
    {
        string fieldString = string.Join(", ", Fields.Select(kv => $"{kv.Key}={kv.Value}"));
        return $"Theme[{fieldString}]";
    }
}

/// <summary>
/// Manages loading and accessing multiple themes, including fallback resolution.
/// </summary>
public class ThemeManager
{
#if DEBUG
    private static readonly string projectBaseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
#else
    private static readonly string projectBaseDir = Path.GetFullPath(AppContext.BaseDirectory);
#endif
    /// <summary>
    /// Gets the default <see cref="ThemeManager"/> instance used across the application (read-only).
    /// </summary>
    /// <remarks>
    /// <para>
    /// During initialization, the <c>DefaultManager</c> will be attempted to be initialized by the class
    /// using configuration settings from <c>config/Setting.json</c>. It looks for:
    /// <list type="bullet">
    /// <item>
    /// <term>ThemePath</term>
    /// <description>Specifies the directory from which to load themes.</description>
    /// </item>
    /// <item>
    /// <term>Theme</term>
    /// <description>The name of the theme to load and apply.</description>
    /// </item>
    /// </list>
    /// If the <c>ThemePath</c> is invalid or not present, it falls back to the default path <c>config/themes</c>.
    /// If the specified <c>Theme</c> cannot be set, it attempts to apply a theme named <c>Default</c>.
    /// </para>
    /// <para>
    /// The static property is guaranteed to be non-null after class initialization, and usually can be used as the first
    /// theme to be used for current display, unless the user prefer other themes that they may choose.
    /// This property cannot be otherwise set.
    /// </para>
    /// </remarks>
    public static ThemeManager DefaultManager { get; }
    static ThemeManager()
    {
        string defaultThemeDir = "config/themes";
        string settingPath = Path.Combine(projectBaseDir, "config/Setting.json");
        string? themePath = null;
        string? themeName = null;
        ThemeManager manager;
        // Try to read Setting.json for ThemePath and Theme
        if (File.Exists(settingPath))
        {
            try
            {
                var settingJson = File.ReadAllText(settingPath);
                var settingDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(settingJson);
                if (settingDict != null)
                {
                    if (settingDict.TryGetValue("ThemePath", out var themePathElem))
                        themePath = themePathElem.GetString();
                    if (settingDict.TryGetValue("Theme", out var themeNameElem))
                        themeName = themeNameElem.GetString();
                }
            }
            catch { }
        }
        // Try loading from ThemePath if specified
        if (!string.IsNullOrWhiteSpace(themePath))
        {
            manager = new ThemeManager(themePath);
            if (!manager.GetAvailableThemes().Any())
            {
                manager = new ThemeManager(defaultThemeDir);
            }
        }
        else
        {
            manager = new ThemeManager(defaultThemeDir);
        }
        // Try to set theme if specified
        if (!string.IsNullOrWhiteSpace(themeName))
        {
            if (!manager.SetCurrentTheme(themeName))
            {
                manager.SetCurrentTheme("Default");
            }
        }
        DefaultManager = manager;
    }

    private readonly Dictionary<string, Theme> Themes = [];
    /// <summary>
    /// The currently active theme name (read-only).
    /// </summary>
    public string CurrentThemeName { get; private set; } = "Base";
    /// <summary>
    /// Gets the currently active theme.
    /// </summary>
    public Theme CurrentTheme => Themes.TryGetValue(CurrentThemeName, out var t) ? t : Themes["Base"];
    /// <summary>
    /// Sets the current theme by name. Returns false if the name does not exist.
    /// </summary>
    /// <returns>Whether theme has been successfully set</returns>
    public bool SetCurrentTheme(string name)
    {
        if (Themes.ContainsKey(name))
        {
            CurrentThemeName = name;
            return true;
        }
        else return false;
    }
    /// <summary>
    /// Initializes and loads themes from a directory. Also injects a hardcoded "Base" theme.
    /// Each valid theme must include "Name" field and a "Type" field with value "HexTheme".
    /// </summary>
    public ThemeManager(string folder)
    {
        // Hardcoded base theme
        Themes["Base"] = new Theme(
            "Base", new()
            {
                ["Color"] = "#00000000",
                ["Font"] = "sans-serif"
            }
        );
        if (!Path.IsPathRooted(folder))
        {
            folder = Path.Combine(projectBaseDir, folder.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }
        if (!Directory.Exists(folder))
        {
            return;
        }
        else foreach (string file in Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories))
            {
                string json = File.ReadAllText(file);
                var raw = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;
                if (raw == null || !raw.TryGetValue("Type", out var typeProp) || typeProp.GetString() != "HexTheme")
                {
                    continue;
                }
                if (!raw.TryGetValue("Name", out var nameProp))
                {
                    continue;
                }
                string name = nameProp.GetString()!;
                if (Path.GetFileNameWithoutExtension(file) != name) continue;
                var fieldMap = raw
                    .Where(kv => kv.Key != "Name" && kv.Key != "Type")
                    .ToDictionary(kv => kv.Key, kv => kv.Value.GetString() ?? "");
                Themes[name] = new Theme(name, fieldMap);
            }
    }
    /// <summary>
    /// Returns a color brush from the current theme using fallback rules.
    /// </summary>
    public IBrush FetchBrush(string key = "Color") => CurrentTheme.FetchBrush(key);
    /// <summary>
    /// Returns a font family from the current theme using fallback rules.
    /// </summary>
    public FontFamily FetchFont(string key = "Font") => CurrentTheme.FetchFont(key);
    /// <summary>
    /// Lists all available theme names.
    /// </summary>
    public IEnumerable<string> GetAvailableThemes() => Themes.Keys;
    /// <summary>
    /// Returns a debug string representation of the ThemeManager, listing all themes.
    /// </summary>
    public override string ToString()
    {
        var themeList = string.Join(", ", Themes.Keys);
        return $"ThemeManager[Current={CurrentThemeName}, Themes=[{themeList}]]";
    }
}