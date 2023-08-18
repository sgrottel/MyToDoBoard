using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Report
{
	using YamlObject = Dictionary<object, object>;

	internal interface IReport
	{

		string InputPath { get; set; }
		string OutputPath { get; set; }

		void Report(YamlObject myToDoYaml);

	}

}
