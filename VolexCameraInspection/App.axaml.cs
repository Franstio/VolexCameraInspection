using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using VolexCameraInspection.Services;
using VolexCameraInspection.ViewModels;
using VolexCameraInspection.Views;

namespace VolexCameraInspection;

public static class ServiceExtension
{
    public static void AddCommonServices(this IServiceCollection serviceCollection)
    {
        Type[] vm = [typeof(MainViewModel),typeof(ConfigViewModel)];
        foreach (var item in vm)
            serviceCollection.AddSingleton(item);

        ConfigService configService = new ConfigService();
        configService.Load();
        serviceCollection.AddSingleton(configService);
        serviceCollection.AddSingleton<IFolderPickingService>(provider =>
            new FolderPickingService(() =>
            {
                // You can resolve current window here
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    return desktop.Windows.FirstOrDefault(w => w.IsActive);
                return null;
        }));
    }
}

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);
        ServiceCollection services = new ServiceCollection();
        services.AddCommonServices();
        Services = services.BuildServiceProvider();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
