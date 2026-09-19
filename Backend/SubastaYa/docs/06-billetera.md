# Billetera virtual

`WalletController`, `DepositFundsCommandHandler`, `GetWalletBalanceQueryHandler`, `GetWalletMovementsQueryHandler`, `WalletRepository`.

- El deposito usa una transaccion explicita porque actualizar el saldo y crear su `LedgerTransaction` tienen que quedar juntos: nunca puede haber saldo sin respaldo contable, ni al reves.
- La auditoria de la acreditacion se escribe dentro de la transaccion, antes del commit: si falla el registro, el rollback deshace tambien el deposito, como exige el punto 3.1 sobre ACID.
- Cada movimiento se registra con su tipo (`Deposit`, `Hold`, `Release`, `Payment`, `Collection`) para que el historial explique el origen de cada cambio de saldo.
- Los tres saldos se devuelven en un unico `WalletBalanceDto` para que el frontend no tenga que combinarlos ni recalcular la resta.
- El deposito devuelve el saldo actualizado, y no solo un OK.
