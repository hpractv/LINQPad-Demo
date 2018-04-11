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
	/* lots of trips
	var schedules = V_ActiveAllocationSchedule;
		//.ToArray();
	
	var multipleSchedules = schedules
		.GroupBy(s => new { s.personId, s.fundingSourceId })
		.Where(s => s.Count() > 1)
		.Dump();
	*/

	//single trip/fragment
	var multipleSchedules =
		from s in V_ActiveAllocationSchedule
		group s by new { s.personId, s.fundingSourceId }
		into sc
		where sc.Count() > 1
		select new {
			sc.Key.personId,
			sc.Key.fundingSourceId
		};
		
	(
		from ms in multipleSchedules
		join p in Persons on ms.personId equals p.personId
		join pp in Reimbursement_PartnerPersons on p.personId equals pp.personId
		join cs in CampaignSegmentHras on pp.campaignSegmentId equals cs.campaignSegmentId
		join a in HraAdministrators on cs.hraAdministratorId equals a.hraAdministratorId
		select new {
				p.personId,
				p.firstName,
				p.lastName,
				p.loginInformationUserName,
				a.hraAdministratorName
			}
	).Take(1000).Dump();
}

public class logon
{

	public int personId { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string loginInformationUserName { get; set; }
    public string hraAdministratorName  { get; set; }
}
