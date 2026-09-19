using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Exceptions
{
    public class CategoryNotFoundException : AppException
    {
        public CategoryNotFoundException(Guid categoryId)
            : base($"No se encontro la categoria {categoryId}.") { }
    }
}
