# Autenticacion

`UsersController`, `SessionsController`, `RegisterUserCommandHandler`, `LoginCommandHandler`, `Security/JwtTokenGenerator.cs`, `Security/PasswordHasher.cs`.

- Las contraseñas se guardan con BCrypt, que incorpora salt propio: nunca se almacena la contraseña en claro ni un hash sin salt.
- El token JWT incluye `sub`, `email` y `name`, para que el frontend pueda mostrar al usuario sin una consulta adicional.
- `MapInboundClaims = false` evita que ASP.NET renombre los claims: el backend lee los mismos nombres que emite.
- Un email repetido lanza `EmailAlreadyRegisteredException`, que el middleware traduce a `409 Conflict` en lugar de un error generico.
