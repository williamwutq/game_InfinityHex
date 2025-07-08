using Avalonia;
using game_InfinityHex.UI;
using System;

namespace game_InfinityHex;

/// <summary>
/// Essential program information including version and description.
/// </summary>
partial class ProgramInfo
{
    /// <summary>
    /// Gets the build version of the compiled code.
    /// </summary>
    public static System.Version? CSharpVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
    /// <summary>
    /// Gets the operating system version as reported by the runtime environment.
    /// </summary>
    public static System.Version? OSVersion => System.Environment.OSVersion.Version;
    /// <summary>
    /// Gets a human-readable description of the operating system,
    /// </summary>
    public static string OSDescription => System.Runtime.InteropServices.RuntimeInformation.OSDescription;
}

partial class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<UI.App>().UsePlatformDetect().WithInterFont().LogToTrace();
}
