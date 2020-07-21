using System;
using System.Net.Mail;

namespace SharesonServer.Repository.SupportFunctions
{
    public class MailHelper
    {
        SmtpClient smtpServer;

        public void InformAboutCreatedAccount()
        {

        }
        public void RemindPassword()
        {

        }
        public void SendMail()
        {
            MailMessage mail = new MailMessage();
            try
            {
                MailAddress From = new MailAddress("reneshijp@gmail.com");
                MailAddress To = new MailAddress("spidig21@gmail.com");
                mail = new MailMessage(From, To);

                mail.Subject = "Test message";
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = "<b>Test Mail</b><br>using <b>HTML</b>.";
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                smtpServer.Send(mail);
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
            
        }

        public MailHelper()
        {
            Initialize();
        }
        private void Initialize()
        {
            smtpServer = new SmtpClient();
            smtpServer.Host = "smtp.gmail.com";
            smtpServer.Port = 587;
            smtpServer.EnableSsl = true;
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Credentials = new System.Net.NetworkCredential("reneshijp", "vzslshmbpsrhdoqz");
        }
    }
}
