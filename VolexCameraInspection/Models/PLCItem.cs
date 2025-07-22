using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolexCameraInspection.Models
{
    public class PLCItem
    {
        public enum PLCItemType
        {
            WR,
            RD
        }

        public PLCItemType Type { get; set; }
        public string Command { get; set; } = string.Empty;
        public int Value { get;set; }
        public string Description { get; set; } = string.Empty;


        public PLCItem(PLCItemType type, string command, int val, string description)
        {
            Type = type;
            Command = command;
            Value = val;
            Description = description;
        }
        public string GetCommand() => $"{Type.ToString()} {Command}{(Type == PLCItemType.WR ? $" {Value}"  : "")}\r\n";
        
    }
}
