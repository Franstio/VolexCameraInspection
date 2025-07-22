using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolexCameraInspection.Models
{
    public class ConfigModel
    {
        public string dbpath { get; set; } = string.Empty;
        public string PLC_IP { get; set; } = string.Empty;
        public string FTP_PATH { get; set; } = string.Empty;
        public string PLC_PORT { get;set; } = string.Empty;
        public ConfigModel()
        {

        }
        public ConfigModel(string _dbpath, string _PLC_IP,string _PLC_PORT, string _FTP_PATH)
        {
            dbpath = _dbpath;
            PLC_IP = _PLC_IP;
            PLC_PORT = _PLC_PORT;
            FTP_PATH = _FTP_PATH;
        }
    }
}
