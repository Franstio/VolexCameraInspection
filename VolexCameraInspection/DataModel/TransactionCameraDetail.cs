using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolexCameraInspection.DataModel
{
    public record TransactionCameraDetail(Guid Transaction_Id,string Code,string Type,bool Output,string Image_Name,DateTime Record_Date);
}
