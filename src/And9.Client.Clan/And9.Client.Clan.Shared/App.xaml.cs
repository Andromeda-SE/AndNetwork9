using System;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using And9.Client.Clan.Views.Pages;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Uno.Extensions;
using WinUIEx;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

namespace And9.Client.Clan;

/// <summary>
///     Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application, IDisposable
{
    private readonly StreamWriter? _logStream;
    private Window? _window;

    /// <summary>
    ///     Initializes the singleton application object.  This is the first line of authored code
    ///     executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
#if WINDOWS
        string logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineers", "AndLogs");
        Directory.CreateDirectory(logFolderPath);
        string logFilePath = Path.Combine(logFolderPath, $"AndNetLog-{DateTime.UtcNow.Ticks:D}.log");
        _logStream = File.CreateText(logFilePath);
        Console.SetOut(_logStream);
        Console.SetError(_logStream);
#endif
        Current.UnhandledException += CurrentOnUnhandledException;
        InitializeLogging();
        InitializeComponent();
#if HAS_UNO || NETFX_CORE
            this.Suspending += OnSuspending;
#endif
    }

    public void Dispose()
    {
        _logStream?.Flush();
        _logStream?.Close();
    }

    private void CurrentOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Ioc.Default.GetRequiredService<ILogger>().LogCritical(e.Exception, "Unhandled exception");
    }

    /// <summary>
    ///     Invoked when the application is launched normally by the end user.  Other entry points
    ///     will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (_window is not null) return;
#if DEBUG
        if (Debugger.IsAttached)
        {
            // this.DebugSettings.EnableFrameRateCounter = true;
        }
#endif

#if NET5_0_OR_GREATER && WINDOWS
        _window = new WindowEx
        {
            Title = "AndromedaNet",
            MinWidth = 640,
            MinHeight = 480,
        };
        _window.Closed += WindowOnClosed;
        _window.VisibilityChanged += WindowOnVisibilityChanged;
        ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompatOnOnActivated;
        _window.Activate();
#else
            _window = Microsoft.UI.Xaml.Window.Current;
#endif

        Frame? frame = _window.Content as Frame;

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (frame == null)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            frame = new();

            frame.NavigationFailed += OnNavigationFailed;

            if (args.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // TODO: Load state from previously suspended application
            }

            // Place the frame in the current Window
            _window.Content = frame;
        }

        InitializeUi(Environment.GetCommandLineArgs());

#if !(NET5_0_OR_GREATER && WINDOWS)
            if (args.UWPLaunchActivatedEventArgs.PrelaunchActivated == false)
#endif
        {
            if (frame.Content == null)
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                frame.Navigate(typeof(LoginPage), args.Arguments);
            // Ensure the current window is active
            _window.Activate();
        }
    }

    private void WindowOnVisibilityChanged(object sender, WindowVisibilityChangedEventArgs args) { }

    private void WindowOnClosed(object sender, WindowEventArgs args)
    {
        if (_window?.Visible ?? false)
            try
            {
                ((WindowEx)_window).Minimize();
                args.Handled = true;
            }
            catch (Exception e) { }
    }
#if WINDOWS
    private void ToastNotificationManagerCompatOnOnActivated(ToastNotificationActivatedEventArgsCompat e)
    {
        WindowEx window = Ioc.Default.GetRequiredService<WindowEx>();
        window.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () => window.BringToFront());
    }
#endif


    /// <summary>
    ///     Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }

    /// <summary>
    ///     Invoked when application execution is being suspended.  Application state is saved
    ///     without knowing whether the application will be terminated or resumed with the contents
    ///     of memory still intact.
    /// </summary>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
        _logStream?.Close();
        SuspendingDeferral? deferral = e.SuspendingOperation.GetDeferral();
        // TODO: Save application state and stop any background activity
        deferral.Complete();
    }

    /// <summary>
    ///     Configures global Uno Platform logging
    /// </summary>
    private static void InitializeLogging()
    {
        ILoggerFactory? factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
            builder.AddConsole();
#endif

            // Exclude logs below this level
            builder.SetMinimumLevel(LogLevel.Information);

            // Default filters for Uno Platform namespaces
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);

            // Generic Xaml events
            // builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

            // Layouter specific messages
            // builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

            // builder.AddFilter("Windows.Storage", LogLevel.Debug );

            // Binding related messages
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

            // Binder memory references tracking
            // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

            // RemoteControl and HotReload related
            // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

            // Debug JS interop
            // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
        });

        LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
			global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
    }

    private void InitializeUi(string[] args)
    {
        IHost? host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                Startup.ConfigureServices(services);
                Gateway.Clan.Client.Startup.ConfigureServices(services);

#if WINDOWS
                services.AddSingleton<WindowEx>((WindowEx)_window);
                /*TrayIcon trayIcon = new TrayIcon(Icon.Yang());
                trayIcon.TrayIconLeftMouseDown += (sender, eventArgs) =>
                {
                    _window.Show();
                };
                services.AddSingleton<WinUIEx.TrayIcon>(trayIcon);*/
#endif
                services.AddSingleton((Frame)_window.Content);
            })
#if WINDOWS
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = false;
                    options.TimestampFormat = "[yy/MM/dd HH:mm:ss.fff]    ";
                    options.ColorBehavior = LoggerColorBehavior.Disabled;
                    options.UseUtcTimestamp = true;
                });
                logging.SetMinimumLevel(LogLevel.Information);
            })
#endif
            .Build();

        Ioc.Default.ConfigureServices(host.Services);
    }
}