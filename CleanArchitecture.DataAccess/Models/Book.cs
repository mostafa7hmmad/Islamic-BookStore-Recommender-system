using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.DataAccess.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Author { get; set; }
        public string? CoverImagePath { get; set; }

        [Required]
        public int BookCategoryId { get; set; }

        [ForeignKey("BookCategoryId")]
        public BookCategory BookCategory { get; set; }

        // --- NEW PROPERTIES REQUIRED BY THE ONNX MODEL ---
        public int AuthorId { get; set; }
        public int TopicId { get; set; }
        public string? ImageUrl { get; set; }
    }
}