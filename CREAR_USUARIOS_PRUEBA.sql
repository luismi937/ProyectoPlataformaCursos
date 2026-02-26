-- =========================================
-- CREAR USUARIOS DE PRUEBA
-- =========================================
-- IMPORTANTE: Estos usuarios tienen contraseñas sin hashear
-- Debes registrarlos desde la aplicación para que Identity los hashee correctamente
-- O usar el script de actualización más abajo

USE PlataformaCursos;
GO

-- =========================================
-- NOTA: Para crear usuarios correctamente, usa la aplicación web
-- Los siguientes comandos son solo para referencia
-- =========================================

-- Para ver usuarios existentes
SELECT IdUsuario, Nombre, Apellidos, Email, Rol, FechaRegistro
FROM Usuarios;
GO

-- =========================================
-- CAMBIAR ROL DE UN USUARIO EXISTENTE
-- =========================================
-- Si ya tienes un usuario y quieres hacerlo ADMIN:

-- Ver usuarios
SELECT IdUsuario, Email, Rol FROM Usuarios;

-- Cambiar rol (reemplaza el email con el tuyo)
UPDATE Usuarios
SET Rol = 'ADMIN'
WHERE Email = 'tuEmail@ejemplo.com';
GO

-- Verificar el cambio
SELECT IdUsuario, Email, Rol FROM Usuarios
WHERE Email = 'tuEmail@ejemplo.com';
GO

-- =========================================
-- NOTA IMPORTANTE SOBRE CONTRASEÑAS
-- =========================================
-- Identity usa hashing BCrypt para las contraseñas
-- NO puedes insertar contraseñas en texto plano
-- 
-- PASOS CORRECTOS PARA CREAR USUARIOS:
-- 1. Registrarse en la aplicación web (/Account/Register)
-- 2. Seleccionar el rol deseado
-- 3. Si necesitas cambiar el rol después, usa el UPDATE de arriba
--
-- El usuario debe CERRAR SESIÓN y volver a iniciar para que el cambio de rol se aplique
-- =========================================

-- =========================================
-- ELIMINAR USUARIO (si necesitas reiniciar)
-- =========================================
-- DELETE FROM Usuarios WHERE Email = 'usuario@ejemplo.com';
-- GO

-- =========================================
-- ESTADÍSTICAS RÁPIDAS
-- =========================================
SELECT 
    Rol,
    COUNT(*) as Cantidad
FROM Usuarios
GROUP BY Rol;
GO
