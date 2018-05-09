using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Android.App;
using Android.Views;
using Android.Widget;
using SCM;
namespace SCMRepartidor
{
    public class AdapterOrders : BaseAdapter<Order>
    {
        private Activity context;
        private List<Order> items;

        public AdapterOrders(Activity context, List<Order> productos)
        {
            this.context = context;
            this.items = productos;
        }

        public override Order this[int position] => items[position];

        public override int Count => items.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Contract.Ensures(Contract.Result<View>() != null);
            var item = items[position];
            View view = convertView;
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.itemPedidos, null);
			view.FindViewById<TextView>(Resource.Id.txvCliente).Text = "Cliente: "+item.Client;
			view.FindViewById<TextView>(Resource.Id.txvProducto).Text ="Auto: "+item.DescriptionProduct;
			view.FindViewById<TextView>(Resource.Id.txvEstado).Text = "Estado: "+item.State;
             return view;
        }

       
    }
}
