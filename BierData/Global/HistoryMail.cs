using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Global
{
    public class HistoryMail
    {
        public HistoryMail(System.Net.Mail.MailMessage mail)
        {
            foreach(var receipient in mail.To)
            {
                To += receipient.Address + "; ";
            }
            To = To.Remove(To.Length - 3);
            Subject = mail.Subject;
            SendingTime = DateTime.Now;
            Body = mail.Body;
        }

        public HistoryMail(string historyEntry)
        {
            string[] parts = historyEntry.Split("|||".ToCharArray());
            To = parts[0];
            Subject = parts[1];
            SendingTime = DateTime.Parse(parts[2]);
            Body = parts[3];
        }

        public override string ToString()
        {
            return To + "|||" + Subject + "|||" + SendingTime.ToString() + "|||" + Body;
        }

        public string To { get; set; }

        public string Subject { get; set; }

        public DateTime SendingTime { get; set; }

        public string Body { get; set; }
    }
}
