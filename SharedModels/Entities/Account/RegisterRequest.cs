using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Entities.Account
{
	public class RegisterRequest
	{
		[Required]
		public string Username { get; set; }

		[Required]
		[MinLength(6, ErrorMessage = "Passwords must contain at least 6 characters")]
		public string Password { get; set; }
	}
}
