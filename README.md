# SubastaYa

Plataforma web de subastas en tiempo real desarrollada para la catedra **Proyecto de Software** (Ingenieria en Informatica, UNAJ).

## Contenido del repositorio

```
SubastaYa/
├── Backend/SubastaYa/          Solucion .NET 8 (API REST + SignalR)
│   ├── SubastaYa.Api/          Controladores, middleware de errores y configuracion
│   ├── SubastaYa.Application/  Casos de uso (CQRS con MediatR), DTOs y validadores
│   ├── SubastaYa.Domain/       Entidades, enums y excepciones de dominio
│   ├── SubastaYa.Infrastructure/  EF Core, repositorios, SignalR y worker
│   ├── SubastaYa.Tests/        Tests unitarios (xUnit)
│   └── docs/                   Decisiones de arquitectura y prueba de concurrencia
└── Frontend/                   Aplicacion React + Vite
```

## Stack tecnologico

| Capa | Tecnologias |
|---|---|
| Backend | .NET 8, ASP.NET Core Web API, MediatR (CQRS), FluentValidation, SignalR |
| Base de datos | SQL Server con Entity Framework Core (Code-First y migraciones) |
| Frontend | React 19, Vite, React Router, Tailwind CSS 4, cliente de SignalR |
| Tests | xUnit |

## Requisitos previos

- .NET SDK 8.0
- SQL Server (LocalDB o una instancia local)
- Node.js 20 o superior
- Visual Studio 2022 o Visual Studio Code

## Puesta en marcha del backend

**1. Configurar la cadena de conexion.**

En `Backend/SubastaYa/SubastaYa.Api/appsettings.json`, la seccion `ConnectionStrings` apunta por defecto a una instancia local:

```json
"DefaultConnection": "Server=localhost;Database=SubastaYa;Trusted_Connection=True;TrustServerCertificate=True;"
```

Si usan otra instancia, reemplacen `Server=localhost` por su nombre de servidor.

**2. Crear la base y aplicar las migraciones.**

Desde Visual Studio, en la **Consola del Administrador de paquetes**, con `SubastaYa.Infrastructure` como proyecto predeterminado:

```powershell
Update-Database
```

Desde la terminal, parados en `Backend/SubastaYa`:

```bash
dotnet ef database update --project SubastaYa.Infrastructure --startup-project SubastaYa.Api
```

El esquema se genera por Code-First: no hace falta ejecutar ningun script SQL.

**3. Confiar el certificado de desarrollo** (solo la primera vez):

```bash
dotnet dev-certs https --trust
```

**4. Levantar la API.**

En Visual Studio, con el perfil **https** seleccionado, presionar F5. Desde la terminal:

```bash
dotnet run --project SubastaYa.Api --launch-profile https
```

La API queda disponible en `https://localhost:7080` y la documentacion **Swagger UI** en `https://localhost:7080/swagger`.

Al iniciar, si la base esta vacia, se cargan automaticamente los datos semilla.

## Puesta en marcha del frontend

Desde la carpeta `Frontend`:

```bash
npm install
npm run dev
```

La aplicacion queda disponible en `http://localhost:5173`, que es el origen habilitado en la politica CORS del backend.

Para apuntar a otra URL de backend, copiar `.env.example` como `.env.local` y editar `VITE_API_BASE_URL`.

Otros comandos:

| Comando | Uso |
|---|---|
| `npm run build` | Genera la version de produccion en `dist/` |
| `npm run lint` | Revisa el codigo con ESLint |
| `npm run preview` | Sirve localmente la version de produccion |

## Datos semilla

Todos los usuarios usan la contraseña `Test123!`.

| Usuario | Saldo total | Retenido | Disponible | Para probar |
|---|---|---|---|---|
| `vendedor@test.com` | $0 | $0 | $0 | Publicacion de subastas |
| `comprador1@test.com` | $150.000 | $45.000 | $105.000 | Postor lider con saldo en garantia |
| `comprador2@test.com` | $200.000 | $0 | $200.000 | Postor habilitado |
| `comprador3@test.com` | $35.000 | $25.000 | $10.000 | Postor con saldo parcialmente retenido |
| `sinfondos@test.com` | $500 | $0 | $500 | Rechazo de puja por fondos insuficientes |

**Categorias:** Tecnologia, Coleccionables, Indumentaria y Vehiculos.

**Subastas de prueba:** una activa estandar que cierra en 25 minutos con dos pujas previas, una activa critica que cierra en 90 segundos para probar la alerta visual y la extension anti-sniping, una programada que inicia en 24 horas, y subastas vencidas con y sin ofertas para probar el cierre y la adjudicacion del worker.

Como la subasta critica cierra 90 segundos despues del primer arranque, conviene recrear la base poco antes de demostrar ese caso.

## Endpoints principales

| Metodo y ruta | Proposito |
|---|---|
| `POST /api/users` | Registro de usuario (201 Created) |
| `POST /api/sessions` | Login, devuelve el token JWT |
| `GET /api/auctions` | Listado con filtros por estado, categoria, rango de precios, orden y paginado |
| `POST /api/auctions` | Creacion de una subasta (201 Created) |
| `GET /api/auctions/{id}` | Detalle, historial de ofertas y monto sugerido |
| `GET /api/auctions/{id}/bids` | Historial de ofertas anonimizado |
| `POST /api/auctions/{id}/bids` | Registro de una oferta (201 Created) |
| `GET /api/categories` | Categorias disponibles |
| `GET /api/wallet/balance` | Saldo total, retenido y disponible |
| `POST /api/wallet/deposit` | Acreditacion de fondos simulados |
| `GET /api/wallet/movements` | Historial de movimientos del libro mayor |
| `GET /api/my-activity/bids` | Subastas en las que participo el usuario |
| `GET /api/my-activity/auctions` | Subastas publicadas por el usuario |
| Hub `/hubs/auctions` | Eventos de tiempo real: `NewBid`, `TimeExtended` y `AuctionClosed` |

Todos los endpoints, salvo el registro y el login, requieren el token JWT en el header `Authorization: Bearer {token}`.

## Reglas de negocio implementadas

**Escrow atomico.** Cuando un usuario realiza la puja mas alta, el monto pasa de su saldo disponible a su saldo retenido. Si otro usuario lo supera, la liberacion del anterior y la retencion del nuevo se ejecutan en una unica transaccion.

**Anti-sniping.** Una oferta valida dentro de los ultimos 60 segundos extiende el cierre 2 minutos, y la extension queda registrada en la auditoria.

**Proceso en segundo plano.** Un worker revisa cada 15 segundos el ciclo de vida de las subastas: habilita las programadas que alcanzaron su fecha de inicio, y cierra las vencidas liquidando el saldo del comprador al vendedor, o dejandolas desiertas si no recibieron ofertas. Cada subasta se procesa en su propio contexto y transaccion, de modo que una falla no afecta a las demas.

**Concurrencia optimista.** Las entidades incluyen un campo `Version` controlado por EF Core. Cada puja actualiza tambien la fila de la subasta, de manera que dos ofertas simultaneas sobre la misma subasta compiten por la misma version y la segunda se rechaza con `409 Conflict`.

**Auditoria.** Se registran de forma inmutable los cambios de estado ejecutados por el worker, las extensiones anti-sniping, los intentos de puja rechazados y las acreditaciones manuales de saldo. En las operaciones exitosas la auditoria se escribe dentro de la transaccion, de modo que un fallo al registrarla revierte tambien la operacion. En los intentos rechazados se registra despues del rollback, ya que la operacion no debe persistirse pero el intento si debe quedar documentado.

**Validaciones de puja.** No se admiten ofertas del propio vendedor, ni de quien ya es el postor lider, ni montos inferiores al minimo, ni pujas sin saldo disponible suficiente. Cada rechazo devuelve el codigo HTTP correspondiente: `400`, `409` o `422`.

**Tiempo real.** La sala de subasta se sincroniza por WebSockets mediante SignalR. Si la conexion no se puede establecer o se interrumpe, el frontend pasa automaticamente a short-polling cada 3 segundos, y la propia sala indica en que modo esta operando.

## Prueba de concurrencia

Se verifico que dos pujas identicas enviadas en paralelo sobre la misma subasta resulten en una sola puja registrada y un `409 Conflict` para la otra.

El script se encuentra en `Backend/SubastaYa/docs/concurrency/test-concurrencia.sh` y envia las dos peticiones en paralelo con `&` y `wait`:

```bash
curl -s -o resultado1.json -w "Comprador1 -> HTTP %{http_code}\n" -X POST \
  "https://localhost:7080/api/auctions/$AUCTION_ID/bids" \
  -H "Authorization: Bearer $TOKEN_COMPRADOR1" \
  -H "Content-Type: application/json" \
  -d "$MONTO" &

curl -s -o resultado2.json -w "Comprador2 -> HTTP %{http_code}\n" -X POST \
  "https://localhost:7080/api/auctions/$AUCTION_ID/bids" \
  -H "Authorization: Bearer $TOKEN_COMPRADOR2" \
  -H "Content-Type: application/json" \
  -d "$MONTO" &

wait
```

**Como ejecutarlo:**

1. Levantar la API y obtener, desde Swagger, el `id` de una subasta activa y los tokens de `comprador1@test.com` y `comprador2@test.com`.
2. Completar `AUCTION_ID`, `TOKEN_COMPRADOR1`, `TOKEN_COMPRADOR2` y un `MONTO` mayor o igual al minimo requerido.
3. Ejecutar `bash test-concurrencia.sh` desde la carpeta `docs/concurrency`.

**Resultado obtenido:**

```
Comprador1 -> HTTP 200
Comprador2 -> HTTP 409

--- Resultado 1 ---
{"id":"37fae286-fc29-4e2f-a170-30e884075bd4","bidderAlias":"vos","amount":17200,"placedAt":"2026-09-13T22:09:02.3562475Z"}
--- Resultado 2 ---
{"error":"Otro usuario modifico el recurso al mismo tiempo. Volve a intentar."}
```

Las respuestas completas quedaron guardadas en `resultado1.json` y `resultado2.json`. En la base se registro una unica puja, y el intento rechazado quedo asentado en la tabla `AuditLogs` con la accion `PujaRechazada`.

## Tests

Desde `Backend/SubastaYa`:

```bash
dotnet test
```

Tambien se pueden ejecutar desde Visual Studio con **Prueba → Ejecutar todas las pruebas**. Los tests cubren los handlers y validadores de los casos de uso.

## Documentacion adicional

Las decisiones de arquitectura estan documentadas en `Backend/SubastaYa/docs/`.
