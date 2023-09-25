using BusinessEntity.Services;
using DataAccess.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DataAccess.Services;
using TurnoMedico.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // Si es necesario
     });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddMvc().AddRazorRuntimeCompilation();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


builder.Services.AddTransient<ReservaService, ReservaService>();
builder.Services.AddTransient<ValidationService, ValidationService>();
builder.Services.AddTransient<TokenService, TokenService>();
builder.Services.AddTransient<DbWrapper, DbWrapper>();
builder.Services.AddTransient<MailService, MailService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "profesional",
    pattern: "{profesional}", // El segmento después del dominio será el identificador del profesional
    defaults: new { controller = "Home", action = "Index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "error",
    pattern: "{*url}",
    defaults: new { controller = "Home", action = "Error" } // Cambia esto según tu controlador y acción de error
);
app.Run();
