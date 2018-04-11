<Query Kind="Program">
  <NuGetReference>CsvHelper</NuGetReference>
  <Namespace>analyticsLibrary.dbObjects</Namespace>
  <Namespace>analyticsLibrary.library</Namespace>
  <Namespace>CsvHelper</Namespace>
</Query>

void Main()
{
	var verdictFile = @"C:\github\LINQPad-Demo\deathpenaltyverdicts.csv";

	var reader = new StreamReader(verdictFile);
	var verdictsCsv = new CsvReader(reader);
	var verdicts = verdictsCsv
		.GetRecords<verdict>();
		//.ToArray(); //try leaving this off

	verdicts
		//.Where(v => v.Penalty == "Death")
		//.Where(v => v.Defendant == "White")
		//.GroupBy(v => new { v.Penalty, v.Defendant, v.Victim })
		//.Select(v => new
		//{
		//	Races = v.Key,
		//	Percentage = ((double)v.Count() / verdicts.Count()).ToString("###.0%"),
		//	//raw = ((double)v.Count() / verdicts.Count())
		//})
		//.OrderByDescending(v => v.Percentage)
		.Dump();
}

class verdict
{
	public string Penalty{ get; set; } 
	public string Victim { get; set; }
	public string Defendant { get; set; }
}