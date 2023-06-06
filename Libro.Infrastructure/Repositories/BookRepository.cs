using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Libro.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(List<Book>,int)> GetAllBooksAsync(int PageNumber, int Count)
        {
            var booksCount= await _context.Books.Where(b => b.IsAvailable == true).CountAsync();
            var books= await _context.Books.Where(b => b.IsAvailable == true).Skip(PageNumber * Count).Take(Count).ToListAsync();
         
            var NumberOfPages = 1;
            if (booksCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)booksCount / Count);

            return (books, NumberOfPages);

        }
        public async Task<Book> GetBookAsync(Guid BookId)
        {
            return await _context.Books.Include(a => a.Authors).FirstOrDefaultAsync(b => b.Id == BookId);
        }

        public async Task CreateBookAsync(Book book)
        {
             _context.Books.AddAsync(book);
        }

        public void UpdateBook(Book book)
        {
            _context.Books.Update(book);
        }
        public void RemoveBook(Book book)
        {
            _context.Books.Remove(book);
        }
        public async Task<(List<Book>,int)> GetSearchedBooksAsync(string? Title, string? AuthorName, string? Genre, int PageNumber, int Count) {
            var Books = _context.Books.Where(a => a.IsAvailable == true);
            if (Title is not null)
                Books = GetBooksByTitleAsync(Books, Title);

            if (Genre is not null)
                Books = GetBooksByGenreAsync(Books, Genre);

            if (AuthorName is not null)
                Books = await GetBooksByAuthorNameAsync(Books, AuthorName);

            var booksCount = await Books.CountAsync();
            var SearchedBooks = await Books.Skip(PageNumber * Count).Take(Count).ToListAsync();
            var NumberOfPages = 1;
            if (booksCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)booksCount / Count);
           
            return (SearchedBooks, NumberOfPages);
            
            
        }
        private async Task<IQueryable<Book>> GetBooksByAuthorNameAsync(IQueryable<Book>? Books, string AuthorName)
        {
           
            var AuthorIds = await _context.Authors.Where(b => b.Name.Contains(AuthorName)).Select(a => a.Id).ToListAsync();
            var BookIds = await _context.AuthorBooks.Where(a => AuthorIds.Contains(a.AuthorId)).Select(a => a.BookId).ToListAsync();
            return Books.Where(b => BookIds.Contains(b.Id));
          
        }

        private IQueryable<Book> GetBooksByGenreAsync(IQueryable<Book> Books, string Genre)
        {
             return Books.Where(b => b.Genre.Contains(Genre));
        }
        private IQueryable<Book> GetBooksByTitleAsync(IQueryable<Book> Books, string Title)
        {

            return Books.Where(b => b.Title.Contains(Title));
             
        }
        public void MakeBookNotAvailable(Book book)
        {
            book.IsAvailable = false;
            _context.Books.Update(book);
        }
        public void MakeBookAvailable(Book book)
        {
            book.IsAvailable = true;
            _context.Books.Update(book);
        }
        public async Task CreateReviewAsync(BookReview bookReview)
        {
          await _context.BookReviews.AddAsync(bookReview);
        }
        public async Task<bool> BookIsReviewedByUser(Guid UserId, Guid BookId) 
        {
            return await _context.BookReviews.Where(r => r.UserId == UserId).Where(r => r.BookId == BookId).AnyAsync();
        }
        public async Task<(List<BookReview>,int)> GetReviewsAsync(Guid BookId, int PageNumber, int Count)
        {
            var BookReviewsCount = await _context.BookReviews.Include(a => a.User).Where(r => r.BookId == BookId).CountAsync();
            var BookReviews= await _context.BookReviews
                .Include(a=>a.User)
                .Where(r=>r.BookId==BookId)
                .Skip(PageNumber * Count)
                .Take(Count)
                .ToListAsync();
            
            var NumberOfPages = 1;
            if (BookReviewsCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)BookReviewsCount / Count);

            return (BookReviews, NumberOfPages);
        }
      

    }
}

