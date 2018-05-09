using Android.App;
using Android.Widget;
using Android.OS;
using SCM;
using System.Collections.Generic;
using System;
using Android.Content;

namespace SCMRepartidor
{
    [Activity(Label = "SCMRepartidor", MainLauncher = true)]
    public class MainActivity : Activity
    {
		ListView list;
        IOrdersRepository repo;
        public static List<Order> Orders;
        AdapterOrders adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            list = FindViewById<ListView>(Resource.Id.lvPedidos);
            repo = new ApiOrdersRepository();
			list.ItemClick += List_ItemClick;


        }

        protected async override void OnResume()
        {
            base.OnResume();
            if (list != null)
            {
                Orders = await repo.GetOrderByDate(DateTime.Now);
                adapter = new AdapterOrders(this, Orders);
                list.Adapter = adapter;
                ((BaseAdapter)list.Adapter).NotifyDataSetChanged();
            }
        }

        void List_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
			int position = e.Position;
            var enterDetail = new Intent(this, typeof(OrderDetail));
            //TODO:Mandar la posicion para manejarlo con el arreglo
            enterDetail.PutExtra("Posicion", position);
			StartActivity(enterDetail);
           
            Toast.MakeText(this, "Seleccionado: " + Orders[position].OrderDetail, ToastLength.Short).Show();
        }
    }
}

