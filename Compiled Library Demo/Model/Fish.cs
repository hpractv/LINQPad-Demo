using System.Collections.Generic;

namespace Compiled_Library_Demo.Model
{
    public enum waterType { fresh, salt }

    public interface IFish : IAnimal
    {
        bool freshWater { get; }
    }

    public class Fish : IFish
    {
        public Fish(bool freshWater)
        {
            this.type = AnimalType.fish;
            this.freshWater = freshWater;
            this.arms = 0;
            this.legs = 0;
            this.tail = true;
        }

        public bool freshWater{ get; }

        public AnimalType type { get; }

        public int? arms { get; }

        public int? legs { get; }

        public bool tail { get; }
    }
}