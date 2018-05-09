
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;

namespace SCM.Droid
{
    [Activity(Label = "Hacer Pedido")]
    public class MakeOrder : Activity
    {
        Product prod;
		int position;
		TextView tvProduct;
		EditText etPhone, etClient;
		Button btnMakeOrder;
        IOrdersRepository repo;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RealizarPedido);
            position = Intent.GetIntExtra("Posicion", -1);
            if (position == -1)
            {
                Finish();
            }
            repo = new ApiOrdersRepository();
            prod = MainActivity.productos[position];
            Glide.With(this)
                 .Load(prod.imagen)
                 .Into(FindViewById<ImageView>(Resource.Id.imgProductoPedido));
			FindViewById<TextView>(Resource.Id.txvPrecioPedido).Text = "Precio: $"+prod.price.ToString();
            tvProduct = FindViewById<TextView>(Resource.Id.txvDescripcionPedido);
            etClient = FindViewById<EditText>(Resource.Id.edtCliente);
            etPhone = FindViewById<EditText>(Resource.Id.edtTelefono);
            tvProduct.Text = prod.description;
           
            btnMakeOrder = FindViewById<Button>(Resource.Id.btnHacerPedido);
			btnMakeOrder.Click += BtnMakeOrder_Click;

            etClient.Text = MainActivity.client;
            etPhone.Text = MainActivity.phone;
        }

        void BtnMakeOrder_Click(object sender, EventArgs e)
        {
			var order = new Order();
            order.Client = etClient.Text;
            order.Phone = etPhone.Text;
            order.Product = prod.code;
            order.OrderDate = DateTime.Now.ToLocalTime();
            order.ProductPrice = prod.price;
            order.DescriptionProduct = prod.description;

			repo.StartOrder(order);
            Toast.MakeText(this,"Pedido realizado",ToastLength.Long).Show();

            MainActivity.phone = etPhone.Text;
            MainActivity.client = etClient.Text;
            Finish();
            Toast.MakeText(this, order.OrderDate.ToString(), ToastLength.Long).Show();
        }


    }
}
