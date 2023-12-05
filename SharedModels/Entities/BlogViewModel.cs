using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Entities
{
	public class BlogViewModel
	{
		public Blog Blog { get; set; }
		public List<BlogEntry> BlogEntries { get; set; }
		public List<Comment> Comments { get; set; }
	}
}
