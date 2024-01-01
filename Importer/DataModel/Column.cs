using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Importer.DataModel
{
	internal class Column
	{
		[YamlMember(Alias = "title")]
		public string? Title { get; set; }

		[YamlMember(Alias = "view", DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults),
			DefaultValue(ColumnView.DefaultView)]
		public ColumnView View { get; set; } = ColumnView.DefaultView;

		[YamlMember(Alias = "cards")]
		public List<Card>? Cards { get; set; } = new();

		[YamlIgnore()]
		public long Order = 0;

	}
}
