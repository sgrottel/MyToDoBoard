using System.ComponentModel;
using YamlDotNet.Serialization;

namespace MyToDo.StaticDataModel
{
	public class Card
	{
		[YamlMember(Alias = "title")]
		public string? Title { get; set; }

		[YamlMember(
			Alias = "labels",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults,
			SerializeAs = typeof(Utility.StringListAsJson)),
			DefaultValue(null)]
		public List<string>? LabelIds { get; set; }

		[YamlMember(
			Alias = "desc",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults,
			ScalarStyle = YamlDotNet.Core.ScalarStyle.Literal),
			DefaultValue("")]
		public string? Description { get; set; }

		[YamlMember(
			Alias = "date",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public DateTime? Date { get; set; }

		[YamlMember(
			Alias = "modDate",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public DateTime? ModifiedDate { get; set; }

		[YamlMember(
			Alias = "links",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public List<string>? Links { get; set; }

		[YamlMember(
			Alias = "checklist",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public List<CheckListItem>? Checklist { get; set; }

		[YamlMember(
			Alias = "comments",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public List<Comment>? Comments { get; set; }
	}
}
