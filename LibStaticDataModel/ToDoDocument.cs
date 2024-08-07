﻿using YamlDotNet.Serialization;

namespace MyToDo.StaticDataModel
{
	public class ToDoDocument
	{
		public const string DefaultSchema = "https://go.grottel.net/mytodoboard/schema.yaml";

		[YamlMember(Alias = "$schema")]
		public string Schema { get; set; } = DefaultSchema;

		[YamlMember(
			Alias = "comment",
			DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults,
			ScalarStyle = YamlDotNet.Core.ScalarStyle.Literal)]
		public string? Comment { get; set; }

		[YamlMember(Alias = "labels", DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public List<Label>? Labels { get; set; }

		[YamlMember(Alias = "columns")]
		public List<Column>? Columns { get; set; }
	}
}
