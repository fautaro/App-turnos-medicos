using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntity.Services
{
    public class MailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public MailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
        }



        public async Task EnviarMailConfirmacionTurno(string toEmail, string profesional, string alias, string FechaHora, DateTime FechaHoraTurno, string token)
        {
            var Subject = $"Turno confirmado con {profesional}";
            var link = $"https://url.com/{alias}/cancelarturno?token={token}";
            var EventTitle = $"Turno con {profesional}";
            string Body = $@"<!DOCTYPE html>
            <html>
            <head>
                <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css"">
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        border: 1px solid #ccc;
                        border-radius: 5px;
                        background-color: #f5f5f5;
                    }}
                    .header {{
                        text-align: center;
                        background-color: #007bff;
                        color: white;
                        padding: 10px;
                        border-radius: 5px 5px 0 0;
                    }}
                    .content {{
                        padding: 20px;
                    }}
                </style>
                    <link href=""https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap"" rel=""stylesheet"">
            </head>
            <body style=""font-family:Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
                <div style=""max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 10px; box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);"">
                    <div style=""text-align: center;"">
                        <h2 style=""color: #007bff;"">¡Tu turno ha sido confirmado!</h2>
                    </div>
                    <div style=""margin-top: 20px;"">
                        <p>Hola,</p>
                        <p >Queremos confirmarte que tu turno ha sido confirmado.</p>
                        <p style=""margin-bottom:25px"">A continuación, los datos del turno:</p>
                        <ul style=""list-style-type: none; padding-left: 0;margin-bottom:25px"">
                            <li><strong>Fecha y Hora:</strong> {FechaHora}</li>
                            <li><strong>Profesional:</strong> {profesional}</li>
                        </ul>
                    </div>
                    <div style=""margin-top: 50px; text-align: center; margin-bottom:35px"">
                        <p style=""margin-bottom:35px"">Si necesitas cancelar el turno, puedes hacerlo utilizando el botón a continuación:</p>
                        <a href=""{link}"" style=""background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: 600;"">Cancelar Turno</a>
                    </div>
                    <div style=""margin-top: 20px;"">
                        <p>Recuerda que los cambios en los turnos deben realizarse con anticipación.</p>
                        <p>Saludos,</p>
                    </div>
                </div>
            </body>
            </html>";


            await SendEmailWithICSAsync(toEmail, Subject, Body, EventTitle, FechaHoraTurno);
        }




        public async Task SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;

                    var message = new MailMessage
                    {
                        From = new MailAddress(_smtpUsername),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    message.To.Add(toEmail);

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }






        public async Task SendEmailWithICSAsync(string toEmail, string subject, string body, string eventTitle, DateTime eventStart)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;

                    var message = new MailMessage
                    {
                        From = new MailAddress(_smtpUsername),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    message.To.Add(toEmail);

                    // Crear archivo .ics
                    var icsContent = GenerateICSContent(eventTitle, eventStart);
                    var icsBytes = Encoding.UTF8.GetBytes(icsContent);
                    var memoryStream = new MemoryStream(icsBytes);

                    // Adjuntar el archivo .ics al correo
                    var attachment = new Attachment(memoryStream, "event.ics", "text/calendar");
                    message.Attachments.Add(attachment);

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private string GenerateICSContent(string eventTitle, DateTime eventStart)
        {
            // Genera el contenido del archivo .ics en formato iCalendar
            // Aquí puedes usar bibliotecas como DDay.iCal para construir el contenido más complejo si es necesario.

            return $"BEGIN:VCALENDAR\nVERSION:2.0\nPRODID:-//YourCompany//Event Calendar//EN\nBEGIN:VEVENT\nSUMMARY:{eventTitle}\nDTSTART:{eventStart:yyyyMMddTHHmmssZ}\nEND:VEVENT\nEND:VCALENDAR";
        }
    }
}
