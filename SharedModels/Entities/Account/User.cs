using Microsoft.AspNetCore.Identity;

namespace SharedModels.Entities.Account
{
	public class User
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string Passwd { get; set; }
		public string Role { get; set; }
		public string Token { get; set; }

		//public string FirstName { get; set; }
		//public string SurName { get; set; }

	}
}
