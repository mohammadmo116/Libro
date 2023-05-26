using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookTransactionRepository
    {
        Task ReserveBook(BookTransaction bookTransaction);
    }
}