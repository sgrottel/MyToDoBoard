using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Importer.DataModel
{
	internal class Comment
	{
		[YamlMember(Alias = "text")]
		public string? Text { get; set; }

		[YamlMember(
			Alias = "date",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public DateTime? Date { get; set; }
	}
}
