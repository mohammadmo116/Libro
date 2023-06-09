using Libro.Application.Interfaces;
using Libro.Application.ReadingLists.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Notifications.Commands
{
    public sealed class NotifyPatronsForReservedBooksCommandHandler : IRequestHandler<NotifyPatronsForReservedBooksCommand,bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotifyPatronsForReservedBooksCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public NotifyPatronsForReservedBooksCommandHandler(
            INotificationRepository notificationRepository,
            IUserRepository userRepository,
            ILogger<NotifyPatronsForReservedBooksCommandHandler> logger,
             IUnitOfWork unitOfWork
            ) 
        {
            _notificationRepository = notificationRepository;
            _userRepository= userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(NotifyPatronsForReservedBooksCommand request, CancellationToken cancellationToken)
        {
       
            var userIds = await _userRepository.GetPatronIdsForReservedBooksAsync();

                 if (!userIds.Any())
                 {
                    return true;
                 }
                var userIdsStringList = userIds.ConvertAll(a => a.ToString());
                List<Notification> notifications = new();
                foreach (var userId in userIds)
                {
                    notifications.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Message = "Please Check Your Reserved Books To Borrow then As Soon As Possible",
                        UserId = userId
                    });


                }
                await _notificationRepository.NotifyUsers(userIdsStringList,
                    "PushNotifications",
                    "Please Check Your Reserved Books To Borrow As Soon As Possible"
                    );
                await _notificationRepository.DataBaseNotify(notifications);
                var NumberOfRows = await _unitOfWork.SaveChangesAsync();
                return NumberOfRows > 0;
            
           
        }
    }
}
