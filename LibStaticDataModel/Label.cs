﻿using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace MyToDo.StaticDataModel
{
	public class Label
	{
		[YamlMember(Alias = "id")]
		public string? Id { get; set; }

		[YamlMember(Alias = "title")]
		public string? Title { get; set; }

		[YamlMember(Alias = "desc", DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public string? Description { get; set; }

		[YamlMember(Alias = "color", DefaultValuesHandling = DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)]
		public string? Color { get; set; }

		public void GenerateId(List<Label> labels)
		{
			string id = MakeTitleUpperChars();

			if (TestId(id, labels))
			{
				Id = id;
				return;
			}

			id = MakeTitleUpperCharsPlus();

			if (TestId(id, labels))
			{
				Id = id;
				return;
			}

			do
			{
				Id = new Guid().ToString();
			} while (!TestId(Id, labels));
		}

		private string MakeTitleUpperChars()
		{
			return new((Title ?? string.Empty).Where(char.IsUpper).ToArray());
		}

		private string MakeTitleUpperCharsPlus()
		{
			Regex r = new Regex(@"[A-Z][a-z]");
			var matches = r.Matches(Title ?? string.Empty);
			if (matches == null) return string.Empty;
			return string.Join("", matches.Where((m) => m.Success).Select((m) => m.Value));
		}

		static public bool TestId(string id, List<Label> labels)
		{
			if (string.IsNullOrEmpty(id)) return false;

			foreach (Label l in labels)
			{
				if (string.Equals(l.Id, id, StringComparison.OrdinalIgnoreCase)) return false;
			}

			return true;
		}
	}
}
