using Hangfire;
using Libro.Application.Notifications.Commands;
using MediatR;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Jobs
{
    public class JobToNotifyPatronDueForDateBooks
    {
        private readonly IMediator _mediator;
        private readonly ILogger<JobToNotifyPatronDueForDateBooks> _logger;
        public JobToNotifyPatronDueForDateBooks(IMediator mediator,
            ILogger<JobToNotifyPatronDueForDateBooks> logger) {
            _mediator = mediator;
            _logger= logger;
        }
        [AutomaticRetry(Attempts = 3)]
        public async Task ExecuteAsync()
        {
            var request = new NotifyPatronsForDueDatesCommand();
            _ = await _mediator.Send(request);
            _logger.LogInformation("Notifying patron users for due date books");
        }
    }
}
