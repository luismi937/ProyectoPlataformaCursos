# Plataforma de Cursos Online

Sistema de gestión de cursos desarrollados con ASP.NET Core MVC + Entity Framework Core + SQL Server.

## ? Tecnologías

- .NET 10
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Identity

## ?? Ejecutar el Proyecto

### Requisitos:
- SQL Server con la base de datos `PlataformaCursos` creada
- .NET 10 SDK

### Pasos:

1. **La base de datos ya debe existir** con sus tablas en SQL Server

2. **Ejecutar la aplicación:**
```bash
dotnet run
```

O presiona **F5** en Visual Studio

3. **Abrir en el navegador:**
```
https://localhost:7218
```

## ?? Funcionalidades

- ? Registro e inicio de sesión
- ? Roles: Alumno, Profesor
- ? Crear y gestionar cursos (Profesor)
- ? Inscribirse a cursos (Alumno)
- ? Ver lecciones y marcar progreso
- ? Seguimiento de avance por curso

## ?? Configuración

La cadena de conexión está en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=LOCALHOST\\DEVELOPER;Initial Catalog=PlataformaCursos;Integrated Security=True;Trust Server Certificate=True;MultipleActiveResultSets=true"
  }
}
```

---

**El proyecto está completamente funcional y conectado a SQL Server.** ??
