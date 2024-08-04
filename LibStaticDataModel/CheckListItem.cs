using System.ComponentModel;
using YamlDotNet.Serialization;

namespace MyToDo.StaticDataModel
{
	public class CheckListItem
	{
		[YamlMember(Alias = "text")]
		public string? Text { get; set; }

		[YamlMember(
			Alias = "checked",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public bool? Checked { get; set; }

		[YamlMember(
			Alias = "date",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public DateTime? Date { get; set; }

		[YamlMember(
			Alias = "desc",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults,
			ScalarStyle = YamlDotNet.Core.ScalarStyle.Literal),
			DefaultValue("")]
		public string? Description { get; set; }

	}
}
