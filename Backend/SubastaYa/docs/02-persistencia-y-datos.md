# Persistencia y datos semilla

`SubastaYaDbContext`, `Persistence/Configurations/`, `Persistence/Converters/UtcDateTimeConverter.cs`, `DbSeeder`, `Migrations/`.

- Cada entidad tiene su `IEntityTypeConfiguration` en lugar de configurar todo en el `DbContext`, para que la configuracion crezca sin volverse un archivo unico gigante.
- `UtcDateTimeConverter` se aplica por convencion a todas las propiedades `DateTime`: las columnas `datetime2` no guardan zona horaria, y sin esa marca la API devolvia fechas sin "Z" que el navegador interpretaba como hora local, corriendo los contadores tres horas.
- Se eligio un conversor por convencion, y no anotar cada propiedad, para que ninguna entidad nueva quede afuera por olvido.
- El seed es un metodo que corre en cada arranque y no datos fijos en la migracion, porque el TP pide subastas con tiempos relativos al momento de ejecucion: con fechas fijas dejarian de servir a los pocos dias.
- Existe `comprador3` con una sexta subasta extra para probar cierre y liquidacion sin alterar los saldos exactos que el TP fija para los otros usuarios.
- La contraseña del seed es un hash de BCrypt generado una vez y guardado como constante: son datos de prueba y el login funciona igual, sin depender de `IPasswordHasher` al sembrar.
- Las imagenes del seed son URLs reales porque la consigna pide una URL de imagen y el frontend las muestra directamente.
