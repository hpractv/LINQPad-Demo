using System;
using Compiled_Library_Demo.Model;
using System.Collections.Generic;

namespace Compiled_Library_Demo
{
    public class Program
    {
        private static void Main(string[] args)
        {
        }

        public static IEnumerable<IAnimal> createZoo(int animalCount) => createZoo(animalCount, () => false);

        public static IEnumerable<IAnimal> createZoo(int animalCount, Func<bool> primateHasTail)
        {
            var animals = new List<IAnimal>();

            for (int i = 0; i < animalCount; i++)
            {
                animals.Add(new Primate(primateHasTail()));
            }

            return animals;
        }

        public static IEnumerable<IAnimal> createZoo(int fishCount, Func<bool> freshWaterFish, int primateCount, Func<bool> primateHasTail)
        {
            var animals = new List<IAnimal>();

            for (int i = 0; i < primateCount; i++)
            {
                animals.Add(new Primate(primateHasTail()));
            }

            for (int i = 0; i < fishCount; i++)
            {
                animals.Add(new Fish(freshWaterFish()));
            }

            return animals;
        }
    }
}