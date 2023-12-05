using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Entities
{
	public class BlogEntryDTO
	{
		public int BlogId { get; set; }
		public string EntryTitle { get; set; }
		public string EntryBody { get; set; }
	}
}
