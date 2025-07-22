using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <-- ADD THIS USING

namespace CleanArchitecture.DataAccess.Models
{
    public class BookCategory
    {
        [Key]
        // ADD THIS ATTRIBUTE. It tells EF Core that YOU will provide the ID value.
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}