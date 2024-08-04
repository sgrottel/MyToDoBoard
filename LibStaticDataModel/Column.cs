using System.ComponentModel;
using YamlDotNet.Serialization;

namespace MyToDo.StaticDataModel
{
	public class Column
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
