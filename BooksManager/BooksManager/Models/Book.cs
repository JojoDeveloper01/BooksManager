using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(100)]
        public string Autor { get; set; }

        [Required]
        public int AnoPublicacao { get; set; }

        public bool Disponivel { get; set; } = true;
    }
}
