# Base de Datos - Plataforma de Cursos

Este archivo contiene el script SQL completo para crear la base de datos del proyecto.

---

## #region DATABASE

```sql
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
```

## #endregion

---

## Notas Importantes

1. **La base de datos debe existir antes de ejecutar la aplicación**
2. La aplicación NO creará la base de datos automáticamente
3. El proyecto usa Entity Framework Core para mapear las tablas existentes
4. Las relaciones están configuradas con Fluent API en `ApplicationDbContext.cs`

## Instrucciones de Uso

### Opción 1: Ejecutar desde SQL Server Management Studio (SSMS)
1. Abrir SSMS
2. Conectar al servidor: `LOCALHOST\DEVELOPER`
3. Abrir una nueva consulta
4. Copiar y pegar el script SQL completo
5. Ejecutar (F5)

### Opción 2: Usar el archivo CreateDatabase.sql
```bash
sqlcmd -S LOCALHOST\DEVELOPER -U SA -i CreateDatabase.sql
```

### Verificar la Creación
```sql
USE PlataformaCursos;
GO

SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
```

Deberías ver las siguientes tablas:
- Usuarios
- Cursos
- Lecciones
- Inscripciones
- Progreso

## Connection String
```
Data Source=LOCALHOST\DEVELOPER;Initial Catalog=PlataformaCursos;Persist Security Info=True;User ID=SA;Trust Server Certificate=True
```

---

**Fecha de Creación:** $(Get-Date)  
**Autor:** Sistema de Gestión de Cursos  
**Versión:** 1.0
