using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Exceptions
{
    public class ConcurrencyConflictException : AppException
    {
        public ConcurrencyConflictException(string entity)
            : base($"Conflicto de concurrencia al actualizar {entity}. Otro usuario modifico el recurso primero.") { }
    }
}
