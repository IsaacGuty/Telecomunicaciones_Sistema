﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Ordenes
    {
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Dirección { get; set; }

        public decimal Teléfono { get; set; }

        public string Servicio { get; set; }

        public string Tipo_Servicio { get; set; }

        public string Nombre_Empleado { get; set; }

        public Ordenes()
        {

        }

        public Ordenes(string oNombre, string oApellido, string oDirección, decimal oTeléfono, string oServicio, string oTipo_Servicio, string oNombre_Empleado)
        {
            this.Nombre = oNombre;
            this.Apellido = oApellido;
            this.Dirección = oDirección;
            this.Teléfono = oTeléfono;
            this.Servicio = oServicio;
            this.Tipo_Servicio = oTipo_Servicio;
            this.Nombre_Empleado = oNombre_Empleado;
        }
    }
}
