using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolexCameraInspection.Services
{
    public interface IFolderPickingService
    {
        Task<Uri[]?> PickFolder(Window? owner=null);
    }
    public class FolderPickingService : IFolderPickingService
    {
        private Func<Window?> getDefaultWindow;
        public async Task<Uri[]?> PickFolder(Window? owner = null)
        {
            owner = owner?? getDefaultWindow();
            if (owner is null)
                return null;
            var pickDialog = await owner.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions() { AllowMultiple = true, Title = "Pick FTP Folder" });
            return pickDialog.Count > 0 ? pickDialog.Select(x=>x.Path).ToArray() : null;
        }

        public FolderPickingService(Func<Window?> window)
        {
            this.getDefaultWindow = window;
        }


    }
}
