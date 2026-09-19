# Sala de subasta en vivo

`pages/AuctionRoomPage.jsx`, `hooks/useAuctionLiveUpdates.js`, `components/common/LiveTimer.jsx`, `hooks/useNow.js`.

## Tiempo real

- Se eligio SignalR sobre el hub `/hubs/auctions`, que la consigna marca como diferencial, y se dejo short-polling cada 3 segundos como respaldo automatico para que la sala nunca quede desactualizada si la conexion falla.

## Consola de puja

- Se valida el monto minimo y el saldo disponible antes de enviar, replicando las reglas de `PlaceBidCommandHandler`, como pide la consigna sobre evitar peticiones innecesarias.
- El saldo disponible se muestra en la sala para que el usuario entienda por que se le rechaza una oferta.
- El boton se deshabilita durante el envio: sin eso, un doble clic generaba dos pujas.
- Cada codigo se trata distinto: `422` avisa fondos insuficientes, `409` informa el conflicto y recarga la sala para mostrar el nuevo minimo, `400` muestra el motivo.

## Temporizador

- El contador y las reglas que dependen del tiempo leen la hora de `useNow`, para que ambos usen el mismo instante y no se contradigan.
