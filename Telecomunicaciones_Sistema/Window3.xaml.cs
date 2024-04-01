﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        private Window9 ventana9;
        // Estructura para representar los pagos
        public static Pagos PagoSeleccionado { get; set; }

        // Constructor de la ventana
        public Window3()
        {
            InitializeComponent();
            Conn = BD.ObtenerConexion();
            CargarDatos();
            ventana9 = new Window9();
        }

        // Clase para el diálogo de nuevo pago
        public partial class NuevoPagoDialog : Window
        {
            // Propiedades para los datos del nuevo pago
            public string ID_Pago { get; set; }
            public string ID_Cliente { get; set; }
            public decimal Monto { get; set; }
            public string ID_TpServicio { get; set; }
            public string MesPagado { get; set; }
            public decimal Fecha { get; set; }
            public string ID_Empleado { get; set; }
        }

        // Estructura para representar los pagos
        public struct Pagos
        {
            public string ID_Pago;
            public string ID_Cliente;
            public string Monto;
            public string ID_TpServicio;
            public string MesPagado;
            public string Fecha;
            public string ID_Empleado;
        }

        // Conexión a la base de datos y variable de control para la ventana principal
        private SqlConnection Conn;
        private bool isMainWindow;

        // Método para cargar los datos de los pagos desde la base de datos
        public void CargarDatos()
        {
            try
            {
                DataTable dataTable = PagoDAL.ObtenerTodosPagos();
                DatGridP.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Regresar a la ventana principal
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            if (!isMainWindow)
            {
                this.Close();
            }
        }

        // Método para solicitar la información de un nuevo pago
        private void SolicitarInformacionPago()
        {
            // Mostrar un diálogo para ingresar información de un nuevo pago
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo pago.", "Nuevo Pago", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Abrir la ventana para agregar un nuevo pago
                Window9 frmAg = new Window9();
                frmAg.PagoModificado += (s, args) => CargarDatos(); // Suscribirse al evento PagoModificado
                frmAg.Show();
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpiar el cuadro de búsqueda y recargar los datos
            txtBuscar.Clear();
            CargarDatos();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Realizar una búsqueda y mostrar los resultados en el DataGrid
            DatGridP.ItemsSource = ClienteDAL.BuscarCliente(txtBuscar.Text).DefaultView;
        }

        // Manejador del evento SelectionChanged del DataGrid
        private void DatGridP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtener y guardar la información del pago seleccionado en la estructura Pagos
            if (DatGridP.SelectedItem != null && DatGridP.SelectedItem is DataRowView)
            {
                DataRowView rowView = DatGridP.SelectedItem as DataRowView;

                PagoSeleccionado = new Pagos
                {
                    ID_Pago = rowView["ID_Pago"].ToString(),
                    ID_Cliente = rowView["ID_Cliente"].ToString(),
                    ID_TpServicio = rowView["ID_TpServicio"].ToString(),
                    Monto = rowView["Monto"].ToString(),
                    MesPagado = rowView["Mes_Pagado"].ToString(),
                    Fecha = rowView["Fecha"].ToString(),
                    ID_Empleado = rowView["ID_Empleado"].ToString()
                };
            }
            else
            {
                PagoSeleccionado = default(Pagos);
            }
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si hay un pago seleccionado
            if (!PagoSeleccionado.Equals(default(Window3.Pagos)))
            {
                // Mostrar la ventana9 para modificar el pago seleccionado
                ventana9 = new Window9(PagoSeleccionado, true); // Aquí se está pasando true como indicador de modificación
                ventana9.PagoModificado += ActualizarDatosPago;
                ventana9.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana 9
                ventana9.Show();
            }
            else
            {
                // Mostrar un mensaje si no se ha seleccionado ningún pago
                MessageBox.Show("No se ha seleccionado ningún pago.");
            }
        }

        // Método para actualizar los datos de los pagos después de una modificación
        private void ActualizarDatosPago(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            SolicitarInformacionPago();
        }
    }
}

