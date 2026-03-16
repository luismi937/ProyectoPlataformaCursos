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
builder.Services.AddScoped<StripePaymentService>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

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

    dbContext.Database.ExecuteSqlRaw("""
        IF COL_LENGTH('Lecciones', 'PreguntaEvaluacion') IS NULL
            ALTER TABLE Lecciones ADD PreguntaEvaluacion nvarchar(500) NOT NULL CONSTRAINT DF_Lecciones_PreguntaEvaluacion DEFAULT('¿Qué has aprendido en esta lección?');
        IF COL_LENGTH('Lecciones', 'RespuestaCorrecta') IS NULL
            ALTER TABLE Lecciones ADD RespuestaCorrecta nvarchar(250) NOT NULL CONSTRAINT DF_Lecciones_RespuestaCorrecta DEFAULT('OK');
    """);

    dbContext.Database.ExecuteSqlRaw("""
        UPDATE Lecciones SET PreguntaEvaluacion = '¿Qué has aprendido en esta lección?' WHERE PreguntaEvaluacion IS NULL OR LTRIM(RTRIM(PreguntaEvaluacion)) = '';
        UPDATE Lecciones SET RespuestaCorrecta = 'OK' WHERE RespuestaCorrecta IS NULL OR LTRIM(RTRIM(RespuestaCorrecta)) = '';
    """);

    await SeedDefaultDataAsync(dbContext);
}

static async Task SeedDefaultDataAsync(ApplicationDbContext dbContext)
{
    var admin = await CreateUserIfNotExistsAsync(dbContext, "admin@plataforma.com", "Admin", "Sistema", "ADMIN", "123456");
    var profesor = await CreateUserIfNotExistsAsync(dbContext, "profesor@plataforma.com", "Carlos", "Profesor", "PROFESOR", "123456");
    await CreateUserIfNotExistsAsync(dbContext, "alumno@plataforma.com", "Laura", "Alumno", "ALUMNO", "123456");

    await EnsureDefaultCourseAsync(
        dbContext,
        "Fundamentos de C#",
        "Curso inicial para aprender sintaxis, POO y buenas prácticas en C#.",
        profesor.Id,
        0,
        false,
        false,
        false,
        DateTime.Now.AddDays(-10));

    await EnsureDefaultCourseAsync(
        dbContext,
        "ASP.NET Core desde cero",
        "Aprende a crear aplicaciones web modernas con ASP.NET Core y Razor.",
        profesor.Id,
        49.99m,
        true,
        true,
        true,
        DateTime.Now.AddDays(-8));

    await EnsureDefaultCourseAsync(
        dbContext,
        "Arquitectura y Seguridad en .NET",
        "Diseño por capas, autenticación y autorización aplicadas a proyectos reales.",
        admin.Id,
        79.99m,
        false,
        true,
        true,
        DateTime.Now.AddDays(-6));

    var cursosConSemilla = await dbContext.Cursos
        .Where(c => c.Titulo == "Fundamentos de C#"
            || c.Titulo == "ASP.NET Core desde cero"
            || c.Titulo == "Arquitectura y Seguridad en .NET")
        .ToListAsync();

    foreach (var curso in cursosConSemilla)
    {
        var tieneLecciones = await dbContext.Lecciones.AnyAsync(l => l.IdCurso == curso.IdCurso);
        if (tieneLecciones)
        {
            continue;
        }

        dbContext.Lecciones.AddRange(
            new Leccion
            {
                IdCurso = curso.IdCurso,
                Titulo = $"Introducción a {curso.Titulo}",
                Contenido = "En esta lección veremos objetivos, estructura y metodología del curso.",
                PreguntaEvaluacion = "Escribe la palabra INTRODUCCION para continuar",
                RespuestaCorrecta = "INTRODUCCION",
                Orden = 1,
                FechaCreacion = DateTime.Now
            },
            new Leccion
            {
                IdCurso = curso.IdCurso,
                Titulo = "Módulo práctico",
                Contenido = "Desarrollo práctico con ejercicios guiados y ejemplos aplicados.",
                PreguntaEvaluacion = "¿Cuál es la palabra clave para crear una clase en C#?",
                RespuestaCorrecta = "class",
                Orden = 2,
                FechaCreacion = DateTime.Now
            },
            new Leccion
            {
                IdCurso = curso.IdCurso,
                Titulo = "Proyecto final",
                Contenido = "Construcción de un mini proyecto para consolidar todo lo aprendido.",
                PreguntaEvaluacion = "Escribe FINAL para completar el curso",
                RespuestaCorrecta = "FINAL",
                Orden = 3,
                FechaCreacion = DateTime.Now
            });
    }

    await dbContext.SaveChangesAsync();
}

static async Task<Usuario> CreateUserIfNotExistsAsync(
    ApplicationDbContext dbContext,
    string email,
    string nombre,
    string apellidos,
    string rol,
    string password)
{
    var existingUser = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    if (existingUser != null)
    {
        return existingUser;
    }

    var usuario = new Usuario
    {
        Nombre = nombre,
        Apellidos = apellidos,
        Email = email,
        UserName = email,
        NormalizedUserName = email.ToUpperInvariant(),
        NormalizedEmail = email.ToUpperInvariant(),
        Rol = rol,
        FechaRegistro = DateTime.Now,
        SecurityStamp = Guid.NewGuid().ToString()
    };

    var hasher = new PasswordHasher<Usuario>();
    usuario.PasswordHash = hasher.HashPassword(usuario, password);

    dbContext.Usuarios.Add(usuario);
    await dbContext.SaveChangesAsync();
    return usuario;
}

static async Task EnsureDefaultCourseAsync(
    ApplicationDbContext dbContext,
    string titulo,
    string descripcion,
    int idProfesor,
    decimal precio,
    bool aceptaEfectivo,
    bool aceptaTarjeta,
    bool aceptaTransferencia,
    DateTime fechaCreacion)
{
    var exists = await dbContext.Cursos.AnyAsync(c => c.Titulo == titulo);
    if (exists)
    {
        return;
    }

    dbContext.Cursos.Add(new Curso
    {
        Titulo = titulo,
        Descripcion = descripcion,
        IdProfesor = idProfesor,
        Activo = true,
        Precio = precio,
        AceptaEfectivo = aceptaEfectivo,
        AceptaTarjeta = aceptaTarjeta,
        AceptaTransferencia = aceptaTransferencia,
        FechaCreacion = fechaCreacion
    });

    await dbContext.SaveChangesAsync();
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
