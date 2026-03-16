using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Identity;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Repositories;
using ProyectoPlataformaCursos.Repositories.Interfaces;
using ProyectoPlataformaCursos.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddUserStore<CustomUserStore>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<IInscripcionRepository, InscripcionRepository>();
builder.Services.AddScoped<ILeccionRepository, LeccionRepository>();
builder.Services.AddScoped<IProgresoRepository, ProgresoRepository>();

builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<ProgressService>();

builder.Services.AddTransient<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, ProyectoPlataformaCursos.Policies.TieneCursosRequirement>();
builder.Services.AddTransient<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, ProyectoPlataformaCursos.Policies.AntiguedadRequirement>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TieneCursosPolicy", policy => policy.Requirements.Add(new ProyectoPlataformaCursos.Policies.TieneCursosRequirement()));
    options.AddPolicy("AntiguedadPolicy", policy => policy.Requirements.Add(new ProyectoPlataformaCursos.Policies.AntiguedadRequirement()));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();

    dbContext.Database.ExecuteSqlRaw("""
        IF COL_LENGTH('Cursos', 'Precio') IS NULL
            ALTER TABLE Cursos ADD Precio decimal(10,2) NOT NULL CONSTRAINT DF_Cursos_Precio DEFAULT(0);
        IF COL_LENGTH('Cursos', 'AceptaEfectivo') IS NULL
            ALTER TABLE Cursos ADD AceptaEfectivo bit NOT NULL CONSTRAINT DF_Cursos_AceptaEfectivo DEFAULT(1);
        IF COL_LENGTH('Cursos', 'AceptaTarjeta') IS NULL
            ALTER TABLE Cursos ADD AceptaTarjeta bit NOT NULL CONSTRAINT DF_Cursos_AceptaTarjeta DEFAULT(1);
        IF COL_LENGTH('Cursos', 'AceptaTransferencia') IS NULL
            ALTER TABLE Cursos ADD AceptaTransferencia bit NOT NULL CONSTRAINT DF_Cursos_AceptaTransferencia DEFAULT(1);
    """);

    dbContext.Database.ExecuteSqlRaw("""
        IF COL_LENGTH('Inscripciones', 'MetodoPago') IS NULL
            ALTER TABLE Inscripciones ADD MetodoPago nvarchar(20) NOT NULL CONSTRAINT DF_Inscripciones_MetodoPago DEFAULT('SIN_COSTE');
        IF COL_LENGTH('Inscripciones', 'ImportePagado') IS NULL
            ALTER TABLE Inscripciones ADD ImportePagado decimal(10,2) NOT NULL CONSTRAINT DF_Inscripciones_ImportePagado DEFAULT(0);
    """);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
