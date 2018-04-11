<Query Kind="SQL">
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
WHERE Person.loginInformationUserName IS NOT NULL;