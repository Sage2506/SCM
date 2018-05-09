using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Content;
using Android.Views;

namespace SCM.Droid
{
    [Activity(Label = "SCM", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        
        IProductosRepository repo;
        IOrdersRepository repoPedidos;
        public static List<Product> productos;
        public static List<Order> pedidos;
        public static string client, phone;
        public static Order p;
        ListView lista;
        AdapterProducts adapter;
        Button btnVerPedido;
        Dialog dialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            lista = FindViewById<ListView>(Resource.Id.lvProductos);
            repo = new ApiProductosRepository();
            repoPedidos = new ApiOrdersRepository();
            //productos = await repo.LeerProducto();
            lista.ItemClick += Lista_ItemClick;
            btnVerPedido = FindViewById<Button>(Resource.Id.btnVerPedidos);
            btnVerPedido.Click += BtnVerPedido_Click1;
            // Get our button from the layout resource,
            // and attach an event to it
            mostrarDialogo();
            pedidos = new List<Order>();



        }

        protected async override void OnResume()
        {
            base.OnResume();
            if (lista != null)
            {
                productos = await repo.GetProduct();

                adapter = new AdapterProducts(this, productos);
                lista.Adapter = adapter;
                ((BaseAdapter)lista.Adapter).NotifyDataSetChanged();
            }
        }

        void Lista_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(MakeOrder));
            //TODO:Mandar la posicion para manejarlo con el arreglo
            intent.PutExtra("Posicion", e.Position);
            StartActivity(intent);
            //int posicion = e.Position;
            //Toast.MakeText(this,"Seleccionado: "+productos[posicion].Descripcion,ToastLength.Short).Show();
        }

        async void BtnVerPedido_Click1(object sender, System.EventArgs e)
        {

            p = await repoPedidos.GetOrderByClient(phone);
            if(p != null){
                pedidos.Add(p);
            }
          
            if (pedidos.Count > 0)
            {
				var intent = new Intent(this, typeof(OrderDetail));
                //intent.PutExtra("Posicion", e.Position);
                StartActivity(intent);
            }else{
                Toast.MakeText(this, "Lo sentimos. No tiene pedidos.\nLo invitamos a realizar uno.", ToastLength.Long).Show();
            }
        }

        public void mostrarDialogo()
        {
            // con este tema personalizado evitamos los bordes por defecto
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            //alert.SetTitle("Confirm delete");
            //alert.SetMessage("Lorem ipsum dolor sit amet, consectetuer adipiscing elit.");

            LayoutInflater inflater = this.LayoutInflater;
            View v = inflater.Inflate(Resource.Layout.dialogDatos, null);
            alert.SetView(v);
            alert.SetCancelable(false);

            Button aceptar = v.FindViewById<Button>(Resource.Id.btnGuardarDialog);
            EditText edtCliente =v.FindViewById<EditText>(Resource.Id.edtDialogNombre);
            EditText edtCelular = v.FindViewById<EditText>(Resource.Id.edtDialogCeluar);

            aceptar.Click += (sender, e) => {
                client = edtCliente.Text;
                phone = edtCelular.Text;
                dialog.Dismiss();
               
            };
            dialog = alert.Create();
            dialog.Show();
    }

    }
}

