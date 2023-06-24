using Libro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{

    public class ReadingListRepository : IReadingListRepository
    {
        private readonly ApplicationDbContext _context;

        public ReadingListRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<ReadingList>, int)> GetReadingListsByUserAsync(Guid UserId, int PageNumber, int Count)
        {

            var readingListsCount = await _context.ReadingLists
                .Where(a => a.UserId == UserId).CountAsync();

            var readingLists = await _context.ReadingLists
                 .Where(a => a.UserId == UserId).Skip(PageNumber * Count).Take(Count).ToListAsync();

            var NumberOfPages = 1;
            if (readingListsCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)readingListsCount / Count);

            return (readingLists, NumberOfPages);
        }

        public async Task<ReadingList> GetReadingListByUserAsync(Guid UserId, Guid ReadingListId)
        {
            var readingList = await _context.ReadingLists
                 .Where(a => a.UserId == UserId)
                 .FirstOrDefaultAsync(a => a.Id == ReadingListId);

            return readingList;


        }
        public async Task<(ReadingList, int)> GetReadingListWithBooksAsync(Guid UserId, Guid ReadingListId, int PageNumber, int Count)
        {

            var booksCount = await _context.BookReadingLists
                .Where(a => a.ReadingListId == ReadingListId)
                .CountAsync();

            var readingList = await _context.ReadingLists
                .Include(
                 a => a.Books
                 .Skip(PageNumber * Count)
                 .Take(Count)
                )
                .Where(a => a.UserId == UserId)
                .FirstOrDefaultAsync(a => a.Id == ReadingListId);

            var NumberOfPages = 1;
            if (booksCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)booksCount / Count);

            return (readingList, NumberOfPages);
        }
        public async Task CreateReadingListAsync(ReadingList readingList)
        {

            _context.ReadingLists.AddAsync(readingList);

        }
        public void UpdateReadingList(ReadingList readingList)
        {

            _context.ReadingLists.Update(readingList);

        }
        public void RemoveReadingList(ReadingList readingList)
        {

            _context.ReadingLists.Remove(readingList);

        }
        public async Task AddBookToReadingList(BookReadingList bookReadingList)
        {

            await _context.BookReadingLists.AddAsync(bookReadingList);

        }
        public void RemoveBookFromReadingList(BookReadingList bookReadingList)
        {

            _context.BookReadingLists.Remove(bookReadingList);

        }
        public async Task<bool> ContainsTheBook(BookReadingList bookReadingList)
        {

            return await _context.BookReadingLists.ContainsAsync(bookReadingList);

        }

    }
}
