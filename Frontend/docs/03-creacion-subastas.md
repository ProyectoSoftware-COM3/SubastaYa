# Creacion de subastas

`pages/CreateAuctionPage.jsx`, `services/auctionService.js`, `utils/validation.js`, `utils/dates.js`.

- El formulario replica las reglas de `CreateAuctionCommandValidator` para no enviar datos que se sabe que van a ser rechazados.
- Se valida en pantalla que el fin sea posterior al inicio, como exige el Modulo 2: dejarlo solo en el backend obligaba al usuario a enviar el formulario para enterarse.
- Se tolera 5 minutos hacia atras en la fecha de inicio, porque "Empezar ahora" quedaria invalido mientras el vendedor completa el resto del formulario.
- El boton se deshabilita durante el envio: sin eso, un doble clic creaba dos subastas.
