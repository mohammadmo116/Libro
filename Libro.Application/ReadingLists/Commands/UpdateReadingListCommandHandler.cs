using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

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
            var readingList = await _readingListRepository.GetReadingListByUserAsync(request.UserId, request.ReadingList.Id);
            if (readingList is null)
            {
                _logger.LogInformation($"CustomNotFoundException readingListId:{request.ReadingList.Id}");
                throw new CustomNotFoundException("ReadingList");

            }
            readingList.Name = request.ReadingList.Name is null ?
              readingList.Name : request.ReadingList.Name;

            readingList.Private = request.ReadingList.Private is null ?
              readingList.Private : request.ReadingList.Private;

            _readingListRepository.UpdateReadingList(readingList);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 0;
        }
    }
}
