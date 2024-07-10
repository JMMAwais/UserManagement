using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

      
        public string Description { get; set; }

        public double price { get; set; }

    }
}
