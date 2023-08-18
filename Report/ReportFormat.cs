using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Report
{
	internal enum ReportFormat
	{
		Html,
		Text
	}

	internal static class ReportFormatUtil
	{

		internal static string[] GetStrings()
		{
			return Array.ConvertAll(Enum.GetValues<ReportFormat>(), ToString);
		}

		internal static string ToString(ReportFormat format)
		{
			switch (format)
			{
				case ReportFormat.Html: return "html";
				case ReportFormat.Text: return "txt";
			}
			return "";
		}

		internal static string ToFileNameExtension(ReportFormat format)
		{
			return ToString(format);
		}

		internal static ReportFormat Parse(string str)
		{
			if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException();
			if (str.Equals("html", StringComparison.CurrentCultureIgnoreCase)) return ReportFormat.Html;
			if (str.Equals("txt", StringComparison.CurrentCultureIgnoreCase)) return ReportFormat.Text;
			if (str.Equals("text", StringComparison.CurrentCultureIgnoreCase)) return ReportFormat.Text;
			throw new ArgumentOutOfRangeException();
		}

	}

}
