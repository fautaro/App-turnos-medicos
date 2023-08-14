string body = @"<!DOCTYPE html>
<html>
<head>
    <style>
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f5f5f5;
        }
        .header {
            text-align: center;
            background-color: #007bff;
            color: white;
            padding: 10px;
            border-radius: 5px 5px 0 0;
        }
        .content {
            padding: 20px;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>Confirmación de Evento</h2>
        </div>
        <div class=""content"">
            <p>Hola,</p>
            <p>Queremos confirmarte que has sido registrado para el siguiente evento:</p>
            <ul>
                <li><strong>Evento:</strong> {eventTitle}</li>
                <li><strong>Fecha y Hora:</strong> {eventStart}</li>
                <li><strong>Lugar:</strong> {eventLocation}</li>
            </ul>
            <p>No faltes a este emocionante evento. ¡Te esperamos!</p>
            <p>Saludos,</p>
            <p>El equipo de [Tu Empresa]</p>
        </div>
    </div>
</body>
</html>";



string body2 = @"<!DOCTYPE html>
<html>
<head>
    <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css"">
    <style>
        body {
            font-family: Arial, sans-serif;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f5f5f5;
        }
        .header {
            text-align: center;
            background-color: #007bff;
            color: white;
            padding: 10px;
            border-radius: 5px 5px 0 0;
        }
        .content {
            padding: 20px;
        }
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
                <li><strong>Fecha y Hora:</strong> {turnoFechaHora}</li>
                <li><strong>Profesional:</strong> {turnoProfesional}</li>
                <li><strong>Lugar:</strong> {turnoLugar}</li>
            </ul>
        </div>
        <div style=""margin-top: 50px; text-align: center; margin-bottom:35px"">
            <p style=""margin-bottom:35px"">Si necesitas cancelar el turno, puedes hacerlo utilizando el botón a continuación:</p>
            <a href=""{cancelLink}"" style=""background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: 600;"">Cancelar Turno</a>
        </div>
        <div style=""margin-top: 20px;"">
            <p>Recuerda que los cambios en los turnos deben realizarse con anticipación.</p>
            <p>Saludos,</p>
        </div>
    </div>
</body>


</html>";