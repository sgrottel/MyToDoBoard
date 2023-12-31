using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TrelloExportCompleter
{

	/// <summary>
	/// https://community.atlassian.com/t5/Trello-questions/Trello-Exported-JSON-data-is-missing-data-before-September-2018/qaq-p/1466232
	/// Hey Jason,
	///
	/// Behavior like this is currently expected with exports.
	/// Trello has a 1,000 action limit for activity included in an export along with the activity in your board menu.
	/// So with older boards, you won't be able to see past a certain point.
	///
	/// The older activity still exists, but to find that you'll need to make a request via our API.
	///
	/// I answered a similar question to this previously which covers how you can get that info:
	/// https://community.atlassian.com/t5/Trello-questions/My-Board-Activity-does-not-go-past-a-certain-date/qaq-p/1362031
	///
	/// Hopefully that will help you get that older activity!
	/// </summary>
	internal class Program
	{
		internal class CardAction
		{
			public string? id { get; set; }
			public string? idMemberCreator { get; set; }
			public object? data { get; set; }
			public object? appCreator { get; set; }

			[JsonPropertyName("type")]
			public string? ActionType { get; set; }
			public string? date { get; set; }
			public object? limits { get; set; }
			public object? memberCreator { get; set; }

			[JsonExtensionData]
			public Dictionary<string, JsonElement>? ExtensionData { get; set; }
		}

		/// <summary>
		/// Using the deserialized object as template to try to keep the members in the correct order for serialization after editing
		/// </summary>
		internal class Board
		{
			public string? id { get; set; }
			public string? nodeId { get; set; }
			public string? name { get; set; }
			public string? desc { get; set; }
			public object? descData { get; set; }
			public bool closed { get; set; }
			public string? dateClosed { get; set; }
			public string? idOrganization { get; set; }
			public object? idEnterprise { get; set; }
			public object? limits { get; set; }
			public bool pinned { get; set; }
			public bool starred { get; set; }
			public string? url { get; set; }
			public object? prefs { get; set; }
			public string? shortLink { get; set; }
			public bool subscribed { get; set; }
			public object? labelNames { get; set; }
			public object?[]? powerUps { get; set; }
			public string? dateLastActivity { get; set; }
			public string? dateLastView { get; set; }
			public string? shortUrl { get; set; }
			public object?[]? idTags { get; set; }
			public string? datePluginDisable { get; set; }
			public object? creationMethod { get; set; }
			public string? ixUpdate { get; set; }
			public object? templateGallery { get; set; }
			public bool enterpriseOwned { get; set; }
			public object? idBoardSource { get; set; }
			public object?[]? premiumFeatures { get; set; }
			public string? idMemberCreator { get; set; }
			public CardAction[]? actions { get; set; }
			public object?[]? cards { get; set; }
			public object?[]? labels { get; set; }
			public object?[]? lists { get; set; }
			public object?[]? members { get; set; }
			public object?[]? checklists { get; set; }
			public object?[]? customFields { get; set; }
			public object?[]? memberships { get; set; }
			public object?[]? pluginData { get; set; }

			[JsonExtensionData]
			public Dictionary<string, JsonElement>? ExtensionData { get; set; }
		};

		static void Main(string[] args)
		{
			if (args.Length != 3)
			{
				Console.WriteLine("Syntax:");
				Console.WriteLine("TrelloExportCompleter.exe <exported.json> <apikey> <apitoken>");
				Console.WriteLine();
				return;
			}

			string jsonFile = args[0];
			string apiKey = args[1];
			string apiToken = args[2];

			Console.WriteLine($"Loading {jsonFile}");
			var json = File.ReadAllText(jsonFile);
			Board board = JsonSerializer.Deserialize<Board>(json) ?? throw new JsonException();

			Console.WriteLine($"Board Id: {board.id}");

			List<CardAction> actions = new();
			if (board.actions != null) actions.AddRange(board.actions);

			if (actions.Count == 1000)
			{
				Console.WriteLine("You came to the right place.");
			}

			actions.RemoveAt(0);

			/*
			Paginated download of actions from a board:
			Set <boardId>, <ApiKey>, <ApiToken>
			Optional:
				<Limit> number of entries in answer 0-1000
				<before> only include actions stored before (older) the action with the specified id
			Since actions are sorted by date, newest to oldest, this way you get pagination.

PS C:\Downloads> curl.exe --request GET --URL 'https://api.trello.com/1/boards/###/actions?key=###&token=###&limit=###'
[...{"id":"0815",...}]
PS C:\Downloads> curl.exe --request GET --URL 'https://api.trello.com/1/boards/###/actions?key=###&token=###&limit=###&before=0815'
[{...}]
			*/


			// TODO


			board.actions = actions.ToArray();

			WriteJsonSlowlyNeedingMuchMemoryButAsItShouldBe(board,
				Path.Combine(
					Path.GetDirectoryName(jsonFile) ?? string.Empty,
					Path.GetFileNameWithoutExtension(jsonFile)
					+ "_out" + Path.GetExtension(jsonFile))
				);
		}

		internal static void WriteJsonSlowlyNeedingMuchMemoryButAsItShouldBe(Board root, string filename)
		{
			Regex largeUnicode = new Regex(@"\\u[0-9a-f]{4}\\u[0-9a-f]{4}", RegexOptions.IgnoreCase);
			var utf8 = new UTF8Encoding(false);
			using (FileStream outFile = File.Create(filename))
			using (TextWriter writer = new StreamWriter(outFile, utf8))
			using (MemoryStream mem = new MemoryStream())
			{
				JsonWriterOptions opt = new()
				{
					Indented = true,
					Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				};
				using (Utf8JsonWriter jsonWriter = new Utf8JsonWriter(mem, opt))
					JsonSerializer.SerializeToDocument(root).WriteTo(jsonWriter);
				mem.Position = 0;
				using (StreamReader reader = new StreamReader(mem, utf8))
				{
					while (reader.Peek() >= 0)
					{
						var line = reader.ReadLine();
						if (line == null) break;

						if (largeUnicode.IsMatch(line))
						{
							line = largeUnicode.Replace(line, (m) => Regex.Unescape(m.Value));
						}

						if (line.StartsWith(' '))
						{
							int oldLen = line.Length;
							line = line.TrimStart();
							line = new string('\t', (oldLen - line.Length) / 2) + line;
						}

						if (reader.Peek() >= 0) writer.WriteLine(line);
						else writer.Write(line);
					}
				}
			}
		}

	}
}
