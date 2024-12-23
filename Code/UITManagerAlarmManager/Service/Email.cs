using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using UITManagerAlarmManager.Data;
using UITManagerAlarmManager.Models;

namespace UITManagerAlarmManager.Service ;
public class Email {
    private readonly ApplicationDbContext _context;
    
    public Email(ApplicationDbContext context) {
        _context = context; 
    }

    public async Task Send(string subject) {
        List<ApplicationUser> User = await _context.Users.ToListAsync();

        try
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            
            // Paramètres SMTP
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("musica.proytb@gmail.com", "sgpq vvbv gcpn lpac"),
                EnableSsl = true
            };

            // Configuration du message
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("musica.proytb@gmail.com"),
                Subject = "Alarm tiggered",
                Body = subject,
                IsBodyHtml = true
            };
            mail.To.Add("mathis.spronck@student.hers.be");

            // Envoyer l'e-mail
            smtpClient.Send(mail);
            Console.WriteLine("E-mail envoye avec succes !");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur : " + ex.Message);
        }

    }
}
