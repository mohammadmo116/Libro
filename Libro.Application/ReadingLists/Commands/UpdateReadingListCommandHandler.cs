using Libro.Application.Interfaces;
using Libro.Infrastructure.Repositories;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libro.Domain.Exceptions;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed class UpdateReadingListCommandHandler : IRequestHandler<UpdateReadingListCommand, bool>
    {
        private readonly IReadingListRepository _readingListRepository;
        private readonly ILogger<UpdateReadingListCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateReadingListCommandHandler(
             IReadingListRepository readingListRepository,
             ILogger<UpdateReadingListCommandHandler> logger,
             IUnitOfWork unitOfWork
            )
        {
            _readingListRepository = readingListRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(UpdateReadingListCommand request, CancellationToken cancellationToken)
        {

            var readingList = await _readingListRepository.GetReadingListAsync(request.ReadingList.Id);
            if (readingList is null)
            {
                _logger.LogInformation($"CustomNotFoundException readingListId:{request.ReadingList.Id}");
                throw new CustomNotFoundException("ReadingList");

            }
            readingList.Name = request.ReadingList.Name is null ?
              readingList.Name : request.ReadingList.Name;


            _readingListRepository.UpdateReadingList(readingList);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 0;
        }
    }
}
