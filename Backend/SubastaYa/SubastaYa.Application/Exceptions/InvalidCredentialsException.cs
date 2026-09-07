using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Exceptions
{
   
    public class InvalidCredentialsException : AppException
    {
        public InvalidCredentialsException()
            : base("Email o contrasena incorrectos.") { }
    }
}
