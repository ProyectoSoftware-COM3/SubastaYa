# Catalogo y exploracion

`pages/HomePage.jsx`, `pages/ActiveAuctionsPage.jsx`, `pages/UpcomingAuctionsPage.jsx`, `components/common/AuctionCard.jsx`, `components/common/Pagination.jsx`, `utils/auctionStatus.js`.

- El filtrado se delega al backend por parametros de `GET /api/auctions`.
- Los filtros viven en la URL (`useSearchParams`) para que sobrevivan a una recarga, se puedan compartir y el boton "atras" funcione.
- Finalizadas y Desiertas se ofrecen por separado porque el endpoint acepta un solo estado por consulta y en el dominio son estados distintos.
- Las categorias salen de `GET /api/categories` y no de las subastas visibles, porque de lo contrario una categoria sin subastas en la pagina actual desapareceria del filtro.
- En las subastas proximas el contador va hasta el cierre porque el listado no incluye la fecha de inicio: esa solo viene en el detalle.
- Solo las activas no vencidas muestran "Pujar": ofrecer la accion en los demas estados llevaria a una peticion que el backend rechaza.
- Se distinguen carga, error con reintento y listado vacio, para que una falla no se confunda con "no hay resultados".
