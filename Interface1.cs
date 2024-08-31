using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letra_T
{
    interface Interface1
    {
        void Add(string key, Interface1 clase);
        Interface1 Get(string key);
        void Delete(string key);
    }
}
