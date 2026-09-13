# TICKET-04: Middleware y UnitOfWork

## Por que el Middleware nace aca, con Register
DomainException, AppException y ExceptionHandlingMiddleware se crean en este ticket,
junto con el primer Command real (Register). Sus dos primeras excepciones de negocio
(ConcurrencyConflictException, EmailAlreadyRegisteredException) se agregan activas
al switch del Middleware en el mismo momento en que nacen.

## Por que existe IUnitOfWork si Register hace un solo SaveChanges
Un solo SaveChangesAsync ya es atomico por si mismo -- Register no lo necesita para
su propia correctitud. Se establece el patron aca porque es el primer Command
(operacion de escritura) del proyecto, pensando en operaciones futuras que si van
a necesitar coordinar varios cambios en una misma transaccion real.
