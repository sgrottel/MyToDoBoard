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
			exitCode = 1;
		}

		private static int exitCode = 0;

		static int Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Console.InputEncoding = System.Text.Encoding.UTF8;

			var inputFileArg = new Argument<FileInfo>("Input YamlFile")
			{
				Description = "The input .mytodo file"
			}.AcceptExistingOnly();

			var outputFileOpt = new Option<FileInfo?>("--output")
			{
				Description = "The output file to be written",
				Aliases = { "-o" }
			};

			var forceWriteOpt = new Option<bool>("--force")
			{
				Description = "If set, will overwrite existing output files",
				Aliases = { "-f" }
			};

			var darkModeOpt = new Option<bool?>("--darkmode")
			{
				Description = "When exporting a html report, switches to dark mode",
				Aliases = { "--dark" }
			};

			var noColumnWrapOpt = new Option<bool?>("--nocolumnwrap")
			{
				Description = "When exporting a html report, switches to no-column-wrap mode",
				Aliases = { "--nowrap" }
			};

			var outputFormatTypeOpt = new Option<string>("--type")
			{
				Description = "Specify the output format type",
				DefaultValueFactory = (_) => ReportFormatUtil.ToString(ReportFormat.Html),
				Aliases = { "-t" }
			}.AcceptOnlyFromAmong(ReportFormatUtil.GetStrings());

			var apiEpCommand = new Command("apiep")
			{
				Description = "LocalHtmlInterop API End Point",
				Hidden = true
			};
			{
				var fArg = new Argument<string>("f") { Description = "file path" };
				var tArg = new Argument<string>("t") { Description = "target file path" };
				var apiEpCheckCommand = new Command("check", description: "Open file for editing")
				{
					fArg,
					tArg
				};
				apiEpCheckCommand.SetAction(
					(ParseResult pr) =>
					{
						try
						{
							LocalHtmlInteropHandler.Check(
								pr.GetRequiredValue(fArg),
								pr.GetRequiredValue(tArg)
								);
						}
						catch (Exception ex)
						{
							PrintError($"Unknown exception: {ex}");
						}
					});
				apiEpCommand.Add(apiEpCheckCommand);
				var apiEpUpdateReportCommand = new Command("update", description: "Open file for editing")
				{
					fArg,
					tArg
				};
				apiEpUpdateReportCommand.SetAction(
					(ParseResult pr) =>
					{
						try
						{
							LocalHtmlInteropHandler.UpdateReport(
								pr.GetRequiredValue(fArg),
								pr.GetRequiredValue(tArg)
								);
						}
						catch (Exception ex)
						{
							PrintError($"Unknown exception: {ex}");
						}
					});
				apiEpCommand.Add(apiEpUpdateReportCommand);
				var apiEpEditCommand = new Command("edit", description: "Open file for editing")
				{
					fArg
				};
				apiEpEditCommand.SetAction(
					(ParseResult pr) =>
					{
						try
						{
							LocalHtmlInteropHandler.Edit(
								pr.GetRequiredValue(fArg)
								);
						}
						catch (Exception ex)
						{
							PrintError($"Unknown exception: {ex}");
						}
					});
				apiEpCommand.Add(apiEpEditCommand);
				var apiEpBrowseCommand = new Command("browse", description: "Open file for editing")
				{
					fArg
				};
				apiEpBrowseCommand.SetAction(
					(ParseResult pr) =>
					{
						try
						{
							LocalHtmlInteropHandler.Browse(
								pr.GetRequiredValue(fArg)
								);
						}
						catch (Exception ex)
						{
							PrintError($"Unknown exception: {ex}");
						}
					});
				apiEpCommand.Add(apiEpBrowseCommand);
			}

			var rootCommand = new RootCommand("MyToDoBoard™ Report Application")
			{
				inputFileArg,
				outputFileOpt,
				forceWriteOpt,
				darkModeOpt,
				noColumnWrapOpt,
				outputFormatTypeOpt,
				apiEpCommand
			};
			rootCommand.SetAction(
				(ParseResult pr) =>
				{
					try
					{
						CreateReport(
							pr.GetRequiredValue(inputFileArg),
							pr.GetValue(outputFileOpt),
							pr.GetValue(forceWriteOpt),
							pr.GetValue(darkModeOpt),
							pr.GetValue(noColumnWrapOpt),
							pr.GetRequiredValue(outputFormatTypeOpt)
							);
					}
					catch (Exception ex)
					{
						PrintError($"Error: {ex}");
					}
				});

			CommandLineConfiguration clc = new(rootCommand) { EnablePosixBundling = false };
			exitCode = rootCommand.Parse(args, clc).Invoke();
			return exitCode;
		}

		internal static void CreateReport(FileInfo inputFile, FileInfo? outputFile, bool forceWrite, bool? darkMode, bool? noColumnWrap, string outputFormatType)
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
						Path.GetFullPath(inputFile.FullName).TrimEnd(['\\', '/']),
						Path.GetFullPath(outputFile.FullName).TrimEnd(['\\', '/']),
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

				StaticDataModel.ToDoDocument? todoDoc;
				using (StreamReader input = new(inputFile.FullName))
				{
					var yamlDeserializer = new DeserializerBuilder()
						.WithNamingConvention(CamelCaseNamingConvention.Instance)
						.IgnoreUnmatchedProperties()
						.Build();
					todoDoc = yamlDeserializer.Deserialize<StaticDataModel.ToDoDocument>(input);
				}
				if (todoDoc == null)
				{
					PrintError("Loaded .mytodo seems empty");
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

				if (report is HtmlReport html)
				{
					html.DarkMode = darkMode;
					html.NoColumnWrapMode = noColumnWrap;
				}

				report.Report(todoDoc);

				Console.WriteLine("Done.");
			}
			catch (YamlDotNet.Core.YamlException yex)
			{
				PrintError($"YAML Exception: {yex.Message}\n\t{yex.Start}\n\t{yex.InnerException}");
			}
			catch (Exception ex)
			{
				PrintError($"Unexpected Error: {ex}");
			}
		}
	}
}
