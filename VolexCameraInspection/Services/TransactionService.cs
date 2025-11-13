using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolexCameraInspection.DataModel;

namespace VolexCameraInspection.Services
{
    public class TransactionService
    {
        ConfigService ConfigService = null!;

        public TransactionService(ConfigService ConfigService)
        {
            this.ConfigService = ConfigService;
        }
        private  IDbConnection GetConnection()
        {
            var con = new Interceptors.LoggingDbInterceptor.InterceptorDbConnection( new NpgsqlConnection(ConfigService.Config.dbpath));
            con.Open();
            return con;
        }
        public async Task Save(TransactionModel transaction)
        {
            using (var con = GetConnection())
            {
                using (var tr = con.BeginTransaction())
                {
                    string query = @$"insert into transaction(id,worknumber,operationusersn,partnumber,startdate,enddate,finaljudgement) values(@Id,@WorkNumber,@OperationUserSN,@PartNumber,@StartDate,@EndDate,@finalJudgement)";
                    await tr.Connection!.ExecuteAsync(query,transaction);
                    foreach (var item in transaction.Details)
                    {
                        query = $@"insert into transactiondetail(transaction_id, code, type, output, image_name, record_date) values(@Transaction_Id,@Code,@Type,@Output,@Image_Name,@Record_Date)";
                        await tr.Connection!.ExecuteAsync(query, item);
                    }
                    tr.Commit();
                }
            }
        }
    }
}
