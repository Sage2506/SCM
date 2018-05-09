using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SCM
{
    public class ApiProductosRepository : IProductosRepository
    {
        HttpClient client;
        private readonly string ApiLocation = @"https://14171224.azurewebsites.net/api/products";
        public ApiProductosRepository()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<List<Product>> GetProduct()
        {
            var uri = new Uri(string.Format(ApiLocation, string.Empty));

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<Product>>(content);
                return lista;
            }
            return null;
        }
    }
}
