using System;
namespace SCM
{
    public class Order
    {
        public Order()
        {
        }
		public string OrderId { get; set; }
        public string OrderDetail { get; set; }
        public string Client { get; set; }
        public string Phone { get; set; }
        public string Product { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal ProductPrice { get; set; }
        public string DescriptionProduct { get; set; }
        public string State { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public string Precition { get; set; }
    }
}
