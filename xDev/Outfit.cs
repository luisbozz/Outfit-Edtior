using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xDev
{
    class Outfit
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public Outfit(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }
}
