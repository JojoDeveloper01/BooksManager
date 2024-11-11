using System.ComponentModel.DataAnnotations;

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

    // This property is for uploading the image file
    public IFormFile ImagemCaminho { get; set; }

    // Add this property if you want to store the file path in the database
    public string ImagemPath { get; set; }
}
