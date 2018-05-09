
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
    [Activity(Label = "DetallePedido")]
    public class OrderDetail  : Activity
    {
        private TextView txvFecha, txvPrecio, txvEstado, txvProducto,txvUbicacion;
		private Button btnCancel,btnLocation,btnGet;
        LinearLayout layUbicacion;
        IOrdersRepository repo;
		Order ord = MainActivity.p;

		OrdersClient ordClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.VerPedido);
            repo = new ApiOrdersRepository();

            txvProducto = FindViewById<TextView>(Resource.Id.txvDescripcionDetalle);
            txvFecha = FindViewById<TextView>(Resource.Id.txvFechaDetalle);
            txvEstado = FindViewById<TextView>(Resource.Id.txvEstadoDetalle);
            txvPrecio = FindViewById<TextView>(Resource.Id.txvPrecioDetalle);
            btnCancel = FindViewById<Button>(Resource.Id.btnCancelarPedido);
            btnLocation = FindViewById<Button>(Resource.Id.btnDondeEstasPedido);
            btnGet = FindViewById<Button>(Resource.Id.btnPedidoRecibido);
            txvUbicacion = FindViewById<TextView>(Resource.Id.txvUbicacion);
            layUbicacion = FindViewById<LinearLayout>(Resource.Id.layUbicacion);
            btnCancel.Click += BtnCancelar_Click;

            layUbicacion.Visibility = ViewStates.Gone;

            ordClient = new OrdersClient();

            btnLocation.Click +=  (object sender, EventArgs e) => {
                
            };

            btnGet.Click += (object sender, EventArgs e) => {
				repo.ForwardOrder(ord.OrderDetail, "Entregado");
                Toast.MakeText(this, "Su pedido ha sido completado.", ToastLength.Long).Show();
                Finish();
            };


            ordClient.OnChangeLocation += PedClient_OnUbicacionCambio;


        }

         

        protected override async void OnResume()
		{
            base.OnResume();
            txvProducto.Text = ord.DescriptionProduct;
            txvFecha.Text = ord.OrderDate.ToString("dd/MM/yyyy");
            txvEstado.Text = ord.State;
            txvPrecio.Text = ord.ProductPrice.ToString();
            //buscaImagen(ord.Product);

            //Estados = Nuevo, En Proceso, Cortando Vegetales, Cocinando Carne,
            //          Armando platillo, En transito, Entregado, No entregado
            if(ord.State == "Nuevo"){
                btnGet.Visibility = ViewStates.Gone;
                btnLocation.Visibility = ViewStates.Gone;
                layUbicacion.Visibility = ViewStates.Gone;
            }else if(ord.State == "EnProceso"){
                btnLocation.Visibility = ViewStates.Gone;
                btnCancel.Visibility = ViewStates.Gone;
                btnGet.Visibility = ViewStates.Gone;
                layUbicacion.Visibility = ViewStates.Gone;
            }else if(ord.State == "Armando"){
                btnLocation.Visibility = ViewStates.Gone;
                btnCancel.Visibility = ViewStates.Gone;
                btnGet.Visibility = ViewStates.Gone;
                layUbicacion.Visibility = ViewStates.Gone;
            }else if(ord.State == "Ensamblando"){
                btnLocation.Visibility = ViewStates.Gone;
                btnCancel.Visibility = ViewStates.Gone;
                btnGet.Visibility = ViewStates.Gone;
                layUbicacion.Visibility = ViewStates.Gone;
            }else if(ord.State == "Pintando"){
                btnLocation.Visibility = ViewStates.Gone;
                btnCancel.Visibility = ViewStates.Gone;
                btnGet.Visibility = ViewStates.Gone;
                layUbicacion.Visibility = ViewStates.Gone;
            }else if(ord.State == "EnTransito"){
                btnLocation.Visibility = ViewStates.Gone;
                btnCancel.Visibility = ViewStates.Gone;
                btnGet.Visibility = ViewStates.Visible;
                layUbicacion.Visibility = ViewStates.Visible;
                await ordClient.Connection();
                await ordClient.WaitOrder(ord.OrderDetail);
            }else if(ord.State == "Entregado" || ord.State == "NoEntregado"){
                btnLocation.Visibility = ViewStates.Gone;
                btnCancel.Visibility = ViewStates.Gone;
                btnGet.Visibility = ViewStates.Gone;
                layUbicacion.Visibility = ViewStates.Gone;
                Toast.MakeText(this, "Su pedido ya ha sido completado.", ToastLength.Long).Show();
            } 
		}

        void PedClient_OnUbicacionCambio(object sender, LocationChangeEventArgs e)
        {
            RunOnUiThread(() => txvUbicacion.Text = string.Format("{0}, {1}", e.Latitude, e.Longitude));

        }


		bool buscaImagen(string codPro){
            foreach(var pr in MainActivity.productos){
				if(pr.code == codPro){
                    Glide.With(this)
                            .Load(pr.imagen)
					     .Into(FindViewById<Refractored.Controls.CircleImageView>(Resource.Id.imgProductoPedido));
					return true;
                }
            }
			return false;
        }


        void BtnCancelar_Click(object sender, EventArgs e)
        {
			repo.CancelOrder(ord.OrderDetail);
            Finish();
            Toast.MakeText(this, "Pedido Cancelado", ToastLength.Long);
        }


	}
}
