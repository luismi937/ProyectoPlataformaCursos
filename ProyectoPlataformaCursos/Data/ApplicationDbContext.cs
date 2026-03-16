using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Models;

namespace ProyectoPlataformaCursos.Data
{
    #region DATABASE
    /*
    -- =========================================
    -- CREACIÓN DE BASE DE DATOS
    -- =========================================
    CREATE DATABASE PlataformaCursos;
    GO

    USE PlataformaCursos;
    GO

    -- =========================================
    -- TABLA USUARIOS
    -- =========================================
    CREATE TABLE Usuarios (
        IdUsuario INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(100) NOT NULL,
        Apellidos NVARCHAR(150) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Rol NVARCHAR(20) NOT NULL CHECK (Rol IN ('ALUMNO', 'PROFESOR', 'ADMIN')),
        FechaRegistro DATETIME DEFAULT GETDATE()
    );

    -- =========================================
    -- TABLA CURSOS
    -- =========================================
    CREATE TABLE Cursos (
        IdCurso INT PRIMARY KEY IDENTITY(1,1),
        Titulo NVARCHAR(200) NOT NULL,
        Descripcion NVARCHAR(MAX),
        IdProfesor INT NOT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        Activo BIT DEFAULT 1,

        CONSTRAINT FK_Curso_Profesor 
            FOREIGN KEY (IdProfesor) REFERENCES Usuarios(IdUsuario)
            ON DELETE NO ACTION
    );

    -- =========================================
    -- TABLA LECCIONES
    -- =========================================
    CREATE TABLE Lecciones (
        IdLeccion INT PRIMARY KEY IDENTITY(1,1),
        IdCurso INT NOT NULL,
        Titulo NVARCHAR(200) NOT NULL,
        Contenido NVARCHAR(MAX),
        Orden INT NOT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_Leccion_Curso 
            FOREIGN KEY (IdCurso) REFERENCES Cursos(IdCurso)
            ON DELETE CASCADE
    );

    -- =========================================
    -- TABLA INSCRIPCIONES
    -- =========================================
    CREATE TABLE Inscripciones (
        IdInscripcion INT PRIMARY KEY IDENTITY(1,1),
        IdUsuario INT NOT NULL,
        IdCurso INT NOT NULL,
        FechaInscripcion DATETIME DEFAULT GETDATE(),
        Estado NVARCHAR(20) DEFAULT 'ACTIVO' 
            CHECK (Estado IN ('ACTIVO', 'FINALIZADO', 'CANCELADO')),

        CONSTRAINT FK_Inscripcion_Usuario 
            FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
            ON DELETE CASCADE,

        CONSTRAINT FK_Inscripcion_Curso 
            FOREIGN KEY (IdCurso) REFERENCES Cursos(IdCurso)
            ON DELETE CASCADE,

        CONSTRAINT UQ_Usuario_Curso UNIQUE (IdUsuario, IdCurso)
    );

    -- =========================================
    -- TABLA PROGRESO
    -- =========================================
    CREATE TABLE Progreso (
        IdProgreso INT PRIMARY KEY IDENTITY(1,1),
        IdUsuario INT NOT NULL,
        IdLeccion INT NOT NULL,
        Completado BIT DEFAULT 0,
        FechaUltimaActualizacion DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_Progreso_Usuario 
            FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
            ON DELETE CASCADE,

        CONSTRAINT FK_Progreso_Leccion 
            FOREIGN KEY (IdLeccion) REFERENCES Lecciones(IdLeccion)
            ON DELETE CASCADE,

        CONSTRAINT UQ_Usuario_Leccion UNIQUE (IdUsuario, IdLeccion)
    );
    */
    #endregion

    public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Leccion> Lecciones { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Progreso> Progresos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("IdUsuario");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Rol).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");

                entity.Ignore(e => e.UserName);
                entity.Ignore(e => e.NormalizedUserName);
                entity.Ignore(e => e.NormalizedEmail);
                entity.Ignore(e => e.EmailConfirmed);
                entity.Ignore(e => e.SecurityStamp);
                entity.Ignore(e => e.ConcurrencyStamp);
                entity.Ignore(e => e.PhoneNumber);
                entity.Ignore(e => e.PhoneNumberConfirmed);
                entity.Ignore(e => e.TwoFactorEnabled);
                entity.Ignore(e => e.LockoutEnd);
                entity.Ignore(e => e.LockoutEnabled);
                entity.Ignore(e => e.AccessFailedCount);
            });

            modelBuilder.Entity<Curso>(entity =>
            {
                entity.ToTable("Cursos");
                entity.HasKey(e => e.IdCurso);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)").HasDefaultValue(0);
                entity.Property(e => e.AceptaEfectivo).HasDefaultValue(true);
                entity.Property(e => e.AceptaTarjeta).HasDefaultValue(true);
                entity.Property(e => e.AceptaTransferencia).HasDefaultValue(true);

                entity.HasOne(e => e.Profesor)
                    .WithMany(u => u.Cursos)
                    .HasForeignKey(e => e.IdProfesor)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Leccion>(entity =>
            {
                entity.ToTable("Lecciones");
                entity.HasKey(e => e.IdLeccion);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Contenido).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.PreguntaEvaluacion).HasMaxLength(500).HasDefaultValue("¿Qué has aprendido en esta lección?");
                entity.Property(e => e.RespuestaCorrecta).HasMaxLength(250).HasDefaultValue("OK");
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Curso)
                    .WithMany(c => c.Lecciones)
                    .HasForeignKey(e => e.IdCurso)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Inscripcion>(entity =>
            {
                entity.ToTable("Inscripciones");
                entity.HasKey(e => e.IdInscripcion);
                entity.Property(e => e.FechaInscripcion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("ACTIVO");
                entity.Property(e => e.MetodoPago).HasMaxLength(20).HasDefaultValue("SIN_COSTE");
                entity.Property(e => e.ImportePagado).HasColumnType("decimal(10,2)").HasDefaultValue(0);

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Inscripciones)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Curso)
                    .WithMany(c => c.Inscripciones)
                    .HasForeignKey(e => e.IdCurso)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.IdUsuario, e.IdCurso }).IsUnique();
            });

            modelBuilder.Entity<Progreso>(entity =>
            {
                entity.ToTable("Progreso");
                entity.HasKey(e => e.IdProgreso);
                entity.Property(e => e.Completado).HasDefaultValue(false);
                entity.Property(e => e.FechaUltimaActualizacion).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Progresos)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Leccion)
                    .WithMany(l => l.Progresos)
                    .HasForeignKey(e => e.IdLeccion)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.IdUsuario, e.IdLeccion }).IsUnique();
            });

            modelBuilder.Ignore<IdentityUserLogin<int>>();
            modelBuilder.Ignore<IdentityUserRole<int>>();
            modelBuilder.Ignore<IdentityUserClaim<int>>();
            modelBuilder.Ignore<IdentityUserToken<int>>();
            modelBuilder.Ignore<IdentityRole<int>>();
            modelBuilder.Ignore<IdentityRoleClaim<int>>();
        }
    }
}
