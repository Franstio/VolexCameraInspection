using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolexCameraInspection.DataModel
{
    public class TransactionModel
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string WorkNumber { get; set; } = string.Empty;
        public string OperationUserSN { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public string FinalJudgement { get; set; } = string.Empty;
        public List<TransactionCameraDetail> Details { get; set; } = new List<TransactionCameraDetail>();
        public TransactionModel()
        {

        }

        public TransactionModel(string workNumber, string operationUserSN, string partNumber)
        {
            Id = Guid.NewGuid();
            WorkNumber = workNumber;
            OperationUserSN = operationUserSN;
            PartNumber = partNumber;
        }
        public TransactionModel(Guid id, string workNumber, string operationUserSN, string partNumber, DateTime startDate, DateTime endDate)
        {
            Id = id;
            WorkNumber = workNumber;
            OperationUserSN = operationUserSN;
            PartNumber = partNumber;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
