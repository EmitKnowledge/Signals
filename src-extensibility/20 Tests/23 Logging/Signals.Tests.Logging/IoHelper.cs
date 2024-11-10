using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Tests.Logging
{
	internal class IoHelper
	{
		internal static string ReadAllText(string file)
		{
			using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using var textReader = new StreamReader(fileStream);
			return textReader.ReadToEnd();
		}
	}
}
