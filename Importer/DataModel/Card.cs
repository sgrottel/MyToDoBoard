using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Importer.DataModel
{
	internal class Card
	{
		[YamlMember(Alias = "title")]
		public string? Title { get; set; }

		[YamlMember(
			Alias = "labels",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults,
			SerializeAs = typeof(YamlWriter.StringListAsJson)),
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
	}
}
