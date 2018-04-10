namespace Compiled_Library_Demo.Model
{
    public enum AnimalType
    {
        reptile, bird, mammal, fish, primate
    }

    public interface IAnimal
    {
        AnimalType type { get; }
        int? arms { get; }
        int? legs { get; }
        bool tail { get; }
   }
}