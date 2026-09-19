# Concurrencia y auditoria

`UnitOfWork`, `BidRepository`, `AuditLogRepository`, `Persistence/Configurations/`, `docs/concurrency/`.

## Concurrencia optimista

- Cada puja marca tambien la subasta como modificada en `BidRepository.AddAsync`: el control de version solo se aplica si la fila se actualiza, y sin eso dos primeras pujas simultaneas no compartian ninguna fila y se guardaban las dos.
- `UnitOfWork` traduce `DbUpdateConcurrencyException` a `ConcurrencyConflictException`, para que la capa de aplicacion no dependa de tipos de EF Core.

## Auditoria

- La auditoria de una operacion exitosa se escribe dentro de la transaccion: si falla su registro, se revierte tambien la operacion (punto 3.1).
- La auditoria de un intento rechazado se escribe despues del rollback, porque la operacion no debe persistirse pero el intento si tiene que quedar documentado (punto 3.4).
