using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using UITManagerAlarmManager.Data;
using UITManagerAlarmManager.Models;

namespace UITManagerAlarmManager.Service;

public class Email {
    private readonly ApplicationDbContext _context;

    public Email(ApplicationDbContext context) {
        _context = context;

        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        string projectDirectory = Path.Combine(baseDirectory, @"..\..\..");

        projectDirectory = Path.GetFullPath(projectDirectory);

        string envFilePath = Path.Combine(projectDirectory, ".env");

        Env.Load(envFilePath);
    }

    public async Task Send(string body) {
        
        List<ApplicationUser> users = await _context.Users.ToListAsync();
        Env.Load("UITManagerAlarmManager/.env");

        string smtpHost = Env.GetString("SMTP_HOST");
        int smtpPort = Env.GetInt("SMTP_PORT");
        string smtpUsername = Env.GetString("SMTP_USERNAME");
        string smtpPassword = Env.GetString("SMTP_PASSWORD");
        string fromEmail = Env.GetString("SMTP_FROM_EMAIL");
        
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        
        SmtpClient smtpClient = new SmtpClient(smtpHost) {
            Port = smtpPort,
            Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            EnableSsl = true
        };
        foreach (var user in users) {
            try {
                MailMessage mail = new MailMessage {
                    From = new MailAddress(fromEmail),
                    Subject = "Alarm triggered",
                    Body = body,
                    IsBodyHtml = true
                };

                if (!string.IsNullOrEmpty(user.Email)) {
                    mail.To.Add(user.Email);
                } else {
                    Console.WriteLine($"Aucun e-mail trouvé pour l'utilisateur {user.UserName}. Envoi ignoré.");
                    continue; 
                }

                await smtpClient.SendMailAsync(mail);
                Console.WriteLine($"E-mail envoyé avec succès à {user.UserName} ({user.Email}) !");
            } catch (Exception ex) {
                Console.WriteLine($"Erreur lors de l'envoi de l'e-mail à {user.UserName} ({user.Email}): {ex.Message}");
            }
        }
    }
}