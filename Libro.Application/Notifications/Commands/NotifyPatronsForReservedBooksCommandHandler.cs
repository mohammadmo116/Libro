using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Text;


namespace Libro.Application.Notifications.Commands
{
    public sealed class NotifyPatronsForReservedBooksCommandHandler : IRequestHandler<NotifyPatronsForReservedBooksCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotifyPatronsForReservedBooksCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFluentEmailFactory _emailFactory;
        public NotifyPatronsForReservedBooksCommandHandler(
            INotificationRepository notificationRepository,
            IUserRepository userRepository,
            ILogger<NotifyPatronsForReservedBooksCommandHandler> logger,
             IUnitOfWork unitOfWork,
             IFluentEmailFactory emailFactory
            )
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _emailFactory = emailFactory;
        }

        public async Task<bool> Handle(NotifyPatronsForReservedBooksCommand request, CancellationToken cancellationToken)
        {
            try
            { 
       
                var users = await _userRepository.GetPatronsWithReservedBooksAsync();

                if (!users.Any())
                {
                    return true;
                }
                List<Notification> notifications = new();
                foreach (var user in users)
                {
                    if(user.BookTransactions.Any())
                    foreach (var Transaction in user.BookTransactions)
                    {
                            var message = "{message=" + $"Please Check Your Reserved Book - Transaction #{Transaction.Id} To Resturn it As Soon As Possible," +
                            $"TransactionId={Transaction.Id}" + "}";
                            //for the database
                            notifications.Add(new()
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            UserId = user.Id,
                            IsRead = false
                        });
                            //SignalR- Push Notifications
                            await _notificationRepository.NotifyUser(user.Id.ToString(), "PushNotifications", message);
                            StringBuilder tamplate = new ();
                            tamplate.AppendLine($"Dear @Model.UserEmail");
                            tamplate.AppendLine($"<p>Please Check Your Reserved Book <a href =\"https://localhost:7062/user/Transactions/@Model.TransactionNumber\">@Model.BookTitle</a> . Borrow As Soon As Possible</p>.");
                            tamplate.AppendLine($"- Libro Team");

                                 var email = await _emailFactory
                                .Create()
                                .To(user.Email)
                                .Subject("Reserved Book")
                                .UsingTemplate(tamplate.ToString(), new { UserEmail = user.Email, TransactionNumber = Transaction.Id, BookTitle = Transaction.Book.Title })
                                .SendAsync();
      
                    }
                       

                }

                await _notificationRepository.DataBaseNotify(notifications);
                var NumberOfRows = await _unitOfWork.SaveChangesAsync();
                return NumberOfRows > 0;


            }
            catch (Exception ex)
        {
                Console.WriteLine("An error occurred while sending the email: " + ex.Message);
            }

            return true;


        }
    }
}
