using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VolexCameraInspection.Models;

namespace VolexCameraInspection.Services
{
    public class PLCService
    {
        public SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        private readonly ConfigService configService;
        public bool isActive { get; private set; } = false;

        public PLCService(ConfigService _configService) 
        { 
            this.configService = _configService;
        }
        TcpClient BuildTcpClient()
        {
            var client = new TcpClient();
            client.NoDelay = true;
            client.SendTimeout = 3000;
            client.ReceiveTimeout = 3000;
            return client;
        }
        private async Task<string> GetMessage(Stream stream)
        {
            byte[] buffer = new byte[64];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer);
        }
        public async Task<string> Send(PLCItem item)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                using (var tcpClient = BuildTcpClient())
                {
                    await tcpClient.ConnectAsync(IPAddress.Parse(configService.Config.PLC_IP),Convert.ToInt32(configService.Config.PLC_PORT)).WaitAsync(TimeSpan.FromMilliseconds(100));
                    isActive = true;
                    string command = item.GetCommand();
                    byte[] buffer = Encoding.ASCII.GetBytes(command);

                    await tcpClient.GetStream().WriteAsync(buffer, 0, buffer.Length);

                    await Task.Delay(100);

                    string res = await GetMessage(tcpClient.GetStream());
                    semaphoreSlim.Release();
                    return res.Replace("\0", "").Replace("\r", "").Replace("\n", "").Trim();
                }
            }
            catch (Exception _)
            {
                isActive = false;
                semaphoreSlim.Release();
                return string.Empty;
            }
        }

    }
}
