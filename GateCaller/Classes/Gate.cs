using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateCaller.Classes
{
    public class Gate
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public Gate(string name, string phoneNumber)
        {
            Name = name;
            PhoneNumber = phoneNumber;
        }
    }
}
