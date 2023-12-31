using Importer.Trello;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Importer
{

	/// <summary>
	/// Experimental importer application
	/// </summary>
	internal class Program
	{
		static int Main(string[] args)
		{
			Console.OutputEncoding = UTF8Encoding.UTF8;
			Console.WriteLine("MyToDoList™ Importer");
			Console.WriteLine(" [ Experimental ]");
			Console.WriteLine();

			if (args.Length != 3)
			{
				Console.WriteLine("You need to specify command, input file, and output file");
				return 1;
			}

			try
			{
				string cmd = args[0];
				string inFile = args[1];
				string outFile = args[2];

				if (!string.Equals(cmd, "trello", StringComparison.CurrentCultureIgnoreCase))
				{
					throw new InvalidOperationException("Only importing from Trello-Json is currently available");
				}
				if (!File.Exists(inFile))
				{
					throw new FileNotFoundException(inFile);
				}
				if (!Directory.Exists(Path.GetDirectoryName(outFile)))
				{
					throw new FileNotFoundException("Output directory must exist");
				}

				var json = File.ReadAllText(inFile);
				Board? board = JsonSerializer.Deserialize<Board>(json);
				if (board == null) throw new InvalidDataException("Expected json of 'Board");

				Console.WriteLine($"Importing board: Trello {board.name}");
				Console.WriteLine($"  state from: {board.dateLastActivity}");
				Console.WriteLine();

				if ((board.actions?.Length ?? 0) == 1000)
				{
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.BackgroundColor = ConsoleColor.Black;
					Console.WriteLine("WARNING: export json contains exactly 1000 actions.");
					Console.WriteLine("This is very likely the API limit, and actions are likely missing from the export.");
					Console.WriteLine("Most importantly, this means that comments on cards might be missing.");
					Console.ResetColor();
					Console.WriteLine();
					Console.WriteLine("It is recommended to generate an API key and API token, and then run the `TrelloExportCompleter` tool on the exported json file");
					Console.WriteLine("https://trello.com/power-ups/admin");
					Console.WriteLine();

					/*
					 * Suspicious. Likely the export limit was hit

					https://community.atlassian.com/t5/Trello-questions/Trello-Exported-JSON-data-is-missing-data-before-September-2018/qaq-p/1466232
					Hey Jason,

					Behavior like this is currently expected with exports.
					Trello has a 1,000 action limit for activity included in an export along with the activity in your board menu.
					So with older boards, you won't be able to see past a certain point.

					The older activity still exists, but to find that you'll need to make a request via our API.
					I answered a similar question to this previously which covers how you can get that info:

					https://community.atlassian.com/t5/Trello-questions/My-Board-Activity-does-not-go-past-a-certain-date/qaq-p/1362031

					Hopefully that will help you get that older activity!

					 */
				}

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
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex}");
				return 2;
			}

			return 0;
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
