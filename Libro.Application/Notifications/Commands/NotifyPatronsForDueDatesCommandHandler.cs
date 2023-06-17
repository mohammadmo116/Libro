using FluentEmail.Core;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Libro.Application.Notifications.Commands
{
    public class NotifyPatronsForDueDatesCommandHandler : IRequestHandler<NotifyPatronsForDueDatesCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotifyPatronsForDueDatesCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFluentEmailFactory _emailFactory;
        public NotifyPatronsForDueDatesCommandHandler(
            INotificationRepository notificationRepository,
            IUserRepository userRepository,
            ILogger<NotifyPatronsForDueDatesCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IFluentEmailFactory emailFactory
            )
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _emailFactory= emailFactory;
        }

        public async Task<bool> Handle(NotifyPatronsForDueDatesCommand request, CancellationToken cancellationToken)
        {

            var users = await _userRepository.GetPatronsWithDueDatesAsync();

            if (!users.Any())
            {
                return true;
            }
       
            List<Notification> notifications = new();

            foreach (var user in users)
            {
                if (user.BookTransactions.Any())
                    foreach (var Transaction in user.BookTransactions)
                    {

                        var message = "{message="+$"Please Check Your Expired Borrowed Book - Transaction #{Transaction.Id} Return it As Soon As Possible,"+
                            $"TransactionId={Transaction.Id}"+"}";
                        //for the database
                        notifications.Add(new()
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            UserId = user.Id,
                            IsRead=false
                        });
                        //SignalR- Push Notifications
                        await _notificationRepository.NotifyUser(user.Id.ToString(),"PushNotifications",message);
                        StringBuilder tamplate = new();
                        tamplate.AppendLine($"Dear @Model.UserEmail");
                        tamplate.AppendLine($"<p>Please Check Your Expired Borrowed Book <a href =\"https://localhost:7062/Book/Transactions/@Model.TransactionNumber\">@Model.BookTitle</a> . Return it As Soon As Possible</p>.");
                        tamplate.AppendLine($"- Libro Team");
                        await _emailFactory
                              .Create()
                              .To(user.Email)
                              .Subject("Expired Borrowed Book")
                              .UsingTemplate(tamplate.ToString(), new { UserEmail = user.Email, TransactionNumber = Transaction.Id, BookTitle = Transaction.Book.Title })
                              .SendAsync();
                    }
            }
        
            await _notificationRepository.DataBaseNotify(notifications);
            var NumberOfRows = await _unitOfWork.SaveChangesAsync();
            return NumberOfRows > 0;
        }
    }

}
