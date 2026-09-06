using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Common.Interfaces
{
    public interface IPasswordHasher
    {   
        string Hash(string plainPassword);
        bool Verify(string plainPassword, string passwordHash);
    }
}
