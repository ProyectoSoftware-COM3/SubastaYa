# Worker de ciclo de vida

`BackgroundJobs/AuctionClosingWorker.cs`.

- Habilita las subastas programadas que alcanzaron su fecha de inicio: sin ese paso ninguna programada llegaba nunca a admitir pujas, porque `PlaceBid` exige estado `Active`.
- El cierre con ganador liquida en una transaccion el debito al comprador, el credito al vendedor y ambos movimientos del ledger.
- Los cambios de estado quedan auditados con estado anterior y nuevo, como pide el punto 3.4.
- Un conflicto de concurrencia no se reintenta en el momento: la subasta se vuelve a evaluar en el ciclo siguiente con datos frescos.
