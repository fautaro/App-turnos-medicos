using BusinessEntity.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // Si es necesario
     });

builder.Services.AddMvc().AddRazorRuntimeCompilation();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


builder.Services.AddTransient<ReservaService, ReservaService>();
builder.Services.AddTransient<ValidationService, ValidationService>();

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
