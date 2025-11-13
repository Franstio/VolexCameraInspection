using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VolexCameraInspection.Models;
using VolexCameraInspection.Services;

namespace VolexCameraInspection.ViewModels
{
    public partial class ConfigViewModel : ObservableObject
    {
        ConfigModel Config = null!;
        [ObservableProperty]
        string ip = "";
        [ObservableProperty]
        string ftp_Path = string.Empty;


        private readonly ConfigService configService;
        private readonly IFolderPickingService folderPickingService;
        public ConfigViewModel(ConfigService configservice, IFolderPickingService pickfolderservice)
        {
            configService = configservice;
            Config = configservice.Config;
            folderPickingService = pickfolderservice;
            Ftp_Path = string.Join(";", Config.FTP_PATHS);
            Ip = $"{Config.PLC_IP}:{Config.PLC_PORT}";
        }
        [RelayCommand]
        public void Save()
        {
            Config.PLC_IP = Ip.Split(":").FirstOrDefault() ?? Config.PLC_IP;
            Config.PLC_PORT = Ip.Split(":").LastOrDefault() ?? Config.PLC_PORT;
            configService.Save();
        }

        [RelayCommand]
        public async Task PickFolder()
        {
            var res = await folderPickingService.PickFolder();
            if (res != null)
            {
                Ftp_Path = string.Join(";", res.Select(x => x.LocalPath));
                Config.FTP_PATHS = res.Select(x => x.LocalPath).ToArray();
            }
        }
    }
}