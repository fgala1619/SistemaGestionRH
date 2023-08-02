using System;
using System.Collections.Generic;

namespace SistemaGestionRH.Models
{
    public partial class Trabajador
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaContratacion { get; set; }
        public int? IdRol { get; set; }
        public decimal SalarioInicial { get; set; }
        public decimal? SalarioIncrementado { get; set; }
        public DateTime? FechaIncrementoSalarial { get; set; }

        public virtual Rol? IdRolNavigation { get; set; }
    }
}
