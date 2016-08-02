using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow
{
    public partial class Orm : NativeDb
    {
        internal Orm Id()
        {
            return this;
        }
    }
}
