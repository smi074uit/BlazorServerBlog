using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SharedModels.Entities
{
	public class Comment
	{
		public int CommentId { get; set; }
		public int EntryId { get; set; }
		public string CommentBody { get; set; }
		public virtual IdentityUser Owner { get; set; }
		public string OwnerId { get; set; } = "";
	}
}
