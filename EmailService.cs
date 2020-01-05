using AppleyardsChimney.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AppleyardsChimney.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig ec;

        public EmailService(IOptions<EmailConfig> emailConfig)
        {
            ec = emailConfig.Value;
        }

        public async Task SendEmailAsync(string PageHeaderText, string Subject, List<string> MessageBody, string SendToEmailAddress = "", 
                                         Attachment attachment = null, bool sendAsHTML = true)
        {
            try
            {           
                //===< Loop through the MessageBody List >===
                string mailMessage = string.Empty;
                if (!string.IsNullOrWhiteSpace(PageHeaderText)) mailMessage = PageHeaderText + (sendAsHTML ? "<br />" : "\n");
                foreach (string item in MessageBody)
                {
                    if (sendAsHTML) { mailMessage += item + "<br />"; }
                    else { mailMessage += item + "\n"; }
                }

                // Send to Developer if nothing else specified
                if (string.IsNullOrWhiteSpace(SendToEmailAddress)) { SendToEmailAddress = ec.DeveloperEmailAddress; }
               
                // Set the Mail Message Details
                MailMessage mm = new MailMessage(ec.FromAddress, SendToEmailAddress, Subject, mailMessage)
                {
                    IsBodyHtml = sendAsHTML
                };

                // Add attachments
                if (attachment != null) { mm.Attachments.Add(attachment); }


                // Create SMTP Client and configure from settings                
                using (var client = new SmtpClient()
                {                    
                    Host = ec.MailServerAddress,                  
                    Port = Convert.ToInt32(ec.MailServerPort),
                    Credentials = new NetworkCredential(ec.UserId, ec.UserPassword)
                }) { await client.SendMailAsync(mm); }

            }
            catch (Exception ex)
            {
                // do something with error           
            }
        
        }
    }
}
