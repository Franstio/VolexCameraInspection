using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolexCameraInspection.Interceptors
{
    public static class LoggingDbInterceptor
    {
        public class InterceptorDbConnection : IDbConnection, IDisposable
        {

            private IDbConnection conn;
            public InterceptorDbConnection(IDbConnection con)
            {
                conn = con;
            }

            private bool disposedValue;

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
            public string ConnectionString { get => conn.ConnectionString; set => conn.ConnectionString = value; }
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).

            public int ConnectionTimeout => conn.ConnectionTimeout;

            public string Database => conn.Database;

            public ConnectionState State => conn.State;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~InterceptorDbConnection()
            // {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            public IDbTransaction BeginTransaction() => conn.BeginTransaction();

            public IDbTransaction BeginTransaction(IsolationLevel il) => conn.BeginTransaction(il);

            public void ChangeDatabase(string databaseName) => 
                conn.ChangeDatabase(databaseName);

            public void Close() => conn.Close();

            public IDbCommand CreateCommand()
            {
                InterceptorDbCommand cmd = new InterceptorDbCommand(conn.CreateCommand());
                return cmd;
            }

            public void Open() => conn.Open();
        }

        public class InterceptorDbCommand : IDbCommand
        {
            private IDbCommand cmd;
            
            public InterceptorDbCommand(IDbCommand _cmd)
            {
                cmd = _cmd;
            }
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
            public string CommandText { get => cmd.CommandText; set => cmd.CommandText = value; }
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
            public int CommandTimeout { get => cmd.CommandTimeout; set => cmd.CommandTimeout = value; }
            public CommandType CommandType { get => cmd.CommandType; set => cmd.CommandType = value; }
            public IDbConnection? Connection { get => cmd.Connection; set => cmd.Connection = value; }

            public IDataParameterCollection Parameters => cmd.Parameters;

            public IDbTransaction? Transaction { get => cmd.Transaction; set => cmd.Transaction = value; }
            public UpdateRowSource UpdatedRowSource { get => cmd.UpdatedRowSource; set => cmd.UpdatedRowSource = value; }

            public void Cancel()
            {
                cmd.Cancel();
            }

            public IDbDataParameter CreateParameter()
            {
                return cmd.CreateParameter();
            }

            public void Dispose()
            {
                cmd.Dispose();
            }

            public int ExecuteNonQuery()
            {
                return cmd.ExecuteNonQuery();
            }

            public IDataReader ExecuteReader()
            {
                return cmd.ExecuteReader();
            }

            public IDataReader ExecuteReader(CommandBehavior behavior)
            {
                return cmd.ExecuteReader(behavior);
            }

            public object? ExecuteScalar()
            {
                return cmd.ExecuteScalar(); ;
            }

            public void Prepare()
            {
                cmd.Prepare();
            }
        }

    }
}
