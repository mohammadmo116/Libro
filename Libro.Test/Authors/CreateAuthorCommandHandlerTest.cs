using Libro.Application.Authors.Commands;
using Libro.Application.Books.Commands;
using Libro.Application.Interfaces;
using Libro.Application.Repositories;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Test.Authors
{
    public class CreateAuthorCommandHandlerTest
    {

        private readonly Author _author;
        private readonly CreateAuthorCommand _command;
        private readonly CreateAuthorCommandHandler _handler;
        private readonly Mock<IAuthorRepository> _authorRepositoryMock;
        private readonly Mock<ILogger<CreateAuthorCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public CreateAuthorCommandHandlerTest()
        {
            _author = new()
            {
              Id = Guid.NewGuid(),
              Name = "Test",
              DateOfBirth = DateTime.UtcNow
            };
            _unitOfWorkMock = new();
            _authorRepositoryMock = new();
            _loggerMock = new();
            _command = new(_author);
            _handler = new(
                _loggerMock.Object,
                _authorRepositoryMock.Object,
                _unitOfWorkMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnAuthor_WhenAuthorIsCreated()
        {
            //Arrange
            _authorRepositoryMock.Setup(
              x => x.CreateAuthorAsync(
                  It.IsAny<Author>()));

            _unitOfWorkMock.Setup(
             x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            //Act
            var result = await _handler.Handle(_command, default);

            //Assert

            _authorRepositoryMock.Verify(
              x => x.CreateAuthorAsync(It.Is<Author>(x => x.Id == _author.Id)),
              Times.Once);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Once);

            Assert.Equal(_author.Id, result.Id);
        }

    }
}
