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
        public async Task<ReadingList> GetReadingListAsync(Guid ReadingListId)
        {
            return await _context.ReadingLists.FindAsync(ReadingListId);
        }
        public async Task<(ReadingList,int)> GetReadingListWithBooksAsync(Guid ReadingListId,int PageNumber, int Count)
        {
          
            var booksCount= await _context.BookReadingLists
                .Where(a => a.ReadingListId == ReadingListId)
                .CountAsync();

            var readingList= await _context.ReadingLists
                .Include(
                 a => a.Books
                 .Skip(PageNumber * Count)
                 .Take(Count)
                )
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
       


    }
}
