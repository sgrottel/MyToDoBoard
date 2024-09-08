using MyToDo.StaticDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Importer
{

	internal class YamlWriter
	{

		public static void Write(ToDoDocument todoDoc, string outFile)
		{
			Console.WriteLine($"Writing YAML {outFile}");
			using var writer = new StreamWriter(path: outFile, append: false, encoding: new UTF8Encoding(false));
			var yaml = new SerializerBuilder()
				.WithTypeConverter(new DateTimeTypeConverter())
				.WithTypeConverter(new StringListAsJsonTypeConverter())
				.Build();
			yaml.Serialize(writer, todoDoc);
		}

		private class DateTimeTypeConverter : IYamlTypeConverter
		{
			public bool Accepts(Type type) => type == typeof(DateTime);

			public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
			{
				throw new NotImplementedException();
			}

			public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
			{
				DateTime? d = value as DateTime?;
				if (d == null)
				{
					emitter.Emit(new YamlDotNet.Core.Events.Scalar(""));
				}
				else
				{
					emitter.Emit(new YamlDotNet.Core.Events.Scalar(d.Value.ToString("yyyy-MM-dd HH:mm:ss")));
				}
			}
		}

		private class StringListAsJsonTypeConverter : IYamlTypeConverter
		{
			public bool Accepts(Type type) => type == typeof(MyToDo.StaticDataModel.Utility.StringListAsJson);

			public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
			{
				throw new NotImplementedException();
			}

			public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
			{
				List<string>? l = value as List<string>;
				if (l == null) return;

				emitter.Emit(new YamlDotNet.Core.Events.SequenceStart(null, null, false, YamlDotNet.Core.Events.SequenceStyle.Flow));
				foreach (string s in l)
				{
					emitter.Emit(new YamlDotNet.Core.Events.Scalar(s));
				}
				emitter.Emit(new YamlDotNet.Core.Events.SequenceEnd());
			}
		}

	}
}
