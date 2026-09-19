# Subastas y catalogo

`AuctionsController`, `GetAuctionsQueryHandler`, `GetAuctionByIdQueryHandler`, `CreateAuctionCommandHandler`, `AuctionRepository`, `Common/Filters/AuctionFilter.cs`, DTOs.

- Los filtros viajan en un `AuctionFilter` en lugar de multiplicar parametros en el repositorio.
- El listado usa `AuctionCardDto` y el detalle `AuctionDetailDto`: la card no necesita descripcion ni historial, y enviarlos encareceria cada consulta del catalogo.
- El detalle incluye `suggestedNextBid` e `isCurrentUserLeading` porque son datos que dependen del usuario que consulta y que el frontend no puede deducir por su cuenta.
- La creacion valida fechas y montos con FluentValidation antes de llegar al handler, y devuelve `201 Created` con el id de la subasta.
