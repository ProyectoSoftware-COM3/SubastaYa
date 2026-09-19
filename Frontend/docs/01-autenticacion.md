# Autenticacion y sesion

`api/authApi.js`, `context/AuthContext.jsx`, `context/contexts.js`, `hooks/useAuth.js`, `utils/jwt.js`, `pages/LoginPage.jsx`, `pages/RegisterPage.jsx`.

- La sesion se guarda en `localStorage` en lugar de en memoria, para que una recarga no obligue a volver a loguearse.
- Si falta el usuario guardado pero hay token, se reconstruye desde sus claims: evita quedar con una sesion a medias.
- Al iniciar se descarta el token vencido (claim `exp`) y se programa el cierre automatico al vencer, para no mostrar pantallas vacias por pedidos que van a fallar.
- Un `401` con token enviado cierra la sesion desde un unico lugar; un `401` en el login se trata como credenciales incorrectas y no cierra nada.
- El contexto se separo del provider (`contexts.js`) porque un archivo de componentes que exporta hooks rompe el Fast Refresh de Vite.
- `RegisterPage` replica las reglas del validador del backend antes de llamar a la API, para no gastar una peticion que se sabe que va a fallar.
- Los botones se deshabilitan durante el envio, para evitar registros o logins duplicados por doble clic.
