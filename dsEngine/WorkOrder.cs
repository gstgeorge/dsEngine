using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dsEngine
{
    internal class WorkOrder
    {
        public SortedSet<Vehicle> Vehicles { get; } = new SortedSet<Vehicle>();
    }
}
