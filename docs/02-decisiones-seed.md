# TICKET-02: Decisiones sobre el DbSeeder

## Por que es un metodo, no datos fijos en la migracion
El TP pide subastas con tiempos relativos al momento en que se corre la app. Si esas
fechas quedaran fijas, dejarian de cumplir la consigna con el paso de los dias. Por
eso el Seed corre en cada arranque, calculando los tiempos con la hora real.

## Por que existe comprador3
Participa en una sexta subasta extra (vencidaLimpia), pensada para probar el cierre
y la liquidacion, sin tocar los numeros exactos
que el TP exige para los otros usuarios.

## Por que el password es un hash fijo, no IPasswordHasher
Los usuarios del seed son datos de prueba, no reales. El hash (SeedPasswordHash) se
genero una unica vez con BCrypt y se pego como constante -- sigue siendo un hash
valido, el login funciona igual.

Detalle completo de cada punto: comentarios en DbSeeder.cs.
