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
				JsonDocument doc = JsonDocument.Parse(json);
				JsonElement root = doc.RootElement;

				Console.WriteLine($"Root -- {root.ValueKind}");
				foreach (JsonProperty p in root.EnumerateObject())
				{
					Console.Write($"  {p.Name} -- {p.Value.ValueKind}");
					if (p.Value.ValueKind == JsonValueKind.String)
					{
						Console.Write($" -- {p.Value.GetString()}");
					}
					Console.WriteLine();
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex}");
				return 2;
			}

			return 0;
		}
	}
}
