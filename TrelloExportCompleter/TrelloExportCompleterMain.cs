using System.Text.Json;

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

			var json = File.ReadAllText(jsonFile);
			JsonDocument doc = JsonDocument.Parse(json);

		}
	}
}
