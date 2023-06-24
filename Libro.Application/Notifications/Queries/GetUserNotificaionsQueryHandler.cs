using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Notifications.Queries
{
    public sealed class GetUserNotificaionsQueryHandler : IRequestHandler<GetUserNotificaionsQuery, (List<Notification>, int)>
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<GetUserNotificaionsQueryHandler> _logger;

        public GetUserNotificaionsQueryHandler(
           INotificationRepository notificationRepository,
           IUserRepository userRepository,
           ILogger<GetUserNotificaionsQueryHandler> logger
           )
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<(List<Notification>, int)> Handle(GetUserNotificaionsQuery request, CancellationToken cancellationToken)
        {

            return await _notificationRepository.GetNotifications(request.UserId, request.PageNumbr, request.Count);
        }
    }
}
