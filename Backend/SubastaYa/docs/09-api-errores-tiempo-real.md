# API, errores y tiempo real

`Program.cs`, `Middleware/ExceptionHandlingMiddleware.cs`, `RealTime/AuctionHub.cs`, `RealTime/AuctionNotifier.cs`.

## API y errores

- Se crean recursos con `201 Created` y se consultan con `200`, para que el codigo refleje lo que realmente ocurrio.

## Tiempo real

- `IAuctionNotifier` se declara en la capa de aplicacion e implementa en infraestructura, para que los handlers no dependan de SignalR.

