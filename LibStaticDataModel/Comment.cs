using YamlDotNet.Serialization;

namespace MyToDo.StaticDataModel
{
	public class Comment
	{
		[YamlMember(Alias = "text")]
		public string? Text { get; set; }

		[YamlMember(
			Alias = "date",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public DateTime? Date { get; set; }
	}
}
