using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using DataAccess.Services;
using DataAccess.Models;
using Microsoft.Extensions.Hosting;

namespace BusinessEntity.Services
{
    public class MailService
    {

        private DbWrapper _dbWrapper;
        private string _smtpServer = "relay.mailbaby.net";
        private int _smtpPort = 2525;
        private string _smtpUsername = "mb46503";
        private string _smtpPassword = "kzhYsRFyuXt2qrfEBkHS";

        public MailService(DbWrapper dbWrapper)
        {
            _dbWrapper = dbWrapper;
        }

        public async Task<bool> EnviarMailCancelacionTurno(string id)
        {

            try
            {
                var DatosTurno = await _dbWrapper.GetDatosTurno(id);
                if (DatosTurno != null)
                {
                    var Profesional = await _dbWrapper.GetProfesionalById(DatosTurno.Profesional_Id);
                    string Body = $@"<!DOCTYPE html>
                <html>
                <head>
                    <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css"">
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                        }}
                        .container {{
                            width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #ccc;
                            border-radius: 5px;
                            background-color: #37517e;
                        }}
                        .header {{
                            text-align: center;
                            background-color: #37517e;
                            color: white;
                            padding: 10px;
                            border-radius: 5px 5px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .sidebar-brand-text {{
                            font-family: Jost;
                            font-weight: 400;
                            color: white;
                            font-size: 24px;
                            text-align: center;
                            background-color: #37517e;
                            margin-bottom: 30px;
                        }}
                    </style>
                </head>
                <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;"">
                    <div style=""width: 600px; margin: 0 auto; background-color: white; font-weight: 500; padding: 20px; border-radius: 10px; box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);"">
                                <div style=""background-color: #37517e; text-align: center; padding: 15px; border-radius: 5px; margin-bottom: 30px;"">
            <img src=""https://i.ibb.co/dQYtnwL/logodef.png"" alt=""logodef"" border=""0"" style=""display: block; margin: 0 auto; width: 120px; height: auto;"">
        </div>
                        <div style=""text-align: center;"">
                            <h3 style=""color: #37517e;"">¡Turno Cancelado!</h2>
                        </div>
                        <div style=""margin-top: 20px;"">
                            <p>Hola,</p>
                            <p>Lamentamos informarte que tu turno ha sido cancelado.</p>
                            <p style=""margin-bottom: 25px;"">A continuación, los detalles del turno cancelado:</p>
                            <ul style=""list-style-type: none; padding-left: 0; margin-bottom: 25px;"">
                                <li><strong>Fecha y Hora:</strong> {DatosTurno.FechaHora}</li>
                                <li><strong>Profesional:</strong> {Profesional.Nombre} {Profesional.Apellido}</li>
                            </ul>
                        </div>
                        <div style=""margin-top: 50px;"">
                            <p>Lamentamos cualquier inconveniente que esto pueda causarte.</p>
                            <p>Saludos,</p>
                        </div>
                    </div>
                </body>
                </html>";

                    var ics = $@"BEGIN:VCALENDAR
                             VERSION:2.0
                             PRODID:-//ical.marudot.com//iCal Event Maker
                             CALSCALE:GREGORIAN
                             BEGIN:VEVENT
                             DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}
                             UID:{Guid.NewGuid()}@example.com
                             DTSTART:{DatosTurno.FechaHora:yyyyMMddTHHmmssZ}
                             DTEND:{DatosTurno.FechaHora.AddHours(1):yyyyMMddTHHmmssZ}
                             SUMMARY:CANCELADO - Turno con {Profesional.Nombre} {Profesional.Apellido}
                             DESCRIPTION:Este turno ha sido cancelado. Contacte con nosotros para más detalles.
                             STATUS:CANCELLED
                             END:VEVENT
                             END:VCALENDAR";

                    using (var client = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        client.EnableSsl = true;


                        var message = new MailMessage
                        {
                            From = new MailAddress("info@agendario.ar"),
                            Subject = "Turno cancelado - Agendario",
                            Body = Body,
                            IsBodyHtml = true
                        };
                        message.To.Add(DatosTurno.Email);

                        var icsContent = ics;
                        var icsBytes = Encoding.UTF8.GetBytes(icsContent);
                        var memoryStream = new MemoryStream(icsBytes);

                        // Adjuntar el archivo .ics al correo
                        var attachment = new Attachment(memoryStream, "event.ics", "text/calendar");
                        message.Attachments.Add(attachment);
                        await client.SendMailAsync(message);
                        await _dbWrapper.GuardarEvento("Email", $"Mail de cancelacion de turno {id} enviado a{DatosTurno.Email}", "");
                    }

                }
                return true;

            }
            catch (Exception ex)
            {
                return false;
                await _dbWrapper.GuardarEvento("Email", ex.Message, "");
            }




        }
        public async Task EnviarMailConfirmacionTurno(string toEmail, string profesional, int Profesional_Id, string FechaHora, DateTime FechaHoraTurno, string token)
        {


            var Profesional = await _dbWrapper.GetProfesionalById(Profesional_Id);
            var Ambiente = "http://agendario.ar";

            var Subject = $"Turno confirmado con {profesional}";
            var link = $"{Ambiente}/{Profesional.Alias}/cancelarturno?token={token}";
            var EventTitle = $"Turno con {profesional}";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            padding: 20px;
        }}
        .container {{
            width: 600px;
            margin: 0 auto;
            background-color: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
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
        .sidebar-brand-text {{
            font-family: Jost;
            font-weight: 500;
            color: white;
            font-size: 24px;
            text-align: center;
            background-color: #37517e;
            margin-bottom: 30px;
            padding: 10px;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div style=""background-color: #37517e; text-align: center; padding: 15px; border-radius: 5px; margin-bottom: 30px;"">
            <img src=""https://i.ibb.co/dQYtnwL/logodef.png"" alt=""logodef"" border=""0"" style=""display: block; margin: 0 auto; width: 120px; height: auto;"">
        </div>
        <div style=""text-align: center;"">
            <h3 style=""color: #007bff;"">¡Tu turno ha sido confirmado!</h3>
        </div>
        <div style=""margin-top: 20px;"">
            <p>Hola,</p>
            <p>Queremos confirmarte que tu turno ha sido guardado correctamente.</p>
            <p style=""margin-bottom: 25px;"">A continuación, los datos del turno:</p>
            <ul style=""list-style-type: none; padding-left: 0; margin-bottom: 25px;"">
                <li><strong>Fecha y Hora:</strong> {FechaHora}</li>
                <li><strong>Profesional:</strong> {profesional}</li>
            </ul>
        </div>
        <div style=""margin-top: 50px; text-align: center; margin-bottom: 35px;"">
            <p style=""margin-bottom: 35px;"">Si necesitas cancelar el turno, puedes hacerlo utilizando el botón a continuación:</p>
            <a href=""{link}"" style=""background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: 600;"">Cancelar Turno</a>
        </div>
        <div style=""margin-top: 20px;"">
            <p>Recuerda que los cambios en los turnos deben realizarse con anticipación.</p>
            <p>Saludos,</p>
        </div>
    </div>
</body>
</html>";




            var ics = GenerateICSContent(EventTitle, FechaHoraTurno, FechaHoraTurno.AddHours(1), profesional);

            await SendEmailWithICSAsync(toEmail, Subject,body, EventTitle, FechaHoraTurno, ics);
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
            }
        }






        public async Task SendEmailWithICSAsync(string toEmail, string subject, string body, string eventTitle, DateTime eventStart, string ics)
        {

            string _smtpServer = "relay.mailbaby.net";
            int _smtpPort = 2525;
            string _smtpUsername = "mb46503";
            string _smtpPassword = "kzhYsRFyuXt2qrfEBkHS";

            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;


                    var message = new MailMessage
                    {
                        From = new MailAddress("info@agendario.ar"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    message.To.Add(toEmail);

                    var icsContent = ics;
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
            }
        }


        private string GenerateICSContent(string eventTitle, DateTime eventStart, DateTime eventEnd, string profesional)
        {
            // Genera el contenido del archivo .ics en formato iCalendar
            // Aquí puedes usar bibliotecas como DDay.iCal para construir el contenido más complejo si es necesario.
            string icsContent = $@"BEGIN:VCALENDAR
        VERSION:2.0
        PRODID:-//ical.marudot.com//iCal Event Maker
        CALSCALE:GREGORIAN
        BEGIN:VTIMEZONE
        TZID:America/Argentina/Buenos_Aires
        LAST-MODIFIED:20201011T015911Z
        TZURL:http://tzurl.org/zoneinfo-outlook/America/Argentina/Buenos_Aires
        X-LIC-LOCATION:America/Argentina/Buenos_Aires
        BEGIN:STANDARD
        TZNAME:-03
        TZOFFSETFROM:-0300
        TZOFFSETTO:-0300
        DTSTART:19700101T000000
        END:STANDARD
        END:VTIMEZONE
        BEGIN:VEVENT
        DTSTAMP:20230810T210604Z
        UID:1691701548811-60579@ical.marudot.com
        DTSTART;TZID=America/Argentina/Buenos_Aires:{eventStart:yyyyMMddTHHmmssZ}
        DTEND;TZID=America/Argentina/Buenos_Aires:{eventEnd:yyyyMMddTHHmmssZ}
        SUMMARY:Turno con {profesional}
        DESCRIPTION:Turno con {profesional}
        END:VEVENT
        END:VCALENDAR";

            return icsContent;

        }
    }
}
