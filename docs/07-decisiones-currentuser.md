# TICKET-07: Por que ICurrentUserService nace aca

## Justificacion
GET /api/auctions/{id} es el primer endpoint que necesita leer el usuario
actual desde el token para calcular algo (el indicador Liderando/Superado
del Modulo 3). Es publico -- no exige login -- pero SI el usuario esta
logueado, usa su identidad. Por eso ICurrentUserService.IsAuthenticated
existe como propiedad separada: permite distinguir "no hay usuario" de
"hay usuario pero nunca pujo en esta subasta", sin forzar un [Authorize]
en un endpoint que tiene que seguir siendo publico.
