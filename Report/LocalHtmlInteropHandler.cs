using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MyToDo.Report
{
	internal static class LocalHtmlInteropHandler
	{
		public static void Check(string f, string t)
		{
			if (!File.Exists(f)) throw new FileNotFoundException(f);
			if (!File.Exists(t)) throw new FileNotFoundException(t);

			var inputFileHash = SHA256.Create();
			inputFileHash.ComputeHash(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read));
			var inputFileHashStr = Convert.ToBase64String(inputFileHash.Hash ?? Array.Empty<byte>());

			var targetFileContent = File.ReadAllText(t);
			string pattern = @$"<!--\s*INPUTFILEHASH:\s*{Regex.Escape(inputFileHashStr)}\s*-->";

			var match = Regex.Match(targetFileContent, pattern);

			Console.WriteLine(match.Success ? "1" : "0");
		}

		public static void UpdateReport(string f, string t)
		{
			if (!File.Exists(f)) throw new FileNotFoundException(f);
			if (!File.Exists(t)) throw new FileNotFoundException(t);

			Program.CreateReport(new(f), new(t), true, null, null, "html");
		}
		public static void Edit(string f)
		{
			if (!File.Exists(f)) throw new FileNotFoundException(f);

			ProcessStartInfo psi = new();
			psi.FileName = f;
			psi.UseShellExecute = true;
			Process.Start(psi);
		}

		public static void Browse(string f)
		{
			if (!File.Exists(f)) throw new FileNotFoundException(f);
			ShowSelectedInExplorer.FileOrFolder(f);
		}
	}
}
