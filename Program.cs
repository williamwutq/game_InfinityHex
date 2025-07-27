using Avalonia;
using Engine;
using Interactive;
using System;
using System.Threading;

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

//*
partial class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // HexEngine hexEngine = SetUpBackend();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        
        // Run(hexEngine);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<UI.App>().UsePlatformDetect().WithInterFont().LogToTrace();

    /// <summary>
    /// Sets up the backend for the game.
    /// </summary>
    public static HexEngine SetUpBackend()
    {
        HexEngine hexEngine = new HexEngine();
        KeyboardListener listener = new KeyboardListener(hexEngine.GetDirectionManager());
        listener.Start();
        return hexEngine;
    }

    public static void Run(HexEngine hexEngine)
    {
        Thread thread = new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(200);

                hexEngine.Move();
                // Console.WriteLine(((Core.IHexPrintable)hexEngine).GetASCIIArt());
            }
        });
        thread.IsBackground = true;
        thread.Start();
    }
}
//*/