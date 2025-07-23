namespace CleanArchitecture.Services.DTOs.Book
{
    public class BookCreateDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int BookCategoryId { get; set; }
        public string? CoverImagePath { get; set; }
    }

    // BookUpdateDTO
    public class BookUpdateDTO : BookCreateDTO { }

    // BookReadDTO
    public class BookReadDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string CategoryName { get; set; }
        public string? CoverImagePath { get; set; }
    }

}
