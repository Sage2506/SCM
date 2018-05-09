
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Util;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SCM;

namespace SCMRepartidor
{
    [Activity(Label = "DetallePedido")]
    public class OrderDetail : Activity, Android.Locations.ILocationListener
    {
		private TextView tvDate, tvPrice, tvState, tvProduct, tvClient, tvPhone;
		private Button btnLocation, btnDeliever, btnDontDeliever,btnStart,btnForward;
        IOrdersRepository repo;
		Order ord;
		String OrderId;
		Double latitude, longitude;
        Location currentLocation;
        LocationManager locationManager;
        string _locationProvider;
        static readonly string TAG = "X:" + typeof(OrderDetail).Name;

        OrdersClient pedClient;

        public async void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if(currentLocation == null){
                //No se pudo determinar ubicacion..."
                latitude = 0;
                longitude = 0;
            }else{
                //Ubicaccion.text = string.Format("{0:f6},{1:f6}", currentLocation.Latitude, currentLocation.Longitude);
                latitude = currentLocation.Latitude;
                longitude = currentLocation.Longitude;

				var ubi = new Order()
                {
					OrderId = OrderId,
					Latitude = latitude,
                    Longitude = longitude,
                    Precition = "P"
                };
                await repo.UpdateLocation(ubi);

                if(pedClient != null){
					await pedClient.NotifyLocation(ord.OrderId, latitude, longitude);
                }

            }


        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }

        private void InitLocation(){
            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Coarse
            };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService,true);
            if(acceptableLocationProviders.Any()){
                _locationProvider = acceptableLocationProviders.First();
            }else{
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "using " + _locationProvider + ".");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DetallePedido);
            //locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            InitLocation();
            repo = new ApiOrdersRepository();
            int posicion = Intent.GetIntExtra("Posicion", -1);
            if (posicion == -1)
            {
                Finish();
            }
            OrderId = MainActivity.Orders[posicion].OrderId;
            tvProduct = FindViewById<TextView>(Resource.Id.txvDescripcionDetalle);
            tvDate = FindViewById<TextView>(Resource.Id.txvFechaDetalle);
            tvState = FindViewById<TextView>(Resource.Id.txvEstadoDetalle);
            tvPrice = FindViewById<TextView>(Resource.Id.txvPrecioDetalle);
            btnLocation = FindViewById<Button>(Resource.Id.btnUbicacion);
            btnDeliever = FindViewById<Button>(Resource.Id.btnEntregarPedido);
            btnDontDeliever = FindViewById<Button>(Resource.Id.btnNoEntregarPedido);
            btnStart = FindViewById<Button>(Resource.Id.btnIniciarPedido);
            btnForward = FindViewById<Button>(Resource.Id.btnAvanzarPedido);
            tvClient = FindViewById<TextView>(Resource.Id.txvClienteDetalle);
            tvPhone = FindViewById<TextView>(Resource.Id.txvTelefonoDetalle);

			btnStart.Click += BtnStart_Click;
            btnForward.Click += BtnForward_Click;
            /*
            btnEntregar.Click += (object sender, EventArgs e) =>
            {
                repo.AvanzarPedido(idPedido, "Entregado");
                OnResume();
            };*/
            btnDontDeliever.Click += (object sender, EventArgs e) => {
                repo.ForwardOrder(OrderId, "NoEntregado");
                OnResume();
            };
            btnLocation.Click += (object sender, EventArgs e) => {
                Toast.MakeText(this,"Ya casi llego jefe, vaya sacando la feria",ToastLength.Long).Show();
               
            };

			pedClient = new OrdersClient();


            // Create your application here
            pedClient.OnWaitingOrder += (object sender, EventArgs e) => {
                Toast.MakeText(this, "Está esperando..", ToastLength.Long).Show();
            };

           
        }

		protected async override void OnResume()
		{
            base.OnResume();

            locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);

            ord = await repo.GetOrder(OrderId);

            tvProduct.Text = ord.DescriptionProduct;
            tvDate.Text = ord.OrderDate.ToString("dd/MM/yyyy");
            tvPrice.Text = ord.ProductPrice.ToString();
            tvClient.Text = "Cliente: " + ord.Client;
            tvPhone.Text = "Telefono: " + ord.Phone;


            tvState.Text = ord.State;



            if(ord.State == "New"){
                btnStart.Visibility = ViewStates.Visible;
                btnLocation.Visibility = ViewStates.Gone;
                btnForward.Visibility = ViewStates.Gone;
                btnDeliever.Visibility = ViewStates.Gone;
                btnDontDeliever.Visibility = ViewStates.Gone;
            }else if(ord.State == "EnProceso" || ord.State == "CortandoVegetales" || ord.State == "CocinandoCarne" || ord.State == "ArmandoPlatillo" ){
                btnStart.Visibility = ViewStates.Gone;
                btnLocation.Visibility = ViewStates.Gone;
                btnForward.Visibility = ViewStates.Visible;
                btnDeliever.Visibility = ViewStates.Gone;
                btnDontDeliever.Visibility = ViewStates.Gone;
            }else if(ord.State == "EnTransito"){
                btnStart.Visibility = ViewStates.Gone;
                btnLocation.Visibility = ViewStates.Gone;
                btnForward.Visibility = ViewStates.Gone;
                btnDeliever.Visibility = ViewStates.Gone;
                btnDontDeliever.Visibility = ViewStates.Visible;
                await pedClient.Connection();
				await pedClient.StartRun(ord.OrderId);
            }else if(ord.State == "Entregado" || ord.State == "NoEntregado"){
                btnStart.Visibility = ViewStates.Gone;
                btnLocation.Visibility = ViewStates.Gone;
                btnForward.Visibility = ViewStates.Gone;
                btnDeliever.Visibility = ViewStates.Gone;
                btnDontDeliever.Visibility = ViewStates.Gone;
                Toast.MakeText(this, "Su pedido ya ha sido entregado o no.", ToastLength.Long).Show();
            }else if(ord.State == "Cancelado"){
                btnStart.Visibility = ViewStates.Gone;
                btnLocation.Visibility = ViewStates.Gone;
                btnForward.Visibility = ViewStates.Gone;
                btnDeliever.Visibility = ViewStates.Gone;
                btnDontDeliever.Visibility = ViewStates.Gone;
            }  


		}

		protected override void OnPause()
		{
            base.OnPause();
            //locationManager.
		}

		void BtnStart_Click(object sender, EventArgs e)
		{
                repo.ForwardOrder(OrderId, "EnProceso");
                OnResume();
		}


		async void BtnForward_Click(object sender, EventArgs e)
        {
            if (ord.State == "New")
            {
				await repo.ForwardOrder(OrderId, "EnProceso");
            }
			else if (ord.State == "EnProceso")
            {
				await repo.ForwardOrder(OrderId, "Armando");
            }
			else if (ord.State == "Armando")
            {
				await repo.ForwardOrder(OrderId, "Ensamblando");
            }
			else if (ord.State == "Ensamblando")
            {
				await repo.ForwardOrder(OrderId, "Pintando");

            }
			else if (ord.State == "Pintando")
            {
				await repo.ForwardOrder(OrderId, "EnTransito");

            }
			else if (ord.State == "Pintando")
            {
                await repo.ForwardOrder(OrderId, "Entregado");

            }

            OnResume();
        }

	}
}
