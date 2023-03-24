using System.Diagnostics;
using System.Text.RegularExpressions;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;


namespace NUnitTestRunner
{
	public sealed class RhinoTestListener : ITestListener
	{
		static Stopwatch sw { get; set; } = Stopwatch.StartNew();


		internal static Dictionary<string, TestStatus> PreviousTestResults = new Dictionary<string, TestStatus>();


		public RhinoTestListener()
		{

		}

		public void SendMessage(TestMessage message)
		{
			RhinoApp.WriteLine(message.Message);
		}

		public void TestFinished(ITestResult result)
		{

			if (!result.Test.IsSuite)
			{
				int ms = (int)sw.ElapsedMilliseconds;
				sw.Reset();

				string message = $"### --- Test ({result.Test.Name})";

				switch (result.ResultState.Status)
				{
					case TestStatus.Failed:
						message += $" failed";
						break;
					case TestStatus.Passed:
					case TestStatus.Warning:
						message += $" passed with result {result.ResultState.Status}";
						break;
					case TestStatus.Inconclusive:
					case TestStatus.Skipped:
						message += $" was skipped or inconclusive";
						break;
				}

				message += " in ";

				double testTime = ms; // result.Duration * 1000;
				if (testTime < 1)
				{
					message += "<1ms.";
				}
				else if (testTime < 1000)
				{
					message += "<1s.";
				}
				else
				{
					message += $"{System.Math.Round(result.Duration, 2)}s.";
				}

				RhinoApp.WriteLine(message);
				if (result.ResultState.Status == TestStatus.Failed)
				{
					RhinoApp.WriteLine($"### ### --- REASON : {result.Message}");
				}

				// Ensure no duplicate keys
				string resultName = result.FullName;
				while (PreviousTestResults.ContainsKey(resultName))
				{
					int i = 0;
					if (int.TryParse(result.Name.Split('_').Last(), out i))
					{
						Regex.Replace(resultName, @"_\([\d]+\)", $"_({i}");
					}
					else
					{
						resultName += $"_({i})";
					}
				}

				PreviousTestResults.Add(result.FullName, result.ResultState.Status);
			}
		}

		public void TestOutput(TestOutput output)
		{
			RhinoApp.WriteLine(output.TestName);
		}

		public void TestStarted(ITest test)
		{
			sw.Restart();

			if (test.IsSuite && test.HasChildren)
			{
				TestSuite suite = (TestSuite)test;
				if (suite.Fixture is object) return;

				if (suite.FullName.ToLower().EndsWith(".dll") ||
					suite.FullName.ToLower().EndsWith(".rhp") ||
					suite.FullName.ToLower().EndsWith(".gha"))
				{
					string message = $"Suite ({suite.FullName.Replace("DLL", "dll")} started running with {suite.TestCaseCount} cases readied.";
					RhinoApp.WriteLine(message);
				}
			}

			if (!test.IsSuite)
			{
				string message = $"### --- Starting Test ({test.Name})";
				RhinoApp.WriteLine(message);
			}

		}
	}

}
