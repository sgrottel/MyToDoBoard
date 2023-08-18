using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Report
{
	using YamlObject = Dictionary<object, object>;
	using YamlList = List<object>;

	internal class HtmlReport : IReport
	{
		public string InputPath { get; set; } = string.Empty;
		public string OutputPath { get; set; } = string.Empty;

		public void Report(YamlObject myToDoYaml)
		{
			throw new NotImplementedException();
		}
	}
}
