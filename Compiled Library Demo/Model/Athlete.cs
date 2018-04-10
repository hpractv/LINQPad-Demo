using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiled_Library_Demo.Model
{
    public enum Sport { soccer, basketball, football, rugby, lacrosse }

    public interface IAthlete
    {
         IEnumerable<Sport> sports { get; set; }
    }

    public class Athlete : Person, IAthlete
    {
        public Athlete(IEnumerable<Sport> sports)
            => this.sports = sports.ToList();

        public IEnumerable<Sport> sports { get; set; }

        public void addSport(Sport sport) => ((List<Sport>) this.sports).Add(sport);
    }
}
