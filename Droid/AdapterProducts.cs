using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using SCM;


namespace SCM.Droid 
{
    public class AdapterProducts : BaseAdapter<Product>
    {
        private Activity context;
        private List<Product> items;

		public AdapterProducts(Activity context,List<Product> products)
        {
            this.context = context;
            this.items = products;
        }

        public override Product this[int position] => items[position];

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
                view = context.LayoutInflater.Inflate(Resource.Layout.itemProducto,null);
            view.FindViewById<TextView>(Resource.Id.txvDescripcion).Text = item.description;
            view.FindViewById<TextView>(Resource.Id.txvPrecio).Text ="$"+ item.price.ToString();
            Glide.With(context)
                 .Load(item.imagen)
                 .Into(view.FindViewById<Refractored.Controls.CircleImageView>(Resource.Id.imgProducto));       
            return view;
        }
    }
}
