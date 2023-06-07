using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Signals.Core")]
[assembly: InternalsVisibleTo("Signals.Core.Web")]
[assembly: InternalsVisibleTo("Signals.Core.Background")]
namespace Signals.Core
{
	internal static class Debugging
	{
		private static object SyncLock { get; set; } = new object();

		private static string File { get; set; } = $"signals-stdout-{DateTime.Now:yyyyMMdd}.trace.log";

		public static bool TracingEnabled = false;

		/// <summary>
		/// Extends all objects to support Debug.WriteLine
		/// due to better verbose experience in Signals
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="message"></param>
		/// <param name="memberName"></param>
		/// <param name="sourceLineNumber"></param>
		internal static void D<T>(
			this T instance,
			string message,
			[CallerMemberName] string memberName = "",
			[CallerLineNumber] int sourceLineNumber = 0)
		{
			// skip tracing if not enabled
			if (!TracingEnabled) return;
			var type = instance == null ? null : typeof(T);
			string formattedMessage;
			if (type == null)
			{
				formattedMessage = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss} [UNKNOWN] [UNKNOWN TYPE] [{memberName}:{sourceLineNumber}] [{message}]";
			}
			else
			{
				formattedMessage = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss} [{type.Assembly?.GetName()?.Name}] [{type.Name}] [{memberName}:{sourceLineNumber}] [{message}]";
			}

			lock (SyncLock)
			{
				using var streamWriter = new StreamWriter(File, true);
				streamWriter.WriteLine(formattedMessage);
				streamWriter.Close();
			}
		}
	}
}