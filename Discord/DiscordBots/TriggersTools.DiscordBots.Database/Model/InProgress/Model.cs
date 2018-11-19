using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TriggersTools.DiscordBots.Database.Model.Special;

namespace TriggersTools.DiscordBots.Database.Model {
	public class Blog {
		public int BlogId { get; set; }
		public string Url { get; set; }

		public List<Post> Posts { get; set; }
	}

	public class Post {
		public int PostId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		public int BlogId { get; set; }
		public Blog Blog { get; set; }
	}

	public class DbMyModel {

		[Key]
		public ulong Id { get; set; }
		
		[Required]
		public string Snowflakes { get; set; } = "";

		private StringSnowflakeList snowflakeList;
		[NotMapped]
		public StringSnowflakeList SnowflakeList =>
			StringSnowflakeList.Create(ref snowflakeList, Snowflakes, v => Snowflakes = v);

	}
}
