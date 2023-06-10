namespace Libro.Presentation.Dtos.Book
{
    public class UpdateBookDto : BookDto
    {
        public Guid Id { get; set; }
        public bool? IsAvailable { get; set; }

    }
}
