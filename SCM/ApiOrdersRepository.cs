using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SCM
{
    public class ApiOrdersRepository : IOrdersRepository
    {
        HttpClient client;
        private readonly string ApiLocation = @"https://14171224.azurewebsites.net/api/Pedidos";
        public ApiOrdersRepository()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

		public async Task<bool> UpdateLocation(Order nuevaUbicacion)
        {
            var uri = new Uri(string.Format(ApiLocation + "/{0}/Ubicacion/", nuevaUbicacion.OrderId));
            var json = JsonConvert.SerializeObject(nuevaUbicacion);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(uri, content);

            if ((response.IsSuccessStatusCode))
            {
                var contentResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var ped = JsonConvert.DeserializeObject<Order>(contentResponse);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ForwardOrder(string pedidoid, string nuevoEstado)
        {
            var uri = new Uri(string.Format(ApiLocation + "/{0}/Forward/{1}",pedidoid,nuevoEstado));

            HttpContent content = new StringContent(string.Empty,Encoding.UTF8,"application/json");

           
            HttpResponseMessage response = await client.PutAsync(uri, content);

            if ((response.IsSuccessStatusCode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CancelOrder(string pedidoid)
        {
            var uri = new Uri(string.Format(ApiLocation + "/{0}", pedidoid));

            //HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.DeleteAsync(uri);

            if ((response.IsSuccessStatusCode))
            {
                return true;
            }
            else
            {
                return false;
            }        
		}

        public async Task<bool> CompleteOrder(string pedidoid)
        {
            var uri = new Uri(string.Format(ApiLocation + "/{0}/Completar", pedidoid));

            HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> StartOrder(Order nuevoPedido)
        {
            //var uri = new Uri(string.Format(ApiLocation + "/{0}", nuevoPedido.OrderId));
            var uri = new Uri(string.Format(ApiLocation));

            var json = JsonConvert.SerializeObject(nuevoPedido);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(uri,content);

            if((response.IsSuccessStatusCode)){
                var contentResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var ped = JsonConvert.DeserializeObject<Order>(contentResponse);
                return ped.OrderDetail;
            }else{
                return string.Empty;
            }
        }

        public async Task<Order> GetOrder(string id)
        {
            var uri = new Uri(string.Format(ApiLocation+"/id/{0}",id));

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var lista = JsonConvert.DeserializeObject<Order>(content);
                return lista;
            }
            return null;
        }

        public async Task<Order> GetOrderByClient(string telefono)
        {
            var uri = new Uri(string.Format(ApiLocation + "/client/{0}", telefono));

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var lista = JsonConvert.DeserializeObject<List<Order>>(content);
                //return lista.FirstOrDefault(p => p.Estado == "Cancelado");

                return lista.OrderByDescending(p => p.OrderDate).FirstOrDefault();
            }
            return null;
        }

        public async Task<List<Order>> GetOrderByDate(DateTime fecha)
        {

            var uri = new Uri(string.Format(ApiLocation + "/{0:yyyy-MM-dd}", fecha));

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var lista = JsonConvert.DeserializeObject<List<Order>>(content);
                return lista.OrderByDescending(p => p.OrderDate).ToList();
            }
            return null;
        }
    }
}
