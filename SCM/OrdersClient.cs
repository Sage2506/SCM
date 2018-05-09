using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SCM
{

    public class LocationChangeEventArgs : EventArgs{

        public LocationChangeEventArgs(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public double Latitude { get;  }
        public double Longitude { get; }
    }

    public class OrdersClient
    {
        private readonly HubConnection hubConnection;
        private readonly IHubProxy proxy;

		public event EventHandler OnStartedOrder;
        public event EventHandler OnWaitingOrder;
        public event EventHandler<LocationChangeEventArgs> OnChangeLocation;


        public OrdersClient()
        {
            hubConnection = new HubConnection(@"https://signalrscm.azurewebsites.net/");
            proxy = hubConnection.CreateHubProxy("PedidosHub");
        }

        /*public PedidosClient(string d){
            hubConnection = new HubConnection(@"https://signalrscm.azurewebsites.net/repartidor.html");
            proxy = hubConnection.CreateHubProxy("PedidosHub");
        }*/

        public async Task Connection(){
            await hubConnection.Start();
            proxy.On("pedidoIniciado", () => { 
                if(OnStartedOrder != null){
                    OnStartedOrder(this, new EventArgs());
                }
            });
            proxy.On("estoyEsperando", () => { 
                if(OnWaitingOrder != null){
                    OnWaitingOrder(this, new EventArgs());
                }
            });
            proxy.On("ubicacionCambio", (double lat, double lon) => {
                if(OnChangeLocation != null){
                    OnChangeLocation(this, new LocationChangeEventArgs(lat, lon));
                }
            });

        }

        public Task StartRun(string pedidoId){
            if(hubConnection.State == ConnectionState.Connected){
                return proxy.Invoke("StartRun", pedidoId);
            }
            return Task.FromResult(0);

        }

        public Task WaitOrder(string pedidoId){
            if (hubConnection.State == ConnectionState.Connected)
            {
				return proxy.Invoke("WaitOrder", pedidoId);
            }
            return Task.FromResult(0);
        }

		public Task NotifyLocation(string OrderId,double lat, double lon){
            if (hubConnection.State == ConnectionState.Connected)
            {
				return proxy.Invoke("NotifyLocation", OrderId, lat, lon);
            }
            return Task.FromResult(0);
        }

		public Task WheresMyOrder(string OrderId){
            if (hubConnection.State == ConnectionState.Connected)
            {
				return proxy.Invoke("WheresMyOrder", OrderId);
            }
            return Task.FromResult(0);           
        }
    }
}
