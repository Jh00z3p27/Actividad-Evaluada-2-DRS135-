using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio"), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono/WhatsApp es obligatorio")]
        public string Phone { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public bool IsBanned { get; set; } = false;
        public string BanReason { get; set; } = string.Empty;
        public DateTime? BanUntil { get; set; }

        public List<Order> Orders { get; set; } = new();
    }
}