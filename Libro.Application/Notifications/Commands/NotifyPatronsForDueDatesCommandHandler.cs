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
using Libro.Domain.Entities;

namespace Libro.Application.Notifications.Commands
{
    public class NotifyPatronsForDueDatesCommandHandler : IRequestHandler<NotifyPatronsForDueDatesCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotifyPatronsForDueDatesCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public NotifyPatronsForDueDatesCommandHandler(
            INotificationRepository notificationRepository,
            IUserRepository userRepository,
            ILogger<NotifyPatronsForDueDatesCommandHandler> logger,
             IUnitOfWork unitOfWork
            )
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(NotifyPatronsForDueDatesCommand request, CancellationToken cancellationToken)
        {

            var userIds = await _userRepository.GetPatronIdsForDueDatesAsync();

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
                    Message = "Please Check Your Borrowed Books To Return Before the Due Date",
                    UserId = userId
                });


            }
            await _notificationRepository.NotifyUsers(userIdsStringList,
                "PushNotifications",
                "Please Check Your Borrowed Books To Return Before the Due Date"
                );
            await _notificationRepository.DataBaseNotify(notifications);
            var NumberOfRows = await _unitOfWork.SaveChangesAsync();
            return NumberOfRows > 0;
        }
    }
    
}
