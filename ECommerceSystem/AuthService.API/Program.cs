using AuthService.Application.Interfaces;
using AuthService.Application.Services.Interfaces;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// ✅ Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register infrastructure (DB, repositories, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Register application services
builder.Services.AddScoped<IAuthService, AuthService.Application.Services.AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    // ✅ Enable Swagger middleware
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// ✅ Add Authentication BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ For APIs (important for Swagger)
app.MapControllers();

// (Optional: keep if you're using MVC views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();