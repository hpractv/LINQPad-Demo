using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiled_Library_Demo.Model
{

    public class Person : IPrimate
    {
        public Person()
        {
            this.type = AnimalType.mammal;
            this.arms = 2;
            this.legs = 2;
            this.tail = false;
        }
        
        public AnimalType type { get; }
        public int? arms { get; }
        public int? legs { get; }
        public bool tail { get; }
    }
}
