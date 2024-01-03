using Importer.Trello;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Importer
{
	internal static class DumpUtility
	{
		public static void Dump(Board board)
		{
			//IEnumerable<Attachment?> attachments = Array.Empty<Attachment?>();
			//foreach (Card c in board.cards ?? Array.Empty<Card>())
			//{
			//	if (c.attachments == null) continue;
			//	attachments = attachments.Concat(c.attachments);
			//}

			//DumpAsCSharpClass(attachments, "Attachment");

			//HashSet<string> actionTypes = new();
			//List<object?> actionData = new();
			//foreach (CardAction ca in board.actions ?? Array.Empty<CardAction>())
			//{
			//	//actionTypes.Add(ca.type);
			//	if (ca.data != null)
			//	{
			//		actionData.Add(ca.data);
			//	}
			//}
			//DumpAsCSharpClass(actionData, "ActionData");
			//foreach (string t in actionTypes) Console.WriteLine(t);

			//HashSet<CheckItemState> ciStates = new();
			//foreach (CheckList cl in board.checklists ?? Array.Empty<CheckList>())
			//{
			//	foreach (CheckItem ci in cl.checkItems ?? Array.Empty<CheckItem>())
			//	{
			//		ciStates.Add(ci.state);
			//	}
			//}
		}

		private static void DumpAsCSharpClass(object? o, string className)
		{
			if (o == null) throw new ArgumentNullException(nameof(o));
			JsonElement? je = o as JsonElement?;
			if (je == null) throw new ArgumentException("Object must be a JsonElement", nameof(o));

			Console.WriteLine($"internal class {className}");
			Console.WriteLine("{");
			foreach (JsonProperty p in je.Value.EnumerateObject())
			{
				Console.WriteLine($"\tpublic {CSharpTypeOf(p.Value.ValueKind)} {p.Name} {{ get; set; }}");
			}
			Console.WriteLine("}");
		}

		private static void DumpAsCSharpClass(IEnumerable<object?>? a, string className)
		{
			if (a == null) throw new ArgumentNullException(nameof(a));
			if (!a.Any()) throw new ArgumentException("List must not be empty", nameof(a));

			Dictionary<string, HashSet<JsonValueKind>> props = new();
			foreach (object? o in a)
			{
				if (o == null) continue;
				JsonElement? je = o as JsonElement?;
				if (je == null) throw new ArgumentException("Inner object must be a JsonElement", nameof(o));

				foreach (JsonProperty p in je.Value.EnumerateObject())
				{
					if (!props.ContainsKey(p.Name))
					{
						props.Add(p.Name, new());
					}
					props[p.Name].Add(p.Value.ValueKind);
				}
			}

			if (!props.Any()) throw new ArgumentException("Failed to extract any class properties");

			foreach (var p in props)
			{
				if (p.Value.Count == 1) continue;
				if (p.Value.Count < 1) throw new Exception("Property without type should never happen");

				if (p.Value.Count > 1)
				{
					// consolidate
					if (p.Value.Count == 2 && p.Value.Contains(JsonValueKind.True) && p.Value.Contains(JsonValueKind.False))
					{
						p.Value.Remove(JsonValueKind.False);
						continue;
					}

					if (p.Value.Count == 2 && p.Value.Contains(JsonValueKind.Null)
						&& !p.Value.Contains(JsonValueKind.Number)
						&& !p.Value.Contains(JsonValueKind.True)
						&& !p.Value.Contains(JsonValueKind.False))
					{
						p.Value.Remove(JsonValueKind.Null);
						continue;
					}

					if (p.Value.Count == 2 && p.Value.Contains(JsonValueKind.Null) && p.Value.Contains(JsonValueKind.Number))
					{
						p.Value.Clear();
						p.Value.Add(JsonValueKind.Object);
						continue;
					}

					throw new Exception($"Property {p.Key} has several types: {string.Join(", ", p.Value)}");
				}
			}

			Console.WriteLine($"internal class {className}");
			Console.WriteLine("{");
			foreach (var p in props)
			{
				Console.WriteLine($"\tpublic {CSharpTypeOf(p.Value.First())} {p.Key} {{ get; set; }}");
			}
			Console.WriteLine("}");
		}

		private static string CSharpTypeOf(JsonValueKind valueKind)
		{
			switch (valueKind)
			{
				case JsonValueKind.String: return "string?";
				case JsonValueKind.Object: return "object?";
				case JsonValueKind.Array: return "object?[]?";
				case JsonValueKind.Number: return "double";
				case JsonValueKind.True: return "bool";
				case JsonValueKind.False: return "bool";
				case JsonValueKind.Null: return "object?";
				case JsonValueKind.Undefined: return "object?";
			}
			return "object?";
		}

	}
}
