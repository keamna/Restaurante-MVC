using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Tienda_Restaurante.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        
        public string UsuarioId { get; set; } = string.Empty;

        [ForeignKey(nameof(UsuarioId))]
        public IdentityUser? Usuario { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        [Range(1, 30)]
        public int Personas { get; set; }

        
        public string Estado { get; set; } = "Pendiente";

        public bool Pagada { get; set; } = false;

        
        public string PaymentIntentId { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = string.Empty;
    }
}
