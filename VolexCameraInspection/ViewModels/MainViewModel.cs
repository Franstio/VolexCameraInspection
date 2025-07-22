using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace VolexCameraInspection.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<string> LeftImages { get; set; } = ["camera.png", "camera.png", "camera.png", "camera.png"];

    public ObservableCollection<string> RightImages { get; set; } = ["camera.png", "camera.png", "camera.png", "camera.png"];

    [ObservableProperty] string scanBadge = string.Empty;

    [ObservableProperty] string scanWorkNumber = string.Empty;

    [ObservableProperty] string scanPartNumber = string.Empty;
    

    [ObservableProperty]
    private bool focusBadge = true;

    [ObservableProperty]
    private bool focusWorkNumber = false;

    [ObservableProperty]
    private bool focusPartNumber = false;

    [RelayCommand]
    public void ScanText()
    {
        if (FocusBadge)
        {
            FocusBadge = false;
            if (string.IsNullOrEmpty(ScanWorkNumber)) FocusWorkNumber = true;
            else if (string.IsNullOrEmpty(ScanPartNumber)) FocusPartNumber = true;
        }
        else if (FocusWorkNumber)
        {
            FocusWorkNumber = false;
            if (string.IsNullOrEmpty(ScanPartNumber)) FocusPartNumber = true;
            else if (string.IsNullOrEmpty(ScanBadge)) FocusBadge = true;
        }
        else if (FocusPartNumber)
        {
            FocusPartNumber = false;
            if (string.IsNullOrEmpty(ScanBadge)) FocusBadge = true;
            else if (string.IsNullOrWhiteSpace(ScanWorkNumber)) FocusWorkNumber= true;
        }
        else
        {
            //Process Start
        }
    }

    [RelayCommand]
    public void OpenConfig()
    {
        ConfigWindow window = new ConfigWindow() { DataContext = App.Services.GetRequiredService<ConfigViewModel>() };
        window.Show();
    }
}
