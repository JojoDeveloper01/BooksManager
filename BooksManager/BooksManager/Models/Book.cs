using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for [NotMapped]
using Microsoft.AspNetCore.Http;

namespace BooksManager.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string? Titulo { get; set; }

        [Required]
        [StringLength(100)]
        public string? Autor { get; set; }

        [Required]
        [StringLength(200)]

        public string? Description { get; set; }

        [Required]
        public int AnoPublicacao { get; set; }

        // This property is for uploading the image file; it should not be mapped to the database
        [NotMapped]
        public IFormFile? Imagem { get; set; }

        // Property for storing the file path in the database
        public string? ImagemPath { get; set; }
    }
}
