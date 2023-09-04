using System.CommandLine;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace MyToDo.Report
{
	internal class Program
	{

		static void PrintError(string msg)
		{
			Console.WriteLine();
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine(msg);
			Console.ResetColor();
		}

		static int Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.InputEncoding = System.Text.Encoding.UTF8;

			var inputFileArg = new Argument<FileInfo>("Input YamlFile", description: "The input .mytodo file")
				.ExistingOnly();

			var outputFileOpt = new Option<FileInfo?>("--output", description: "The output file to be written");
			outputFileOpt.AddAlias("-o");

			var forceWriteOpt = new Option<bool>("--force", description: "If set, will overwrite existing output files");
			forceWriteOpt.AddAlias("-f");

			var outputFormatTypeOpt = new Option<string>("--type",
					description: "Specify the output format type",
					getDefaultValue: () => ReportFormatUtil.ToString(ReportFormat.Html))
				.FromAmong(ReportFormatUtil.GetStrings());
			outputFormatTypeOpt.AddAlias("-t");

			var rootCommand = new RootCommand("MyToDoBoard™ Report Application") { inputFileArg, outputFileOpt, forceWriteOpt, outputFormatTypeOpt };
			rootCommand.SetHandler((inputFile, outputFile, forceWrite, outputFormatType) =>
			{
				Console.Write("MyToDoBoard™ Report ... ");

				try
				{
					if (!inputFile.Exists)
					{
						throw new FileNotFoundException();
					}

					ReportFormat format;
					try
					{
						format = ReportFormatUtil.Parse(outputFormatType);
					}
					catch
					{
						PrintError($"Unsupported format {outputFormatType}");
						return;
					}

					// If output file is not specified, use input file as template
					if (outputFile == null)
					{
						outputFile = new(Path.ChangeExtension(inputFile.FullName, ReportFormatUtil.ToFileNameExtension(format)));
						if (string.Compare(
							Path.GetFullPath(inputFile.FullName).TrimEnd(new char[] { '\\', '/' }),
							Path.GetFullPath(outputFile.FullName).TrimEnd(new char[] { '\\', '/' }),
							StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							PrintError("Output file name conflict with input file. Please specify '--output' file.");
							return;
						}

						if (File.Exists(outputFile.FullName))
						{
							if (!forceWrite)
							{
								PrintError($"Output file \"{outputFile.FullName}\" already exists. Please, specify '--force' to overwrite the output file.");
								return;
							}
						}
					}

					object? yaml = null;
					using (StreamReader input = new(inputFile.FullName))
					{
						var yamlDeserializer = new DeserializerBuilder()
							.WithNamingConvention(CamelCaseNamingConvention.Instance)
							.Build();
						yaml = yamlDeserializer.Deserialize(input);
					}

					if (yaml == null)
					{
						PrintError("Loaded .mytodo seems empty");
						return;
					}
					if (!(yaml.IsYamlObject()))
					{
						PrintError("Loaded .mytodo seems illegal. Root should be an object");
						return;
					}

					IReport report;
					switch (format)
					{
						case ReportFormat.Html: report = new HtmlReport(); break;
						case ReportFormat.Text: report = new TextReport(); break;
						default:
							{
								PrintError($"Report logic for type {format} is not implemented. Please, choose another format.");
								return;
							}
					}

					report.InputPath = inputFile.FullName;
					report.OutputPath = outputFile.FullName;

					report.Report(yaml.AsYamlObject());

					Console.WriteLine("Done.");
				}
				catch (Exception ex)
				{
					PrintError($"Unexpected Error: {ex}");
				}
			}, inputFileArg, outputFileOpt, forceWriteOpt, outputFormatTypeOpt);
			return rootCommand.Invoke(args);
		}
	}
}
