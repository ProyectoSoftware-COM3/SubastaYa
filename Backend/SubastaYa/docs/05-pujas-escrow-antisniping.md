# Pujas, escrow y anti-sniping

`PlaceBidCommandHandler`, `BidRepository`, `WalletRepository`.

- Toda la puja ocurre dentro de una transaccion explicita: retener el saldo nuevo, liberar el del lider anterior, registrar la puja y sus movimientos tienen que confirmarse juntos o revertirse por completo.
- El handler se dividio en metodos con un unico proposito (validar, retener, liberar, registrar, auditar, notificar) para que cada regla del negocio se pueda leer y defender por separado.
- Se rechaza la puja del propio vendedor: inflaria el precio de su publicacion y podria adjudicarsela sin que exista una venta real.
- Se rechaza la puja de quien ya es lider: subir la propia oferta solo retiene mas saldo sin cambiar el resultado. Puede volver a ofertar cuando otro lo supere.
- Cada validacion lanza una excepcion propia (`AuctionNotActiveException`, `InvalidBidException`, `InsufficientBalanceException`) para que el middleware pueda devolver `400`, `409` o `422` segun el caso, y no un `500` generico.
- El anti-sniping extiende el cierre 2 minutos si la oferta llega dentro de los ultimos 60 segundos, y la extension queda auditada porque cambia una condicion del negocio.
- La notificacion en tiempo real se envia despues del commit: avisar antes podria anunciar una puja que termina revirtiendose.
