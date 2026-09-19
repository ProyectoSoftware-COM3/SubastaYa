# Decisiones transversales

`services/api.js`, `config.js`, `hooks/useAsyncData.js`, `context/ToastContext.jsx`, `components/common/Toast.jsx`, `utils/dates.js`, `utils/formatters.js`, `components/layout/Navbar.jsx`, `components/common/AuctionImage.jsx`.

## Cliente HTTP

- Todas las llamadas pasan por `fetchWithAuth`: un solo lugar para el token y el manejo de errores, en vez de repetirlo en cada servicio.
- Se interpretan los dos formatos del backend, `{ error }` y `{ errors: [...] }`, porque leyendo uno solo el usuario veia siempre un mensaje generico.
- El error conserva el codigo HTTP, asi cada pantalla reacciona distinto ante `400`, `409` o `422`.

## Carga de datos

- `useAsyncData` concentra el patron cargar, error y reintentar, para no repetir esa logica en cada pantalla.

## Fechas y formatos

- El problema de las fechas se resolvio en el backend con `UtcDateTimeConverter`, que marca como UTC lo que se lee de la base. `utils/dates.js` mantiene la misma normalizacion en el front como resguardo, para que el contador no dependa de que cada respuesta traiga la zona horaria.
- Un unico formato de moneda `es-AR` en `utils/formatters.js`, para que el mismo monto no se vea distinto en cada pantalla.

