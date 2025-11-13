using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VolexCameraInspection.DataModel;
using VolexCameraInspection.Services;

namespace VolexCameraInspection.ViewModels;

public partial class MainViewModel : ViewModelBase,IDisposable
{
    public ObservableCollection<string> FrontImages { get; set; } = [Path.Combine("Assets","camera.png"), Path.Combine("Assets", "camera.png"), Path.Combine("Assets", "camera.png"), Path.Combine("Assets", "camera.png")];

    public ObservableCollection<string> BackImages { get; set; } = [Path.Combine("Assets", "camera.png"), Path.Combine("Assets", "camera.png"), Path.Combine("Assets", "camera.png"), Path.Combine("Assets", "camera.png")];

    [ObservableProperty] string scanBadge = string.Empty;

    [ObservableProperty] string scanWorkNumber = string.Empty;

    [ObservableProperty] string scanPartNumber = string.Empty;
    

    [ObservableProperty]
    private bool focusBadge = true;

    [ObservableProperty]
    private bool focusWorkNumber = false;

    [ObservableProperty]
    private bool focusPartNumber = false;

    [ObservableProperty]
    private bool isEnabled = true;



    private readonly int LIMIT = 4;

    private Queue<TransactionModel> TransactionQueue = new Queue<TransactionModel>();

    private PLCService PLCService;

    private CameraService CameraService;
    
    private CancellationTokenSource CancellationTokenSource ;
    
    private List<FileSystemWatcher> FileSystemWatchers = [];
    
    private readonly ConfigService ConfigService;
    private readonly TransactionService transactionService;
    public record CameraImageModel(string name,ObservableCollection<Bitmap> imageName);

    public ObservableCollection<CameraImageModel> Cameras { get; set; } = [];

    public MainViewModel(PLCService pLCService,ConfigService configService,CameraService cameraService,TransactionService trservice)
    {
        PLCService = pLCService;
        CancellationTokenSource = new CancellationTokenSource();
        ConfigService = configService;
        CameraService = cameraService;
        transactionService = trservice;
        LoadWatcher();
    }
    private void LoadWatcher()
    {
        
        for (int i=0;i<ConfigService.Config.FTP_PATHS.Length;i++)
        {
            var f  = new FileSystemWatcher(ConfigService.Config.FTP_PATHS[i]);

            string foldername = new DirectoryInfo( ConfigService.Config.FTP_PATHS[i]).Name;
            f.Created += async (sender,e) =>await WatchCamera(sender,e);
            f.EnableRaisingEvents = true;
            f.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            f.Filter = "*.*";
            FileSystemWatchers.Add(f);
            LoadImages(foldername, i%2 == 0 ? "Front Camera" : "Back Camera");
        }
    }
    void LoadImages(string foldername,string? name=null,params TransactionCameraDetail[] details)
    {
        var find = Cameras.Where(x => x.name == foldername).FirstOrDefault();
        if (details.Length < 1 || find is null)
        {
            string[] images = new string[LIMIT];
            Array.Fill<string>(images, Path.Combine("Assets", "camera.png"));
            var imgs = images.Select(x => new Bitmap(AssetLoader.Open(new Uri($"avares://VolexCameraInspection/{x}"))));
            Cameras.Add(new CameraImageModel($"{name ?? foldername}", new ObservableCollection<Bitmap>(imgs)));
        }
        else
        {
            details = details.Where(x => x.Type == foldername).ToArray();
            int index = Cameras.IndexOf(find);
            Cameras[index].imageName.Clear();
            int left = LIMIT - details.Length;
            ObservableCollection<string> list = new ObservableCollection<string>();
            foreach (var detail in details)
            {
                string img = CameraService.GetImage(detail);
                Cameras[index].imageName.Add(new Bitmap(img));
            }
            if (left > 0)
            {
                string[] images = new string[left];
                Array.Fill<string>(images, Path.Combine("Assets", "camera.png"));

                foreach (var i in images)
                {
                    Cameras[index].imageName.Add(new Bitmap(AssetLoader.Open(new Uri($"avares://VolexCameraInspection/{i}"))));
                }
                
            }

        }
        
    }


    [RelayCommand]
    public  void ScanText()
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
            else if (string.IsNullOrWhiteSpace(ScanWorkNumber)) FocusWorkNumber = true;

        }
        if (!string.IsNullOrEmpty(ScanBadge) && !string.IsNullOrEmpty(ScanPartNumber) &&  !string.IsNullOrEmpty(ScanWorkNumber))
        {
            //Process Start
            TransactionModel model = new TransactionModel(ScanWorkNumber, ScanBadge, ScanPartNumber);
            TransactionQueue.Enqueue(model);
            //        ScanWorkNumber = string.Empty;
            //        ScanBadge = string.Empty;
            FocusPartNumber = true;
            FocusBadge = false;
            FocusWorkNumber = false;
            IsEnabled = false;
            //            await PLCService.Send(new Models.PLCItem(Models.PLCItem.PLCItemType.WR, "MR811", 1,"Start Mes"));
        }
    }
    [RelayCommand]
    public void OpenConfig()
    {
        ConfigWindow window = new ConfigWindow() { DataContext = App.Services.GetRequiredService<ConfigViewModel>() };
        window.Show();
    }

    public void StartProcess()
    {

        while (!CancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                var item = TransactionQueue.Peek();
                
            }
            catch (TaskCanceledException _)
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + " | " + ex.StackTrace);
            }
        }
    }

    public  async Task WatchCamera(object sender, FileSystemEventArgs e)
    {
        await Task.Delay(1000);
        if (TransactionQueue.Count < 1)
            return;
        var item = TransactionQueue.Peek();
        int index = item.Details.Count+1;
        FileInfo fileInfo = new FileInfo(e.FullPath);
        string code = $"{fileInfo.Directory!.Name[0]}{index}";
        bool result = false;
        item.Details.Add(new TransactionCameraDetail(item.Id, code,fileInfo!.Directory.Name,result, $"{code}{fileInfo.Extension}", DateTime.Now));
        CameraService.SaveImage(fileInfo, item.Details.Last());
        File.Delete(e.FullPath);
        LoadImages(fileInfo.Directory!.Name,null, item.Details.ToArray());
        if (item.Details.Count >= (LIMIT * 2))
        {
            await Task.Delay(1000);
            var tr = TransactionQueue.Dequeue();
            tr.FinalJudgement = tr.Details.Any(x => !x.Output) ? "NG" : "PASS";
            await transactionService.Save(tr);
            Cameras.Clear();
            for (int i = 0; i < ConfigService.Config.FTP_PATHS.Length; i++)
            {
                string foldername = new DirectoryInfo(ConfigService.Config.FTP_PATHS[i]).Name;
                IsEnabled = true;
                ScanPartNumber = string.Empty;
                LoadImages(foldername);
            }
        }
    }

    public void Dispose()
    {
        CancellationTokenSource.Cancel();
    }
}
