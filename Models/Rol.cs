using System;
using System.Collections.Generic;

namespace SistemaGestionRH.Models
{
    public partial class Rol
    {
        public Rol()
        {
            Trabajadors = new HashSet<Trabajador>();
        }

        public int IdRol { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<Trabajador> Trabajadors { get; set; }
    }
}
