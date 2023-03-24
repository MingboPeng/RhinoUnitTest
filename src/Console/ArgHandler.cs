namespace NUnitTestRunner.Console
{

	/// <summary>Handles Args</summary>
	public static class ArgHandler
	{
		public const char Delimiter = '+';
		public const string PreCommand = "--";
		public const char DllDelimiter = ';';

		/// <summary>For toggled Args, e.g --ExitOnFinish</summary>
		private static readonly IReadOnlyDictionary<string, Action<NUnitTestRunnerArgs>> s_boolArgSwitch =
			new Dictionary<string, Action<NUnitTestRunnerArgs>>
		{
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.NoOutputLog).ToLower()}", (testArgs) => testArgs.NoOutputLog = true },
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.StopOnFail).ToLower()}", (testArgs) => testArgs.StopOnFail = true },
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Quiet).ToLower()}", (testArgs) => testArgs.Quiet = true },
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Help).ToLower()}", (testArgs) => testArgs.Help = true },
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Debug).ToLower()}", (testArgs) => testArgs.Debug = true },
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.ExitOnFinish).ToLower()}", (testArgs) => testArgs.ExitOnFinish = true },
		};

		/// <summary>For paired Args, e.g --Format NUnit</summary>
		private static readonly IReadOnlyDictionary<string, Action<NUnitTestRunnerArgs, string>> s_argArgSwitch =
			new Dictionary<string, Action<NUnitTestRunnerArgs, string>>
		{
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Filter).ToLower()}", Filter },
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Format).ToLower()}", Format },
			{ $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.ClassAndMethod).ToLower()}", RunMethod }
		};

		/// <summary>Parses the given string and turns it into Test Args</summary>
		public static NUnitTestRunnerArgs Parse(string input)
		{
			string[] arguments = input?.Split(new string[] { $"{Delimiter}" },
											StringSplitOptions.RemoveEmptyEntries);

			if (null == arguments || arguments.Length <= 0)
			{
				throw new ArgumentException("Invalid inputs");
			}

			NUnitTestRunnerArgs testArgs = new NUnitTestRunnerArgs();

			for (int i = 0; i < arguments.Length; i++)
			{
				int index = i;

				string lowerArg = arguments[i].ToLower();

				if (s_boolArgSwitch.TryGetValue(lowerArg, out Action<NUnitTestRunnerArgs> boolArgAction))
				{
					boolArgAction.Invoke(testArgs);
				}
				else if (s_argArgSwitch.TryGetValue(lowerArg, out Action<NUnitTestRunnerArgs, string> argArgAction))
				{
					int argIndex = index + 1;
					if (argIndex > arguments.Length)
					{
						throw new ArgumentException($"Missing Argument for {lowerArg}");
					}

					string argValue = arguments[argIndex];
					argArgAction(testArgs, argValue);
					i += 1;
				}
				else if (lowerArg.EndsWith(".dll") ||
					lowerArg.EndsWith(".rhp") ||
					lowerArg.EndsWith(".gha"))
				{
					testArgs.Dlls = lowerArg.Split(';');
				}
				else
				{
					throw new ArgumentException($"Invalid Argument {lowerArg}");
				}

			}

			return testArgs;

		}

		private static void Filter(NUnitTestRunnerArgs testArgs, string argument)
		{
			testArgs.Filter = argument;
		}

		private static void Format(NUnitTestRunnerArgs testArgs, string argument)
		{
			if (Enum.TryParse(argument, true, out OutputFormat format))
			{
				testArgs.Format = format;
			}
			else
			{
				throw new Exception("Output format was not in the correct format.");
			}
		}

		private static void RunMethod(NUnitTestRunnerArgs testArgs, string argument)
		{
			testArgs.ClassAndMethod = argument;
		}

	}

}
