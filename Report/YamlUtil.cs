using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Report
{
	using YamlObject = Dictionary<object, object>;
	using YamlList = List<object>;

	internal static class YamlUtil
	{

		internal static bool IsYamlObject(this object obj)
		{
			return obj is YamlObject;
		}

		internal static YamlObject AsYamlObject(this object obj, string? errorMessage = null)
		{
			try
			{
				return (YamlObject)obj;
			}
			catch
			{
				if (errorMessage != null)
				{
					throw new InvalidCastException(errorMessage);
				}
				else
				{
					throw;
				}
			}
		}

		internal static YamlObject? TryAsYamlObject(this object? obj)
		{
			return obj as YamlObject;
		}

		internal static bool IsYamlList(this object obj)
		{
			return obj is YamlList;
		}

		internal static YamlList AsYamlList(this object obj, string? errorMessage = null)
		{
			try
			{
				return (YamlList)obj;
			}
			catch
			{
				if (errorMessage != null)
				{
					throw new InvalidCastException(errorMessage);
				}
				else
				{
					throw;
				}
			}
		}

		internal static YamlList? TryAsYamlList(this object? obj)
		{
			return obj as YamlList;
		}

		internal static object? GetYamlProperty(this YamlObject obj, string name)
		{
			object? prop;
			if (!obj.TryGetValue(name, out prop))
			{
				throw new KeyNotFoundException($"No property name '{name}' found");
			}
			return prop;
		}

		internal static object NotNull(this object? obj, string errorMessage)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(errorMessage, innerException: null);
			}
			return obj;
		}

	}

}
