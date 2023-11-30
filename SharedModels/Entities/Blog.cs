using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SharedModels.Entities
{
	public class Blog
	{
		public int BlogId { get; set; }
		public string BlogTitle { get; set; }
		public string? Description { get; set; }
		public bool Locked { get; set; }
		public virtual IdentityUser Owner { get; set; }
		public string OwnerId { get; set; } = "";
	}
}
