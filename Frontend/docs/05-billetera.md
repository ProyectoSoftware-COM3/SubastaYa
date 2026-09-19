# Billetera virtual

`pages/WalletPage.jsx`, `services/walletService.js`.

- Tras acreditar se usa el saldo que devuelve el backend y se recarga el historial, para no dejar la pantalla desactualizada ni pedir una recarga manual.
- El deposito valida el monto antes de llamar a la API, y el boton se deshabilita durante el envio.
