<Query Kind="Program">
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\ES.Auth.WpfSample.exe">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\ES.Auth.WpfSample.exe</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\IdentityModel.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\IdentityModel.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\IdentityModel.OidcClient.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\IdentityModel.OidcClient.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.Extensions.Logging.Abstractions.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.Extensions.Logging.Abstractions.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.Extensions.Logging.Debug.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.Extensions.Logging.Debug.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.Extensions.Logging.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.Extensions.Logging.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.IdentityModel.Logging.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.IdentityModel.Logging.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.IdentityModel.Tokens.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Microsoft.IdentityModel.Tokens.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Newtonsoft.Json.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\Newtonsoft.Json.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\System.IdentityModel.Tokens.Jwt.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\System.IdentityModel.Tokens.Jwt.dll</Reference>
  <Reference Relative="..\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\System.Security.Claims.dll">C:\github\authentication-microservice\src\ES.Auth.WpfSample\bin\Debug\System.Security.Claims.dll</Reference>
  <GACReference>System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a</GACReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>RestSharp</NuGetReference>
  <Namespace>analyticsLibrary.dbObjects</Namespace>
  <Namespace>analyticsLibrary.library</Namespace>
  <Namespace>ES.Auth</Namespace>
  <Namespace>ES.Auth.WpfSample</Namespace>
  <Namespace>IdentityModel.Client</Namespace>
  <Namespace>IdentityModel.OidcClient</Namespace>
  <Namespace>IdentityModel.OidcClient.Browser</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>RestSharp</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	var env = environments[environmentEnum.stage];
	var numberOfAccounts = int.Parse(Util.ReadLine("Number of accounts to test?", "1"));

	var sql = $@"
	declare @hraAccounts as table(
		hraAccountId bigint
	)
	
	insert into @hraAccounts(hraAccountId)
	select distinct top {numberOfAccounts} r.hraAccountId
	from HraForms.V_ActiveHraFormFileRecord r
	where r.canRequestMailing = 1;
	
	select distinct
		r.preferredHraAccountHolderPersonId,
		r.accountholderPersonId,
		r.hraAccountId,
		r.hraFormFileRecordId,
		r.documentFormCode,
		r.documentTypeId,
		m.addressId
	from HraForms.V_ActiveHraFormFileRecord r
		inner join @hraAccounts a on r.hraAccountId = a.hraAccountId
		inner join dbo.PersonAddressMap m
			on r.preferredHraAccountHolderPersonId = m.personId
	where r.canRequestMailing = 1
	order by r.preferredHraAccountHolderPersonId, r.hraAccountId";

	sql
		.OnDemand("Expand")
		.Dump("Select SQL");

	var forms = env.db.query(sql)
		.Select(r => new
		{
			preferredhraaccountholderpersonid = r["preferredHraAccountHolderPersonId"].intOrNull(),
			accountholderpersonid = r["accountholderPersonId"].intOrNull(),
			hraaccountid = (long)r["hraAccountId"],
			hraformfilerecordid = r["hraFormFileRecordId"].intOrNull(),
			documentformcode = r["documentFormCode"].intOrNull(),
			documenttypeid = r["documentTypeId"].intOrNull(),
			addressid = r["addressId"].intOrNull(),
		});


	var requests = new List<(int personId, string request)>();

	forms
		.GroupBy(f => f.hraaccountid)
		.forEach(f =>
		{
			var documents = f
			.GroupBy(fd => new { fd.hraaccountid, fd.hraformfilerecordid, fd.documentformcode, fd.documenttypeid })
			.Select(fd => new FormsToMail()
			{
				HraAccountId = fd.Key.hraaccountid,
				DocumentTypeId = fd.Key.documenttypeid.Value,
				DocumentFormCode = fd.Key.documentformcode.Value,
				MailingAddressId = fd.First().addressid.Value
			}).ToArray();

			var request = new Request() { FormsToMail = documents };
			
			requests.Add((personId: f.First().accountholderpersonid.Value, request: request.ToJson()));
		});


	requests.forEach(r =>
	{
		$"Processing Requst for personId: {r.personId}".Dump();
		var authToken = getToken(env, r.personId);

		r.request.OnDemand("Expand").Dump($"Request: {r.personId}");

		var client = new RestClient($"https://{env.name}hraforms.exchange-solutions.tech/api/mailhraform/".Dump("Client"));
		var request = new RestRequest(Method.POST);
		request.AddHeader("Postman-Token", "97575cdf-641e-4ace-b158-42574d5a574c");
		request.AddHeader("Cache-Control", "no-cache");
		request.AddHeader("Authorization", $"Bearer {authToken}");
		request.AddHeader("Content-Type", "application/json");

		request.AddParameter("undefined", r.request, ParameterType.RequestBody);
		IRestResponse response = client.Execute(request);
		response.StatusCode.Dump("Result");

	});
}

private static ObservableCollection<RequestSettings> authSettings => (new NativeHybridViewModel()).AllSettings;

public enum environmentEnum { dev, qa, stage };
private Dictionary<environmentEnum, environment> _environments;
public Dictionary<environmentEnum, environment> environments
{
	get
	{
		if (_environments == null)
		{
			_environments = new Dictionary<environmentEnum, environment>();
			_environments.Add(environmentEnum.dev, new environment() { name = "dev", db = sql_dev.db, auth = authSettings.Single(s => s.ClientId == "ssc" && s.Name == "Dev") });
			_environments.Add(environmentEnum.qa, new environment() { name = "qa", db = sql_qa.db, auth = authSettings.Single(s => s.ClientId == "ssc" && s.Name == "QA") });
			_environments.Add(environmentEnum.stage, new environment() { name = "stage", db = sql_stage.db, auth = authSettings.Single(s => s.ClientId == "ssc" && s.Name == "Stage") });
		}
		return _environments;
	}
}

public class environment
{
	public string name { get; set; }
	public internalDb db { get; set; }
	public RequestSettings auth { get; set; }
}

public string getToken(environment env, int personId)
	=> Util.ReadLine($"Token for person Id: {personId} - Environment: {env.name}");


//app.quitcktype.io json2csharp - newtonsoft.json
public partial class Request
{
	[JsonProperty("FormsToMail")]
	public FormsToMail[] FormsToMail { get; set; }
}

public partial class FormsToMail
{
	[JsonProperty("HraAccountId")]
	public long HraAccountId { get; set; }

	[JsonProperty("DocumentTypeId")]
	public int DocumentTypeId { get; set; }

	[JsonProperty("DocumentFormCode")]
	public int DocumentFormCode { get; set; }

	[JsonProperty("MailingAddressId")]
	public int MailingAddressId { get; set; }
}

public partial class Request
{
	public static Request FromJson(string json) => JsonConvert.DeserializeObject<Request>(json, Converter.Settings);
}

public static class Serialize
{
	public static string ToJson(this Request self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
	public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
	{
		MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
		DateParseHandling = DateParseHandling.None,
		Converters = {
				new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
			},
	};
}