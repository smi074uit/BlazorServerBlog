using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Entities
{
	public class BlogEntry
	{
		public int BlogEntryId { get; set; }
		public int BlogId { get; set; }
		public string EntryTitle { get; set; }
		public string EntryBody { get; set; }
		public List<Tag> Tags { get; set; } = new();
	}

	public class Tag
	{
		public int TagId { get; set; }
		public string TagName { get; set; }
		public List<BlogEntry> Entries { get; set; } = new();
	}
}
