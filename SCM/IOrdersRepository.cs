using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCM
{
    public interface IOrdersRepository
    {
        Task<Order> GetOrder(string id);
        Task<List<Order>> GetOrderByDate(DateTime fecha);
        Task<Order> GetOrderByClient(string telefono);
        Task<String> StartOrder(Order nuevoPedido);
        Task<bool> ForwardOrder(string pedidoid, string nuevoEstado);
        Task<bool> CancelOrder(string pedidoid);
        Task<bool> CompleteOrder(string pedidoid);
		Task<bool> UpdateLocation(Order nuevaUbicacion);

    }
    public class ActualizaUbicacionModel
    {
        public string PedidoId { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Precision { get; set; }
    }


}
