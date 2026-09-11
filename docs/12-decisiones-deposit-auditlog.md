# TICKET-12: Decisiones sobre AuditLog, transaccion y LedgerTransaction

## Por que AuditLogRepository tiene su propio SaveChanges
El AuditLog usa un SaveChanges independiente, separado del IUnitOfWork de la
transaccion principal. Esto es lo que permite, en general, que la escritura
del audit log no dependa del resultado de la operacion de negocio -- en este
Handler puntual (DepositFundsCommandHandler), el AuditLog se escribe una vez
que el deposito ya se confirmo (despues del Commit), registrando la
acreditacion exitosa tal como exige 3.4 del TP.

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
