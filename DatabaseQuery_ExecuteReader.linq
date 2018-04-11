<Query Kind="Program">
  <Connection>
    <ID>5b1167bd-8927-424f-8aab-b93bd7b5b1fd</ID>
    <Persist>true</Persist>
    <Server>devsqlag</Server>
    <Database>DevExtendHealth</Database>
    <NoCapitalization>true</NoCapitalization>
    <IncludeSystemObjects>true</IncludeSystemObjects>
  </Connection>
  <Namespace>analyticsLibrary.dbObjects</Namespace>
  <Namespace>analyticsLibrary.library</Namespace>
</Query>

void Main()
{
	var sql = @"
	SELECT TOP 1000
	    Person.personId,
	    Person.firstName,
	    Person.lastName,
	    Person.loginInformationUserName,
	    
	    a.hraAdministratorName
	FROM dbo.Person
	    INNER JOIN
	    (
	        SELECT
	            personId,
	            fundingSourceId
	            FROM Hra.V_ActiveAllocationSchedule
	            GROUP BY personId, fundingSourceId
	            HAVING COUNT(*) > 1
	    ) AS CustomerWithMultipleAllocationSchedules
			ON CustomerWithMultipleAllocationSchedules.personId = Person.personId
	    JOIN Reimbursement.PartnerPerson pp ON CustomerWithMultipleAllocationSchedules.personId = pp.personId
	    JOIN Reimbursement.CampaignSegmentHra cs on cs.campaignSegmentId = pp.campaignSegmentId
	    JOIN Reimbursement.HraAdministrator a on cs.hraAdministratorId = a.hraAdministratorId
	WHERE Person.loginInformationUserName IS NOT NULL
	";
	
	var command = Connection.CreateCommand();
	command.CommandText = sql;

	Connection.Open();
	var reader = command.ExecuteReader();
	var data = new List<logon>();

	while (reader.Read())
	{
		data.Add(new logon()
		{
			personId = reader.GetInt32(0),
			firstName = reader.GetString(1),
			lastName = reader.GetString(2),
			hraAdministratorName = reader.GetString(3),
		});
	}
	
	if(Connection.State == ConnectionState.Open) Connection.Close();
	
	
	data.Dump();
}

public class logon
{

	public int personId { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string loginInformationUserName { get; set; }
    public string hraAdministratorName  { get; set; }
}
