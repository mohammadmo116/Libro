using Libro.Domain.Entities;

namespace Libro.Infrastructure.Repositories
{
    public interface IReadingListRepository
    {
        Task<(ReadingList, int)> GetReadingListWithBooksAsync(Guid UserId, Guid ReadingListId, int PageNumber, int Count);
        Task<ReadingList> GetReadingListByUserAsync(Guid UserId, Guid ReadingListId);
        Task CreateReadingListAsync(ReadingList readingList);
        void UpdateReadingList(ReadingList readingList);
        void RemoveReadingList(ReadingList readingList);
 
    }
}