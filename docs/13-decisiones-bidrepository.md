# TICKET-13: Por que IBidRepository nace aca

## Por que no antes
En los tickets anteriores (GetAuctionById, GetAuctionBids), las pujas siempre
se consultaban DENTRO del contexto de una subasta especifica -- se accedia a
traves de IAuctionRepository (GetByIdWithBidsAsync), no necesitaban un
repositorio propio.

## Por que aca
GetMyBids necesita algo distinto: todas las pujas de UN USUARIO, sin importar
en que subasta -- una consulta por BidderId, no por AuctionId. Ese tipo de
acceso no tiene sentido resolverlo "a traves de" IAuctionRepository (pensado
para buscar por subasta), por eso se justifica IBidRepository como interfaz
propia.
