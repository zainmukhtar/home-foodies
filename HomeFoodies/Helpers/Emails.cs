using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace HomeFoodies.Helpers
{
    public class Emails
    {
        private String EMAIL_ID;
        private String EMAIL_PASSWORD;
        private String SMTP_HOST;
        private Int32 SMTP_PORT;

        public Emails(String pEmailID, String pEmailPassword, String pSMTPHost, Int32 pSMTPPORT)
        {
            EMAIL_ID = pEmailID;
            EMAIL_PASSWORD = pEmailPassword;
            SMTP_HOST = pSMTPHost;
            SMTP_PORT = pSMTPPORT;
        }

        public Boolean SendEmail(String pTo, String pSubject, String pEmailBody)
        {
            MailAddress ma = new MailAddress(EMAIL_ID, "Home Foodies");
            MailMessage msg = new MailMessage();
            msg.To.Add(pTo);
            msg.From = ma;
            string emailBody = pEmailBody;
            msg.Priority = MailPriority.High;
            msg.Body = emailBody;
            msg.Subject = pSubject;
            SendEmailUsingGmail(msg);

            return true;
        }

        public void SendEmailUsingGmail(MailMessage msg)
        {
            try
            {
                SmtpClient smpt = null;
                smpt = new SmtpClient(SMTP_HOST, Convert.ToInt32(SMTP_PORT));
                smpt.EnableSsl = true;
                smpt.DeliveryMethod = SmtpDeliveryMethod.Network;
                smpt.UseDefaultCredentials = false;
                smpt.Credentials = new System.Net.NetworkCredential(EMAIL_ID, EMAIL_PASSWORD);
                smpt.Send(msg);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
