using Libro.Application.BookTransactions.Commands;
using Libro.Application.BookTransactions.Queiries;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheoryAttribute = Xunit.TheoryAttribute;

namespace Libro.Test.BookTransactions
{
    public class TrackDueDateQueryHandlerTest
    {
        private readonly List<BookTransaction> _bookTransactionsList;
        private readonly TrackDueDateQueryHandler _handler;
        private readonly Mock<IBookTransactionRepository> _bookTransactionRepository;
        private readonly Mock<ILogger<TrackDueDateQueryHandler>> _loggerMock;

        public TrackDueDateQueryHandlerTest()
        {

            _bookTransactionRepository = new();
            _loggerMock = new();

            _bookTransactionsList = new()
            {new(){
                   Id = Guid.NewGuid(),
                   BookId = Guid.NewGuid(),
                   UserId = Guid.NewGuid(),
            },
            new(){
                   Id = Guid.NewGuid(),
                   BookId = Guid.NewGuid(),
                   UserId = Guid.NewGuid(),
            },
            new(){
                   Id = Guid.NewGuid(),
                   BookId = Guid.NewGuid(),
                   UserId = Guid.NewGuid(),
            },
            };

  
            _handler = new TrackDueDateQueryHandler(
               _loggerMock.Object,
               _bookTransactionRepository.Object
               );
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1,1)]
        [InlineData(5, 5)]
        public async Task Handle_Should_ReturnListOfBookTransactions(int PageNumber,int Count)
        {
            //Arrange
            _bookTransactionRepository.Setup
                (
                    x => x.TrackDueDateAsync
                    (
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )
                )
                .ReturnsAsync(_bookTransactionsList
                                .Skip(PageNumber*Count)
                                .Take(Count)
                                .ToList()
                                );


            //Act
            var _query = new TrackDueDateQuery(PageNumber,Count);
            var result = await _handler.Handle(_query, default);


            //Assert
            _bookTransactionRepository.Verify(
                x => x.TrackDueDateAsync(
                    It.Is<int>(a=>a== PageNumber),
                    It.Is<int>(a => a == Count)),
                Times.Once);
            CollectionAssert.AreEqual(_bookTransactionsList.Skip(PageNumber * Count).Take(Count).ToList(), result);

          
        }

    }
}
