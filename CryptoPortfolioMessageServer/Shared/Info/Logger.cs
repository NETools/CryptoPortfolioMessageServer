using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoPortfolioMessageServer.Shared.Data;

namespace CryptoPortfolioMessageServer.Shared.Info
{
	public class Logger
	{
		private static Logger _Logger;
		private List<Action<string, LoggerState>> _loggerSources = [];
		private Logger()
		{
			AddSource((message, state) =>
			{
				switch (state)
				{
					case LoggerState.Normal:
						Console.ForegroundColor = ConsoleColor.Gray;
						break;
					case LoggerState.Okay:
						Console.ForegroundColor = ConsoleColor.Green;
						break;
					case LoggerState.Error:
						Console.ForegroundColor = ConsoleColor.Red;
						break;
					case LoggerState.Warning:
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;
				}

				Console.Write(message);

				Console.ForegroundColor = ConsoleColor.Gray;
			});
		}

		public static Logger Default()
		{
			if (_Logger == null)
				_Logger = new Logger();
			return _Logger;
		}

		public void AddSource(Action<string, LoggerState> source)
		{
			_loggerSources.Add(source);
		}

		public void WriteLine(string message, LoggerState state)
		{
			_loggerSources.ForEach(src => src?.Invoke(message + Environment.NewLine, state));
		}

		public void Write(string message, LoggerState state)
		{
			_loggerSources.ForEach(src => src?.Invoke(message, state));
		}
	}
}
