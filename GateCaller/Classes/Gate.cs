using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateCaller.Classes
{
    internal class Gate
    {
        public string Name { get; set; }
        public int PhoneNumber { get; set; }

        public Gate(string name, int phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
        }
    }
}
