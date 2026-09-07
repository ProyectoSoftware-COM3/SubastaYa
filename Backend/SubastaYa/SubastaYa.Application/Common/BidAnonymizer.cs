using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Common
{
    public static class BidAnonymizer
    {
        public static string Anonymize(string name) =>
            name.Length <= 2 ? name : $"{name[0]}***{name[^1]}";
    }
}

