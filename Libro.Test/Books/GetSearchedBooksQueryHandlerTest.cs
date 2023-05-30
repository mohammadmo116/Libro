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

        private readonly List<string> _books;
        private readonly GetSearchedBooksQueryHandler _handler;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ILogger<GetSearchedBooksQueryHandler>> _loggerMock;
        public GetSearchedBooksQueryHandlerTest()
        {
            _booksList = new()
            {
                 new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "title",
                    IsAvailable = true,
                    Genre="genre",
                    Authors=new List<Author> {
                    new Author(){ 
                        Id= Guid.NewGuid(),
                        Name= "author1",
                    },
                      new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author2",
                    },
                    }
               
                },
                    new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "title1",
                    IsAvailable = true,
                    Genre="genre1",
                         Authors=new List<Author> {
                    new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author11",
                    },
                      new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author12",
                    },
                    }
                },
                              new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "title2",
                    IsAvailable = true,
                    Genre="genre2",
                         Authors=new List<Author> {
                    new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author21",
                    },
                      new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author22",
                    },
                    }
                }

            };

          
        

            _books = new()
             { "test","test1","test2"};

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
        [InlineData(null, null, null, 5, 5)]
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
                   .ReturnsAsync(() => _books.Skip(pageNumber * count).Take(count).ToList());
            GetSearchedBooksQuery _query = new(title, author, genre, pageNumber, count);

            //Act
            var result = await _handler.Handle(_query, default);
            //Assert
            CollectionAssert.AreEqual(_books.Skip(pageNumber* count).Take(count).ToList(), result);
            _bookRepositoryMock.Verify(
            x => x.GetAllBooksAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
            Times.Once);


        }

        [Theory]
        [InlineData(null, "a", null, 3, 5)]
        [InlineData(null, "a", null, 1, 1)]
    
        public async Task Handle_Should_ReturnSearchedBooksByAuthor(
          string title,
          string author,
          string genre,
          int pageNumber,
          int count)
        {
            //Arrange
            _bookRepositoryMock.Setup(
                   x => x.GetBooksByAuthorNameAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()))
                   .ReturnsAsync(() => _booksList);
            GetSearchedBooksQuery _query = new(title, author, genre, pageNumber, count);

            //Act
            var result = await _handler.Handle(_query, default);
            //Assert
            CollectionAssert.AreEqual(
                _booksList.Select(a=>a.Title).Skip(pageNumber* count).Take(count).ToList()
                , result);

            _bookRepositoryMock.Verify(
          x => x.GetBooksByAuthorNameAsync(
                  It.IsAny<List<Book>>(),
                  It.IsAny<string>()),
          Times.Once);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByGenreAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Never);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByTitleAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Never);
            _bookRepositoryMock.Verify(
            x => x.GetAllBooksAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
            Times.Never);
        }


        [Theory]
        [InlineData("a", "a", null, 3, 5)]
        [InlineData("t", "a", null, 1, 1)]

        public async Task Handle_Should_ReturnSearchedBooksByAuthorAndTitle(
          string title,
          string author,
          string genre,
          int pageNumber,
          int count)
        {
            //Arrange
            _bookRepositoryMock.Setup(
                   x => x.GetBooksByAuthorNameAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()))
                   .ReturnsAsync(() => _booksList);
            _bookRepositoryMock.Setup(
                   x => x.GetBooksByTitleAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()))
                   .ReturnsAsync(() => _booksList);
            GetSearchedBooksQuery _query = new(title, author, genre, pageNumber, count);

            //Act
            var result = await _handler.Handle(_query, default);
            //Assert
            CollectionAssert.AreEqual(
                _booksList.Select(a => a.Title).Skip(pageNumber * count).Take(count).ToList()
                , result);

            _bookRepositoryMock.Verify(
          x => x.GetBooksByAuthorNameAsync(
                  It.IsAny<List<Book>>(),
                  It.IsAny<string>()),
          Times.Once);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByGenreAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Never);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByTitleAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Once);
            _bookRepositoryMock.Verify(
            x => x.GetAllBooksAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
            Times.Never);
        }

        [Theory]
        [InlineData("a", "a", "t", 3, 5)]
        [InlineData("t", "a", "g", 1, 1)]
        public async Task Handle_Should_ReturnSearchedBooksByAuthorAndTitleAndGenre(
            string title,
            string author,
            string genre,
            int pageNumber,
            int count)
        {
            //Arrange
            _bookRepositoryMock.Setup(
                   x => x.GetBooksByAuthorNameAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()))
                   .ReturnsAsync(() => _booksList);

            _bookRepositoryMock.Setup(
                   x => x.GetBooksByTitleAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()))
                   .ReturnsAsync(() => _booksList);

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
                _booksList.Select(a => a.Title).Skip(pageNumber * count).Take(count).ToList()
                , result);

            _bookRepositoryMock.Verify(
          x => x.GetBooksByAuthorNameAsync(
                  It.IsAny<List<Book>>(),
                  It.IsAny<string>()),
          Times.Once);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByGenreAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Once);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByTitleAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Once);
            _bookRepositoryMock.Verify(
            x => x.GetAllBooksAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
            Times.Never);
        }


        [Theory]
        [InlineData("a", null, null, 3, 5)]
        [InlineData("t", null, null, 1, 1)]
        public async Task Handle_Should_ReturnSearchedBooksByTitle(
            string title,
            string author,
            string genre,
            int pageNumber,
            int count)
        {
            //Arrange
          
            _bookRepositoryMock.Setup(
                   x => x.GetBooksByTitleAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()))
                   .ReturnsAsync(() => _booksList);

            GetSearchedBooksQuery _query = new(title, author, genre, pageNumber, count);

            //Act
            var result = await _handler.Handle(_query, default);
            //Assert
            CollectionAssert.AreEqual(
                _booksList.Select(a => a.Title).Skip(pageNumber * count).Take(count).ToList()
                , result);

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
            _bookRepositoryMock.Verify(
         x => x.GetBooksByTitleAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Once);
            _bookRepositoryMock.Verify(
            x => x.GetAllBooksAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
            Times.Never);
        }

        [Theory]
        [InlineData("a", null, "g", 3, 5)]
        [InlineData("t", null, "fd", 1, 1)]
        public async Task Handle_Should_ReturnSearchedBooksByTitleAndGenre(
            string title,
            string author,
            string genre,
            int pageNumber,
            int count)
        {
            //Arrange

            _bookRepositoryMock.Setup(
                   x => x.GetBooksByTitleAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()))
                   .ReturnsAsync(() => _booksList);

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
                _booksList.Select(a => a.Title).Skip(pageNumber * count).Take(count).ToList()
                , result);

            _bookRepositoryMock.Verify(
          x => x.GetBooksByAuthorNameAsync(
                  It.IsAny<List<Book>>(),
                  It.IsAny<string>()),
          Times.Never);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByGenreAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Once);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByTitleAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Once);
            _bookRepositoryMock.Verify(
            x => x.GetAllBooksAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
            Times.Never);
        }

        [Theory]
        [InlineData(null, "a", "g", 3, 5)]
        [InlineData(null, "fds", "aw", 1, 1)]
        public async Task Handle_Should_ReturnSearchedBooksByAuthorAndGenre(
       string title,
       string author,
       string genre,
       int pageNumber,
       int count)
        {
            //Arrange

            _bookRepositoryMock.Setup(
                   x => x.GetBooksByAuthorNameAsync(
                       It.IsAny<List<Book>>(),
                       It.IsAny<string>()))
                   .ReturnsAsync(() => _booksList);

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
                _booksList.Select(a => a.Title).Skip(pageNumber * count).Take(count).ToList()
                , result);

            _bookRepositoryMock.Verify(
          x => x.GetBooksByAuthorNameAsync(
                  It.IsAny<List<Book>>(),
                  It.IsAny<string>()),
          Times.Once);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByGenreAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Once);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByTitleAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Never);
            _bookRepositoryMock.Verify(
            x => x.GetAllBooksAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
            Times.Never);
        }

        [Theory]
        [InlineData(null, null, "g", 3, 5)]
        [InlineData(null, null, "ds", 1, 1)]
        public async Task Handle_Should_ReturnSearchedBooksByGenre(
            string title,
            string author,
            string genre,
            int pageNumber,
            int count)
        {
            //Arrange

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
                _booksList.Select(a => a.Title).Skip(pageNumber * count).Take(count).ToList()
                , result);

            _bookRepositoryMock.Verify(
          x => x.GetBooksByAuthorNameAsync(
                  It.IsAny<List<Book>>(),
                  It.IsAny<string>()),
          Times.Never);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByGenreAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Once);
            _bookRepositoryMock.Verify(
         x => x.GetBooksByTitleAsync(
                 It.IsAny<List<Book>>(),
                 It.IsAny<string>()),
         Times.Never);
            _bookRepositoryMock.Verify(
            x => x.GetAllBooksAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
            Times.Never);
        }

    }
}
