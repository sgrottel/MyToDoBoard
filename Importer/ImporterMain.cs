using Importer.Trello;
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

				//DumpAsCSharpClass(board.labelNames, "LabelNames");
				//DumpAsCSharpClass(board.labels, "Labels");

				//JsonDocument doc = JsonDocument.Parse(json);
				//JsonElement root = doc.RootElement;

				//foreach (JsonProperty p in root.EnumerateObject())
				//{
				//	Console.WriteLine($"\t\tpublic {CSharpTypeOf(p.Value.ValueKind)} {p.Name} {{ get; set; }}");
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

		private static void DumpAsCSharpClass(object?[]? a, string className)
		{
			if (a == null) throw new ArgumentNullException(nameof(a));
			if (a.Length == 0) throw new ArgumentException("List must not be empty", nameof(a));

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
					throw new Exception($"Property {p.Key} has several types: {string.Join(", ", p.Value)}");
					// Could try to consolidate
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
