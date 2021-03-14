using Microsoft.Azure.WebJobs.Extensions.SendGrid;
using Microsoft.Extensions.Options;
using CustomOption = Speeding.Infraction.Management.AF01.ConfigOptions;
using Speeding.Infraction.Management.AF01.Handlers.Interfaces;
using Speeding.Infraction.Management.AF01.Models;
using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Speeding.Infraction.Management.AF01.Handlers.Implementations
{
    public class SendGridOwnerNotificationHandler : IOwnerNotificationHandler
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly CustomOption.SendGridOptions _options;

        public SendGridOwnerNotificationHandler(ISendGridClient sendGridClient,
            IOptions<CustomOption.SendGridOptions> settings)
        {
            _sendGridClient = sendGridClient ??
                throw new ArgumentNullException(nameof(sendGridClient));

            _options = settings.Value;
                
        }
        public async Task NotifyOwnerAsync(OwnerNotificationMessage ownerNotificationMessage)
        {
            var message = CreateEmailMessage(ownerNotificationMessage);

            await _sendGridClient
                .SendEmailAsync(message)
                .ConfigureAwait(false);

            throw new NotImplementedException();
        }

        private SendGridMessage CreateEmailMessage(OwnerNotificationMessage ownerNotificationMessage)
        {
            var emailBody = string.Format(
                    _options.EmailBodyTemplate,
                    ownerNotificationMessage.VehicleOwnerInfo.GetFullName(),
                    ownerNotificationMessage.VehicleOwnerInfo.VehicleRegistrationNumber,
                    ownerNotificationMessage.InfractionDate,
                    ownerNotificationMessage.InfractionDistrict,
                    ownerNotificationMessage.TicketNumber

                );

            var subject = string.Format(
                    _options.EmailSubject,
                    ownerNotificationMessage.TicketNumber
                );

            SendGridMessage message = new SendGridMessage
            {
                From = new EmailAddress(_options.EmailFromAddress),
                Subject = subject,
               
            };
            message.AddTo(ownerNotificationMessage.VehicleOwnerInfo.Email);
            message.AddContent("text/html", emailBody);

            foreach (var attachment in ownerNotificationMessage.Attachments)
            {
                message.AddAttachment(
                        new Attachment
                        {
                            Content = attachment.Content,
                            Type = attachment.ContentType,
                            Filename = attachment.Name
                        }
                    );

            }

            return message;
        }
    }
}
