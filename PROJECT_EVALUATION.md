# Evaluacion tecnica del proyecto XClone-Back

Fecha de evaluacion: 27 de abril de 2026
Perfil evaluador: Senior .NET

## Resultado general

Calificacion global: **6.3 / 10**

Estado: **Buen potencial de arquitectura, pero con deuda tecnica relevante en testing, consistencia de autenticacion y madurez de entrega (CI/documentacion).**

## Rubrica de evaluacion

| Area | Peso | Nota | Resultado ponderado |
|---|---:|---:|---:|
| Arquitectura y separacion por capas | 20% | 8.0 | 1.60 |
| Calidad de codigo y mantenibilidad | 20% | 6.5 | 1.30 |
| Seguridad (Auth, manejo de errores, hardening) | 20% | 5.5 | 1.10 |
| Testing y confiabilidad | 20% | 3.0 | 0.60 |
| Operacion/DevOps (config, CI, estandares) | 20% | 3.5 | 0.70 |
| **Total** | **100%** |  | **6.30 / 10** |

## Fortalezas

- Arquitectura por capas bien definida (`Domain`, `Application`, `Infrastructure`, `AppWeb`, `Test`).
- Uso de inyeccion de dependencias y extensiones por modulo (`AddApplication`, `AddInfrastructure`, `AddAppWeb`).
- Middleware global para errores centraliza respuestas y evita leaks directos de excepciones.
- JWT configurado con validaciones relevantes (`issuer`, `audience`, `lifetime`, `signing key`, `ClockSkew = 0`).
- Uso de rate limiting y SignalR; demuestra preocupacion por proteccion y capacidades en tiempo real.
- Logging centralizado con Serilog y sinks de consola + MongoDB.

## Hallazgos clave (prioridad alta)

### 1) Inconsistencia de cookie de acceso (riesgo funcional alto)

- En `AuthController`, el login escribe cookie `accessToken`.
- En `JwtBearerEvents`, la lectura busca `access_token`.
- Impacto: autenticacion por cookie puede fallar sistematicamente aunque el login sea exitoso.

Evidencia:
- `AppWeb/Controllers/AuthController.cs`
- `Infrastructure/DependencyInjection.cs`

### 2) Errores de autenticacion terminan como 500 en lugar de 401

- `LoginHandler` lanza `UnauthorizedAccessException`.
- `ErrorHandlerMiddleware` no tiene catch especifico para `UnauthorizedAccessException`.
- Impacto: semantica HTTP incorrecta, DX/UX pobre en cliente y observabilidad engañosa.

Evidencia:
- `Application/Modules/Auth/Login/LoginHandler.cs`
- `AppWeb/Middlewares/ErrorHandlerMiddleware.cs`

### 3) Suite de tests desactualizada y no compilable

- `dotnet test Test/Test.csproj` falla por namespaces inexistentes:
  - `Application.Modules.Users.CreateUser`
  - `Application.Modules.Users.UpdateUser`
- El codigo real contiene `Application.Modules.Users.UpdateProfile`.
- Impacto: pipeline de calidad roto, regresiones no detectadas.

Evidencia:
- `Test/Application/Users/CreateUserTest.cs`
- `Test/Application/Users/UpdateUserTest.cs`
- `Application/Modules/Users/UpdateProfile/UpdateProfileHandler.cs`

## Hallazgos secundarios

- Nombres inconsistentes en DTOs/archivos (`Respond` vs `Response`, typo en `ChangePasswordCommnad`) reducen legibilidad y mantenibilidad.
- `XDbContext` monolitico y muy grande: costo alto de mantenimiento manual y revisiones.
- Falta de artefactos de madurez de equipo: no se detecta `README`, `CI workflows`, `.editorconfig`, `global.json`.
- `appsettings` minimalista sin estructura de opciones operativas visibles (aunque varias settings llegan por env vars).

## Riesgo actual por area

- Riesgo funcional: **Medio-Alto**
- Riesgo de seguridad: **Medio**
- Riesgo de regresiones: **Alto**
- Riesgo operativo (onboarding/release): **Alto**

## Recomendaciones priorizadas

### Prioridad 1 (esta semana)

1. Corregir la inconsistencia del nombre de cookie (`accessToken` vs `access_token`) en una sola convencion.
2. Mapear `UnauthorizedAccessException` a HTTP 401 en el middleware global.
3. Reparar tests para que compilen con los namespaces/clases actuales y ejecutar en cada push.

### Prioridad 2 (2-3 semanas)

1. Estandarizar nomenclatura (`Response`, `Command`) y corregir typos para reducir ruido cognitivo.
2. Separar configuraciones de EF Core (entity configurations por tabla) para reducir acoplamiento en `XDbContext`.
3. Incorporar `README` tecnico con arquitectura, setup local, variables de entorno y comandos (`build`, `test`, `run`).

### Prioridad 3 (1 mes)

1. Agregar CI basico (restore/build/test) y reporte de cobertura.
2. Agregar `.editorconfig` y analisis estatico (warnings como errores en CI para codigo nuevo).
3. Definir una estrategia de pruebas minima por modulo (unitarias + integracion para auth y endpoints criticos).

## Veredicto senior

El proyecto esta bien encaminado en arquitectura y separacion de responsabilidades. Sin embargo, hoy no alcanza una nota alta porque la base de testing esta rota y hay inconsistencias en autenticacion/manejo de errores que impactan comportamiento real del sistema. Si se ejecutan las mejoras de Prioridad 1 y 2, este backend puede subir rapidamente al rango **7.5 - 8.2 / 10**.
