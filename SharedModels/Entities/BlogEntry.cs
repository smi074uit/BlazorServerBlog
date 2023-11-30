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
	}
}
