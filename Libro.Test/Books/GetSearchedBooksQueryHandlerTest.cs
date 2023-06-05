using FluentAssertions;
using Libro.Application.Books.Queries;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheoryAttribute = Xunit.TheoryAttribute;

namespace Libro.Test.Books
{
    public class GetSearchedBooksQueryHandlerTest
    {
        private readonly List<Book> _booksList;
        private readonly GetSearchedBooksQueryHandler _handler;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ILogger<GetSearchedBooksQueryHandler>> _loggerMock;
      
        public GetSearchedBooksQueryHandlerTest()
        {
            var AuthorsList = new List<Author> {
                    new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author1",
                    },
                      new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author2",
                    },
            };

            _booksList = new()
            {
                 new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "title",
                    IsAvailable = true,
                    Genre="genre",
                    Authors= AuthorsList,
               
                },
                    new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "title1",
                    IsAvailable = true,
                    Genre="genre1",
                    Authors= AuthorsList,
                },
                              new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "title2",
                    IsAvailable = true,
                    Genre="genre2",
                   Authors= AuthorsList,
                }

            };

          
        

        

            _bookRepositoryMock = new();
            _loggerMock = new();

            _handler = new(
                _loggerMock.Object,
                _bookRepositoryMock.Object
            );

        }

        [Theory]
        [InlineData(null, null, null, 1, 10)]
        [InlineData(null, null, null, 1, 1)]
        [InlineData(null, null, null, 3, 5)]
        public async Task Handle_Should_ReturnAllBooks(
            string title,
            string author,
            string genre,
            int pageNumber,
            int count)
        {
            //Arrange
            _bookRepositoryMock.Setup(
                   x => x.GetAllBooksAsync(
                       It.IsAny<int>(),
                       It.IsAny<int>()))
                   .ReturnsAsync(() =>(
                   _booksList.Skip(pageNumber * count)
                   .Take(count)
                   .ToList()!,1));

            GetSearchedBooksQuery _query = new(title, author, genre, pageNumber, count);

            //Act
            var result = await _handler.Handle(_query, default);
            //Assert
            CollectionAssert.AreEqual(_booksList.Skip(pageNumber * count).Take(count).ToList(), result.Item1);

            _bookRepositoryMock.Verify(
                x => x.GetAllBooksAsync(
                It.Is<int>(a => a == pageNumber),
                It.Is<int>(a => a == count)),
                Times.Once);

            _bookRepositoryMock.Verify(
               x => x.GetBooksByTitleAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()),
               Times.Never);

            _bookRepositoryMock.Verify(
                x => x.GetBooksByAuthorNameAsync(
                It.IsAny<List<Book>>(),
                It.IsAny<string>()),
                Times.Never);

            _bookRepositoryMock.Verify(
                x => x.GetBooksByGenreAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()),
                Times.Never);
           
             

        }

        [Theory]
        [InlineData("t", null, null, 1, 1)]
        [InlineData(null, "a", null, 3, 5)]
        [InlineData(null, null, "g", 1, 1)]
        [InlineData("t", "a", null, 3, 5)]
        [InlineData(null, "a", "g", 1, 1)]
        [InlineData("t", null, "G", 3, 5)]
        [InlineData("t", "a", "g", 1, 1)]

        public async Task Handle_Should_ReturnSearchedBooks(
          string title,
          string author,
          string genre,
          int pageNumber,
          int count)
        {
            //Arrange
            if (title is not null)
                _bookRepositoryMock.Setup(
                       x => x.GetBooksByTitleAsync(
                           It.IsAny<List<Book>>(),
                           It.IsAny<string>()))
                       .ReturnsAsync(() => _booksList);

            if (author is not null)
                _bookRepositoryMock.Setup(
                       x => x.GetBooksByAuthorNameAsync(
                           It.IsAny<List<Book>>(),
                           It.IsAny<string>()))
                       .ReturnsAsync(() => _booksList);

            if (genre is not null)
                _bookRepositoryMock.Setup(
                 x => x.GetBooksByGenreAsync(
                     It.IsAny<List<Book>>(),
                     It.IsAny<string>()))
                  .ReturnsAsync(() => _booksList);

            GetSearchedBooksQuery _query = new(title, author, genre, pageNumber, count);

            //Act
            var result = await _handler.Handle(_query, default);

            //Assert
            CollectionAssert.AreEqual(
                _booksList.Skip(pageNumber * count).Take(count).ToList()
                , result.Item1);

            _bookRepositoryMock.Verify(
                x => x.GetAllBooksAsync(
                It.Is<int>(a=>a==pageNumber),
                It.Is<int>(a=>a==count)),
                Times.Never);

            if (title is null)
            {
                _bookRepositoryMock.Verify(
                     x => x.GetBooksByTitleAsync(
                    It.IsAny<List<Book>>(),
                    It.Is<string>(a=>a==title)),
                    Times.Never);

            }
            else
            {
                _bookRepositoryMock.Verify(
                    x => x.GetBooksByTitleAsync(
                        It.IsAny<List<Book>>(),
                        It.Is<string>(a => a == title)),
                        Times.Once);
            }

            if (author is null)
            {
                _bookRepositoryMock.Verify(
                     x => x.GetBooksByAuthorNameAsync(
                        It.IsAny<List<Book>>(),
                        It.Is<string>(a => a == author)),
                        Times.Never);
            }
            else
            {
                _bookRepositoryMock.Verify(
                    x => x.GetBooksByAuthorNameAsync(
                         It.IsAny<List<Book>>(),
                         It.Is<string>(a => a == author)),
                         Times.Once);
            }


            if (genre is null)
            {
                _bookRepositoryMock.Verify(
                    x => x.GetBooksByGenreAsync(
                        It.IsAny<List<Book>>(),
                        It.Is<string>(a => a == genre)),
                        Times.Never);
            }
            else
            {
                _bookRepositoryMock.Verify(
                     x => x.GetBooksByGenreAsync(
                        It.IsAny<List<Book>>(),
                        It.Is<string>(a => a == genre)),
                        Times.Once);
            }




        }


      

        

    }
}
