using System;
using System.Net.Mail;
using System.Threading;


namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmailService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EmailService.svc or EmailService.svc.cs at the Solution Explorer and start debugging.
    public class EmailService : IEmailService
    {
        public static DateTime Minutes;
        void send_mail(string email)
        {

            string[] temp = email.Split('@');
            temp = temp[1].Split('.');
            string domain = temp[0];

            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + domain + "')", true);
            SmtpClient smtpClient = new SmtpClient("smtp." + domain + ".com", 587);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            smtpClient.Credentials = new System.Net.NetworkCredential("infoarchi40@gmail.com", "1a5a9a1a5a9a");

            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress("infoarchi40@gmail.com");
            mail.To.Add(new MailAddress(email));

            mail.Subject = "Blood Pressure Tracker";
            mail.Body = "Please, Measure your blood pressure ASAP";
            smtpClient.Send(mail);
        }
        void IEmailService.remind_userByEmail(string email, DateTime Endtime)
        {
            int seconds = (Int32)((Endtime.Subtract(DateTime.Now)).TotalSeconds);
            int mili_seconds = seconds * 1000;
            Minutes = Endtime;
            Thread.Sleep(mili_seconds);
            send_mail(email);
        }

        DateTime IEmailService.get_time()
        {
            return Minutes;
        }
    }
}
