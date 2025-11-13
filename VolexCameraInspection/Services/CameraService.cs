using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolexCameraInspection.DataModel;

namespace VolexCameraInspection.Services
{
    public class CameraService
    {
        string basepath = Path.Combine(AppContext.BaseDirectory,"Assets","images");
        public CameraService() 
        {
            
        }  

        public void SaveImage(FileInfo fileInfo,TransactionCameraDetail detail)
        {
            Guid[] check = [detail.Transaction_Id];
            string path = Path.Combine(basepath);
            foreach (Guid c in check)
            {
                path = Path.Combine(path, c.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, detail.Image_Name);
            File.Copy(fileInfo.FullName,path,true);
        }
        public string GetImage(TransactionCameraDetail detail)
        {
            return Path.Combine(basepath,detail.Transaction_Id.ToString(),detail.Image_Name);
        }
    }
}
