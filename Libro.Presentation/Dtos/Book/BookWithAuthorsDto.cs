using Libro.Presentation.Dtos.Author;

namespace Libro.Presentation.Dtos.Book
{
    public class BookWithAuthorsDto : BookDto
    {
        public Guid Id { get; set; }

        public List<AuthorDto>? Authors { get; set; }

    }
}
