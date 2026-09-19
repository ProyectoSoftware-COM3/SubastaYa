# Arquitectura general

`SubastaYa.Api`, `SubastaYa.Application`, `SubastaYa.Domain`, `SubastaYa.Infrastructure`.

- Los casos de uso se resuelven con MediatR (commands y queries): cada operacion vive en su propia clase, y el controlador solo traduce HTTP a un mensaje.
- La validacion se ejecuta en un `ValidationBehavior` previo al handler, para que ningun caso de uso tenga que repetir la comprobacion de su entrada.
- `IUnitOfWork` se introdujo con el primer command aunque Register no lo necesitaba, porque las operaciones siguientes (puja, deposito, cierre) si requieren coordinar varios cambios en una transaccion.
- `LedgerTransaction` no tiene repositorio propio: un movimiento nunca existe sin su billetera, por eso `AddMovementAsync` y `GetMovementsAsync` viven en `IWalletRepository`.
- `IBidRepository` nace cuando aparece la primera consulta de pujas por usuario (`GetMyBids`): hasta entonces las pujas siempre se leian dentro de una subasta, a traves de `IAuctionRepository`.
- `ICurrentUserService` expone `IsAuthenticated` como propiedad separada para distinguir "no hay usuario" de "hay usuario que nunca pujo", sin forzar `[Authorize]` en un endpoint que debe seguir siendo publico.
