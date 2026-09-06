using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Domain.Exceptions
{
    public class EmailAlreadyRegisteredException : DomainException
    {
        public EmailAlreadyRegisteredException(string email)
            : base($"El email {email} ya esta registrado.") { }
    }
}
