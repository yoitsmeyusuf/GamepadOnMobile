using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm;
using Nefarius.ViGEm.Client.Targets;

namespace SignalRServer.Hubs
{
    public class MyHub : Hub
    {
        private readonly ViGEmClient _client;
        private readonly IXbox360Controller  _controller;
        private bool _isConnected = false; // Cihazın bağlı olup olmadığını belirlemek için

        public MyHub()
        {
            _client = new ViGEmClient();
            _controller = _client.CreateXbox360Controller();
        }

        public async Task ConnectToDevice()
        {
            Console.WriteLine("baglanti");
         
                _controller.Connect();
                _isConnected = true;
                
                await Clients.All.SendAsync("DeviceConnected", true); // İstemcilere bağlantı durumunu bildir
        
        }

        public async Task SendOrientationData(double gamma)
        {
           
                // Açı değeri dönüştürme (-90 ile 90 arasından 0 ile 65535 arasına)
                int simulatedValue = (int)Math.Round((gamma + 90.0) / 180.0 * 65535);
                
                // Açı değerini simüle etme
                _controller.SetButtonsFull(1);

                Console.WriteLine(gamma);
                await Clients.All.SendAsync("OrientationDataUpdated", gamma);
            
              // Cihaz bağlı değilse hata bildir
            
        }
         
         public async Task Disconnecter()
        {
             _controller.Disconnect();
               
            
            
            _client.Dispose();
              await Clients.All.SendAsync("DiviceConnectionDisposed");
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
                _controller.Disconnect();
               
            
            
            _client.Dispose();
            await base.OnDisconnectedAsync(exception);
        }
    }
}