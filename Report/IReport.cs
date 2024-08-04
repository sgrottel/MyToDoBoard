namespace MyToDo.Report
{

	internal interface IReport
	{

		string InputPath { get; set; }
		string OutputPath { get; set; }

		void Report(StaticDataModel.ToDoDocument todoDoc);

	}

}
