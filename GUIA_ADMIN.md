# ?? GUÍA RÁPIDA: Cómo Usar el Rol de ADMIN

## ? **La Solución Ya Está Aplicada**

Los cambios en `AccountController.cs` ya están listos. Solo necesitas seguir estos pasos:

---

## ?? **Paso 1: Detener y Reiniciar la Aplicación**

1. En Visual Studio, presiona **Shift + F5** para detener el debugger
2. Presiona **F5** para volver a ejecutar la aplicación

---

## ?? **Paso 2: Crear o Modificar Usuario ADMIN**

Tienes **3 opciones**:

### **Opción A: Registrar Nuevo Usuario ADMIN** (Recomendado)

1. Ve a la aplicación en el navegador
2. Click en **"Registrarse"**
3. Llena el formulario:
   - Nombre: `Admin`
   - Apellidos: `Sistema`
   - Email: `admin@test.com`
   - Contraseña: `Admin123` (o la que prefieras, mínimo 6 caracteres)
   - **ROL: Selecciona "Admin"** ?? IMPORTANTE
4. Click en **"Registrarse"**
5. ? Ya eres ADMIN

---

### **Opción B: Cambiar Rol de Usuario Existente**

Si ya tienes un usuario registrado y quieres hacerlo ADMIN:

1. **Abre SQL Server Management Studio**
2. **Ejecuta este comando** (reemplaza el email):

```sql
USE PlataformaCursos;
GO

-- Cambiar tu usuario a ADMIN
UPDATE Usuarios
SET Rol = 'ADMIN'
WHERE Email = 'tu@email.com';  -- ? Pon tu email aquí
GO

-- Verificar
SELECT Email, Rol FROM Usuarios WHERE Email = 'tu@email.com';
```

3. **IMPORTANTE:** Cierra sesión en la aplicación y vuelve a iniciar sesión para que los cambios se apliquen

---

### **Opción C: Insertar Usuario Manualmente (Avanzado)**

**?? PROBLEMA:** No puedes insertar directamente porque la contraseña debe estar hasheada.

**Solución:** Usa la Opción A o B.

---

## ?? **Paso 3: Verificar que Funciona**

### **1. Iniciar Sesión:**
- Ve a `/Account/Login`
- Ingresa tu email y contraseña de ADMIN
- Click en "Iniciar Sesión"

### **2. Verificar el Menú:**
Deberías ver en la barra de navegación:
- ?? Inicio
- ?? Cursos
- ?? **Administración** ?? Este menú SOLO aparece para ADMIN

### **3. Acceder al Panel de Admin:**
- Click en **"Administración"** en el menú
- O ve directamente a: `https://localhost:5001/Admin/Index`
- Deberías ver el **Panel de Administración** con estadísticas

---

## ?? **Qué Puede Hacer un ADMIN**

Una vez dentro, el ADMIN puede:

1. ? **Ver Estadísticas:**
   - Total de usuarios
   - Total de cursos
   - Total de inscripciones

2. ? **Gestionar Usuarios:**
   - Ver lista completa de usuarios
   - Ver roles de cada usuario
   - Eliminar usuarios (excepto otros admins)

3. ? **Gestionar Cursos:**
   - Ver todos los cursos
   - Activar/desactivar cursos
   - Ver detalles de cursos

---

## ?? **Verificar Claims (Opcional)**

Si quieres ver los claims del usuario actual, agrega esto temporalmente en cualquier vista:

```razor
<div class="alert alert-info">
    <h4>Claims del Usuario:</h4>
    @foreach (var claim in User.Claims)
    {
        <p><strong>@claim.Type:</strong> @claim.Value</p>
    }
</div>
```

Deberías ver algo como:
```
http://schemas.microsoft.com/ws/2008/06/identity/claims/role: ADMIN
http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name: Admin Sistema
http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress: admin@test.com
```

---

## ?? **Problemas Comunes**

### **Problema: No veo el menú de Administración**

**Causa:** El rol no está configurado correctamente

**Solución:**
1. Verifica en la BD:
   ```sql
   SELECT Email, Rol FROM Usuarios WHERE Email = 'tu@email.com';
   ```
2. Asegúrate de que dice **`ADMIN`** en mayúsculas
3. Cierra sesión y vuelve a iniciar

---

### **Problema: Dice "Acceso Denegado"**

**Causa:** El rol no se agregó como Claim

**Solución:**
1. Cierra sesión completamente
2. Vuelve a iniciar sesión
3. Los Claims se agregan al iniciar sesión, no al cambiar en la BD

---

### **Problema: No puedo registrar con rol ADMIN**

**Causa:** El formulario de registro tiene un dropdown de roles

**Solución:**
1. Ve a `/Account/Register`
2. En el campo **"Rol"**, selecciona **"Admin"** del dropdown
3. El dropdown tiene 3 opciones:
   - Alumno
   - Profesor
   - Admin ?? Selecciona esta

---

## ?? **Resumen Rápido**

```
1. Detener aplicación (Shift+F5)
2. Volver a ejecutar (F5)
3. Registrarte seleccionando rol "Admin"
   O
   Cambiar rol en BD con UPDATE
4. Iniciar sesión
5. ? Acceder a /Admin/Index
```

---

## ?? **Diferencias entre Roles**

| Rol | Puede Ver Cursos | Puede Crear Cursos | Puede Inscribirse | Panel Admin |
|-----|------------------|-------------------|-------------------|-------------|
| **ALUMNO** | ? | ? | ? | ? |
| **PROFESOR** | ? | ? | ? | ? |
| **ADMIN** | ? | ? | ? | ? |

---

## ?? **¡Listo!**

Ahora puedes:
- ? Registrar usuarios ADMIN
- ? Iniciar sesión como ADMIN
- ? Acceder al panel de administración
- ? Gestionar usuarios y cursos

---

**¿Necesitas ayuda?**
Consulta el archivo `SOLUCION_ROL_ADMIN.md` para detalles técnicos.
