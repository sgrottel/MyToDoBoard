using Importer.DataModel;
using Importer.Trello;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

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

				ToDoDocument todoDoc = new();
				if (string.IsNullOrEmpty(todoDoc.Comment))
				{
					todoDoc.Comment = $"Imported from Trello {Path.GetFileName(inFile)}\nBoard: {board.name}\nState: {board.dateLastActivity}";
				}

				todoDoc.Import(board);

				YamlWriter.Write(todoDoc, outFile);

				Console.WriteLine("Done.");
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
