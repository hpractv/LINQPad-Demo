<Query Kind="Program">
  <Reference Relative="Compiled Library Demo\bin\Debug\Compiled Library Demo.exe">C:\github\LINQPad-Demo\Compiled Library Demo\bin\Debug\Compiled Library Demo.exe</Reference>
  <Namespace>analyticsLibrary.dbObjects</Namespace>
  <Namespace>analyticsLibrary.library</Namespace>
  <Namespace>Compiled_Library_Demo</Namespace>
  <Namespace>Compiled_Library_Demo.Model</Namespace>
</Query>

void Main()
{
	var George = new Primate(true);
	George.legs.Dump("Monkey Legs");
	George.type.Dump("Animal Type");

	var nemo = new Fish(freshWater: true);
	nemo.arms.Dump("Fish Arms");
	nemo.type.Dump("Animal Type");
	
	var zooAnimalCount = 100;

	var zoo = Compiled_Library_Demo.Program.createZoo(zooAnimalCount);
	zoo
		.OnDemand("Expand")
		.Dump("Zoo Animals");
	
	((double)zoo.Count(z => z.tail)/(double)zoo.Count()).ToString("0.00%").Dump("Zoo Tail Count");
	
	var zooMixedTails = Compiled_Library_Demo.Program.createZoo(zooAnimalCount, this.fiftyFifty);
	zooMixedTails
		.OnDemand("Expand")
		.Dump("Zoo Mixed Tails Animals");
	((double)zooMixedTails.Count(z => z.tail)/(double)zooMixedTails.Count()).ToString("0.00%").Dump("Zoo Tail Count");


	var zooMostlyTails = Compiled_Library_Demo.Program.createZoo(zooAnimalCount, this.thirtyThreeSixtySix);
	zooMostlyTails
		.OnDemand("Expand")
		.Dump("Zoo Mostly Tails Animals");
	((double)zooMostlyTails.Count(z => z.tail) / (double)zooMostlyTails.Count()).ToString("0.00%").Dump("Zoo Tail Count");


	var zooFishAndMonkies = Compiled_Library_Demo.Program.createZoo(zooAnimalCount/2, this.thirtyThreeSixtySix, zooAnimalCount/2, this.fiftyFifty);
	zooFishAndMonkies
		.OnDemand("Expand")
		.Dump("Zoo Fish & Monkiess");
		
	zooFishAndMonkies
		.GroupBy(z => z.type)
		.Select(z => new
		{
			type = z.Key,
			tailPercentage = ((double)z.Count(x => x.tail) / (double)z.Count()).ToString("0.00%")
		})
		.Dump("Animal Tail Count");
	
}

public Random rand = new Random();
public bool fiftyFifty() => rand.Next(0, 2) == 1;
public bool thirtyThreeSixtySix()
{
	var hasTail = fiftyFifty();
	if (!hasTail)
		hasTail = fiftyFifty();
		
	return hasTail;
}

