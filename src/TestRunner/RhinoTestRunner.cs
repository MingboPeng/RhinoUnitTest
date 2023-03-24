using System.Diagnostics;
using System.Xml;

using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

using NUnitTestRunner.Commands;

namespace NUnitTestRunner
{

	/// <summary>A Test Runner for Rhino based Unit Test projects.</summary>
	public sealed class RhinoTestRunner
	{

		readonly NUnitTestRunnerArgs Args;

		/// <summary>The minimum constructor.</summary>
		/// <param name="_dll">Full dll path</param>
		public RhinoTestRunner(NUnitTestRunnerArgs args)
		{
			Args = args;
		}

		/// <summary>Runs the unit tests inside of the DLL.</summary>
		public bool Run(ITestFilter filter = null)
		{
			OnStarted?.Invoke(this, new NUnitTestRunnerEventArgs(NUnitTestRunnerCommand.Instance.Args));
			try
			{
				var sw = Stopwatch.StartNew();

				if (Args.Dlls == null || Args.Dlls.Length == 0)
				{
					RhinoApp.WriteLine("No Test Assemblies Found!");
					return false;
				}

				string dll = Args.Dlls.FirstOrDefault();
				if (!System.IO.File.Exists(dll))
				{
					RhinoApp.WriteLine($"Path not found for assembly {dll}.");
					return false;
				}

				if (null == filter)
				{
					filter = new RhinoTestFilter();
				}

				string fileName = System.IO.Path.GetFileNameWithoutExtension(dll);

				DefaultTestAssemblyBuilder builder = new DefaultTestAssemblyBuilder();
				IDictionary<string, object> options = new Dictionary<string, object>
				{
					{ NUnit.FrameworkPackageSettings.RunOnMainThread, true },
					{ NUnit.FrameworkPackageSettings.InternalTraceLevel, "Verbose" },
					{ NUnit.FrameworkPackageSettings.NumberOfTestWorkers, 1 }, // Ensures no parallel running
					{ NUnit.FrameworkPackageSettings.DebugTests, Debugger.IsAttached },
					{ NUnit.FrameworkPackageSettings.PauseBeforeRun, false },
					{ NUnit.FrameworkPackageSettings.StopOnError, false }
				};

				builder.Build(dll, options);

				RhinoApp.WriteLine($"Running tests for {dll}");

				RhinoTestListener tListen = new RhinoTestListener();
				NUnitTestAssemblyRunner ntar = new NUnitTestAssemblyRunner(builder);
				ntar.Load(dll, options);

				int count = ntar.CountTestCases(filter);
				RhinoApp.WriteLine($"COUNT : {count}");

				ITestResult result = ntar.Run(tListen, filter);
				TNode node = result.ToXml(true);

				string dllPath = System.IO.Directory.GetParent(dll).FullName;
				string filepath = System.IO.Path.Combine(dllPath, $"{fileName}_integrated_out.xml");

				XmlWriterSettings settings = new XmlWriterSettings()
				{
					Indent = true,
					ConformanceLevel = ConformanceLevel.Auto,
				};

				if (!Args.NoOutputLog)
				{
					using (XmlWriter xml = XmlWriter.Create(filepath, settings))
					{
						node.WriteTo(xml);
					}
				}

				string time = sw.Elapsed.ToString(@"d'd 'hh'h 'mm'm 'ss's 'fff'ms'");
				sw.Stop();

				RhinoApp.WriteLine($"Ran {result.TotalCount} tests in {time}, resulting in :");
				RhinoApp.WriteLine($"--- ✔ {result.PassCount} passed");
				RhinoApp.WriteLine($"--- ❌ {result.FailCount} failed");
				RhinoApp.WriteLine($"--- ➖ {result.SkipCount + result.InconclusiveCount} skipped or inconclusive.");
				RhinoApp.WriteLine($"--- ⚠ {result.WarningCount} warnings!");

				RhinoApp.WriteLine($"Finished Test Suite and wrote file to : {filepath}");
				RhinoApp.WriteLine(result.Output);
			}
			catch (Exception ex)
			{
				RhinoApp.WriteLine("Failed to run test"); ;
				RhinoApp.WriteLine(ex.Message);
			}
			finally
			{
				OnCompleted?.Invoke(this, new NUnitTestRunnerEventArgs(NUnitTestRunnerCommand.Instance.Args));
			}

			return true;
		}

		/// <summary>On Runner Started</summary>
		public EventHandler<NUnitTestRunnerEventArgs> OnStarted { get; set; }

		/// <summary>On Runner Completed. This will still be called on failure</summary>
		public EventHandler<NUnitTestRunnerEventArgs> OnCompleted { get; set; }

	}

}
