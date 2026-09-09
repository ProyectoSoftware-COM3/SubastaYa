# TICKET-12: Decisiones sobre AuditLog, transaccion y LedgerTransaction

## Por que AuditLogRepository tiene su propio SaveChanges
El AuditLog tiene que sobrevivir aunque la operacion de negocio falle: registrar
que "se intento acreditar saldo" es informacion util incluso si el deposito se
revierte por algun error. Si el AuditLog compartiera la misma transaccion que
el deposito (via IUnitOfWork), un Rollback borraria tambien el registro de
auditoria -- perdiendo justo el rastro que 3.4 del TP exige mantener. Por eso
se escribe DESPUES del Commit, con su propio SaveChanges independiente.

## Por que el deposito usa una transaccion explicita
Actualizar el saldo de la Wallet y crear el LedgerTransaction del movimiento
son dos cambios que tienen que quedar juntos o ninguno: si algo falla entre
medio, no puede quedar el saldo actualizado sin su registro contable, ni al
reves. IUnitOfWork garantiza que ambos se confirman o se revierten como una
sola unidad.

## Por que LedgerTransaction no tiene su propio repositorio
Un movimiento (LedgerTransaction) nunca existe por si solo: siempre pertenece
a una Wallet ya creada (tiene WalletId obligatorio). No tiene sentido un
repositorio independiente para algo que no puede existir sin su "dueño" --
por eso GetMovementsAsync y AddMovementAsync viven dentro de IWalletRepository,
no en una interfaz propia.
