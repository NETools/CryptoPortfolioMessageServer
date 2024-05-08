using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Services
{
	public class EmailSender
	{
		private SendGridClient _client;
		private EmailAddress _from;
		public EmailSender(string apiKey, EmailAddress from)
		{
			_client = new SendGridClient(apiKey);
			_from = from;
		}

		public async Task SendEmail(string subject, string htmlData, string recipientMail)
		{
			var to = new EmailAddress(recipientMail);
			var message = MailHelper.CreateSingleEmail(_from, to, subject, "", htmlData);
			var response = await _client.SendEmailAsync(message);
		}
	}
}
