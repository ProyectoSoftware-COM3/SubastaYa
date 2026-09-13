#!/bin/bash
# Prueba de concurrencia (4.1 del TP): dos pujas identicas, mandadas en paralelo
# (con & y wait), sobre la misma subasta. El Optimistic Locking (campo Version en
# Auction) debe garantizar que solo una tenga exito (200) y la otra falle con
# 409 Conflict, sin que la base quede en un estado inconsistente.
#
# Como conseguir los datos antes de correr este script:
# 1. Correr la app (F5) y abrir Swagger.
# 2. GET /api/auctions -> copiar el "id" de una subasta con status "Active".
# 3. POST /api/sessions con comprador1@test.com / Test123! -> copiar el "token".
# 4. POST /api/sessions con comprador2@test.com / Test123! -> copiar el "token".
# 5. Pegar los 3 valores abajo, y poner un MONTO mayor al "currentPrice" actual
#    de la subasta + su MinIncrement.

AUCTION_ID="PEGAR_AQUI_EL_ID_DE_UNA_SUBASTA_ACTIVA"
TOKEN_COMPRADOR1="PEGAR_AQUI_EL_TOKEN_DE_COMPRADOR1"
TOKEN_COMPRADOR2="PEGAR_AQUI_EL_TOKEN_DE_COMPRADOR2"
MONTO=17200

echo "Disparando 2 pujas identicas en paralelo..."

curl -s -o resultado1.json -w "Comprador1 -> HTTP %{http_code}\n" -X POST \
  "https://localhost:7080/api/auctions/$AUCTION_ID/bids" \
  -H "Authorization: Bearer $TOKEN_COMPRADOR1" \
  -H "Content-Type: application/json" \
  -d "$MONTO" &

curl -s -o resultado2.json -w "Comprador2 -> HTTP %{http_code}\n" -X POST \
  "https://localhost:7080/api/auctions/$AUCTION_ID/bids" \
  -H "Authorization: Bearer $TOKEN_COMPRADOR2" \
  -H "Content-Type: application/json" \
  -d "$MONTO" &

wait

echo ""
echo "--- Resultado 1 ---"
cat resultado1.json
echo ""
echo "--- Resultado 2 ---"
cat resultado2.json

# Resultado real obtenido (guardado como evidencia, 13/09/2026):
# Comprador1 -> HTTP 200
# Comprador2 -> HTTP 409
#
# --- Resultado 1 ---
# {"id":"37fae286-fc29-4e2f-a170-30e884075bd4","bidderAlias":"vos","amount":17200,"placedAt":"2026-09-13T22:09:02.3562475Z"}
# --- Resultado 2 ---
# {"error":"Otro usuario modifico el recurso al mismo tiempo. Volve a intentar."}
