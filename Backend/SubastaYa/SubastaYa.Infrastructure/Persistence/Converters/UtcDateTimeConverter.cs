using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SubastaYa.Infrastructure.Persistence.Converters
{
    // SQL Server (datetime2) no guarda la zona horaria: al leer, EF devuelve las fechas con Kind = Unspecified
    // y la API las serializa sin la "Z", entonces el navegador las interpreta como hora local (UTC-3).
    // Como todo el sistema trabaja en UTC, al leer marcamos cada fecha como UTC.
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter()
            : base(
                // Al guardar: si por algun motivo llega una fecha local, se normaliza a UTC antes de persistir.
                value => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value,
                // Al leer: el valor ya esta en UTC, solo le falta la marca.
                value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
        {
        }
    }
}