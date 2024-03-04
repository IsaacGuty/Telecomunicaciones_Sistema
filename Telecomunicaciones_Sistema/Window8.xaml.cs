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
    /// Lógica de interacción para Window8.xaml
    /// </summary>
    public partial class Window8 : Window
    {
        public event EventHandler EmpleadoAgregado;

        public event EventHandler EmpleadoModificado;

        private List<Empleados> empleados;

        public Empleados NuevoEmpleado { get; private set; }


        public Window8()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            empleados = new List<Empleados>();
        }

        public Window8(Window6.Empleados empleadoSeleccionado)
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            empleados = new List<Empleados>();
            this.empleadoSeleccionado = empleadoSeleccionado;
            MostrarDetallesEmpleado();
        }

        private SqlConnection Conn;
        private Window6.Empleados empleadoSeleccionado;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            NuevoEmpleado = new Empleados
            {
                ID_Empleado = txtIDE.Text,
                Nombre_E = txtNombreE.Text,
                Apellido_E = txtApellidoE.Text,
                Teléfono_E = Convert.ToDecimal(txtTelefonoE.Text),
                Correo_E = txtCorreoE.Text,
                ID_Dirección = txtDireccionE.Text,
                Puesto = txtPuesto.Text,
                Estado = txtEstado.Text
            };
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Empleados WHERE ID_Empleado = @ID_Empleado", Conn);
                cmd.Parameters.AddWithValue("@ID_Empleado", NuevoEmpleado.ID_Empleado);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    cmd = new SqlCommand("UPDATE Empleados SET Nombre_E = @Nombre_E, Apellido_E = @Apellido_E, Teléfono_E = @Teléfono_E, Correo_E = @Correo_E, ID_Dirección = @ID_Dirección, Puesto = @Puesto, Estado = @Estado WHERE ID_Empleado = @ID_Empleado", Conn);
                }
                else
                {
                    cmd = new SqlCommand("INSERT INTO Empleados (ID_Empleado, Nombre_E, Apellido_E, Teléfono_E, Correo_E, ID_Dirección, Puesto, Estado) VALUES (@ID_Empleado, @Nombre_E, @Apellido_E, @Teléfono_E, @Correo_E, @ID_Dirección, @Puesto, @Estado)", Conn);
                }

                    cmd.Parameters.AddWithValue("@ID_Empleado", NuevoEmpleado.ID_Empleado);
                    cmd.Parameters.AddWithValue("@Nombre_E", NuevoEmpleado.Nombre_E);
                    cmd.Parameters.AddWithValue("@Apellido_E", NuevoEmpleado.Apellido_E);
                    cmd.Parameters.AddWithValue("@Teléfono_E", NuevoEmpleado.Teléfono_E);
                    cmd.Parameters.AddWithValue("@Correo_E", NuevoEmpleado.Correo_E);
                    cmd.Parameters.AddWithValue("@ID_Dirección", NuevoEmpleado.ID_Dirección);
                    cmd.Parameters.AddWithValue("@Puesto", NuevoEmpleado.Puesto);
                    cmd.Parameters.AddWithValue("@Estado", NuevoEmpleado.Estado);
                    cmd.ExecuteNonQuery();

                    if (count > 0)
                    {
                        MessageBox.Show("Empleado modificado correctamente.");
                    }
                    else
                    {
                        MessageBox.Show("Empleado agregado correctamente.");
                    }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al modificar/agregar el empleado: " + ex.Message);
            }
            finally
            {
                Conn.Close();
            }

            OnEmpleadoModificado();

            this.Close();
        }

        private void OnEmpleadoAgregado()
        {
            EmpleadoAgregado?.Invoke(this, EventArgs.Empty);
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Window6 frmPr = new Window6();
        }

        private void GuardarCambios()
        {

        }

        private void OnEmpleadoModificado()
        {
            EmpleadoModificado?.Invoke(this, EventArgs.Empty);
        }

        private void MostrarDetallesEmpleado()
        {
            txtIDE.Text = empleadoSeleccionado.ID_Empleado;
            txtNombreE.Text = empleadoSeleccionado.Nombre_E;
            txtApellidoE.Text = empleadoSeleccionado.Apellido_E;
            txtCorreoE.Text = empleadoSeleccionado.Correo_E;
            txtTelefonoE.Text = empleadoSeleccionado.Teléfono_E;
            txtDireccionE.Text = empleadoSeleccionado.ID_Dirección;
            txtPuesto.Text = empleadoSeleccionado.Puesto;
            txtEstado.Text = empleadoSeleccionado.Estado;
        }
    }
}
