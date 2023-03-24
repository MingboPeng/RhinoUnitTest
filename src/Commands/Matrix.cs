using System.IO;
using System.Reflection;

using NUnitTestRunner.Console;
using NUnitTestRunner.Utils;

using Rhino.Commands;
using Rhino.UI;

namespace NUnitTestRunner.Commands
{

	internal static class Matrix
	{

		internal static Result RunCommandMatrix(NUnitTestRunnerArgs args)
		{
			if (null == args)
				return Result.Failure;

			if (args.Help)
			{
				return RunHelp(args); //, new string[] { input });
			}
			else if (!string.IsNullOrEmpty(args.ClassAndMethod))
			{
				RhinoApp.WriteLine("Found compatible Method and Type");
				return RunAssemblyMethod(args);
			}
			else if (args.Mode == RunMode.Scripted)
			{
				return RunTestAssembly(args);
			}
			else
			{
				Assembly testAssembly = Assembly.LoadFrom(args.Dlls.FirstOrDefault());
				var dialog = new UI.TestRunnerDialog(testAssembly, args);
				dialog.RestorePosition();
				dialog.ShowSemiModal(args.Doc, RhinoEtoApp.MainWindow);
				dialog.SavePosition();
			}

			return Result.Failure;
		}

		#region Help Command

		internal static Result RunHelp(NUnitTestRunnerArgs _)
		{
			var assem = typeof(NUnitTestRunnerPlugIn).Assembly;
			Version version = assem.GetName().Version;
			string bits = Environment.Is64BitProcess ? "64" : "32";

			RhinoApp.WriteLine(@$"
NUnitTestRunner v{version} ({bits}-bit Desktop .NET 4.8, runtime: {Environment.Version})
Copyright (C) StructureCraft.

usage: [-]NUNitTestRunner <command> [args] [options]

Examples:
    NUNitTestRunner
    -NUNitTestRunner (Will Run Headlessly)
    NUNitTestRunner {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Help)}
    NUNitTestRunner ""pathtomytests\testassembly.dll""
    NUNitTestRunner ""pathtomytests\testassembly.dll{ArgHandler.Delimiter}{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.NoOutputLog)}{ArgHandler.Delimiter}{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Quiet)}""
    NUNitTestRunner ""pathtomytests\testassembly.dll{ArgHandler.Delimiter}<MethodAndClassName>""

The available NUNitTestRunner commands are:
    {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Help)}           Runs this help menu
    <dll>            Runs the NUnit tests insde of the given test assembly
                         Entering nothing runs the last given test assembly    

Valid args:
    <dll>{ArgHandler.Delimiter}<MethodAndClassName>   Runs the given method from the dll, instead of tests

Valid options:
  {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.NoOutputLog)}        Produces no output file (default : {new NUnitTestRunnerArgs().NoOutputLog})
  {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Quiet)}                      Writes no lines to the console (within the runners control) (default : {new NUnitTestRunnerArgs().Quiet})
  {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.StopOnFail)}            Stops running tests when a single failure is hit (default : {new NUnitTestRunnerArgs().StopOnFail})
  {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Debug)}                    Writes more info to the console (default : {new NUnitTestRunnerArgs().Debug})
  {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Filter)}                      Uses the given filter when looking for tests (no default) (accepted : words, wildcards etc. e.g. *compatibility*)
  {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Format)}                  The output format of the output file (default : {new NUnitTestRunnerArgs().Format}) (accepted {string.Join(", ", Enum.GetNames(typeof(OutputFormat)))})
  {ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.ExitOnFinish)}         Exits Rhino once tests are complete (default : {new NUnitTestRunnerArgs().ExitOnFinish})
");

			return Result.Success;
		}

		#endregion

		#region Run Test Assembly

		internal static Result RunTestAssembly(NUnitTestRunnerArgs args)
		{
			if (args.Dlls == null || args.Dlls.Length <= 0)
			{
				RhinoApp.WriteLine("RunTestAssembly requires a valid DLL.");
				return Result.Failure;
			}

			RhinoApp.WriteLine(string.Join(", ", args.Dlls));
			var runner = new RhinoTestRunner(args);
			runner.Run();

			return Result.Success;
		}

		#endregion

		#region Run Assembly Method

		/// <summary>Runs the given assembly method</summary>
		internal static Result RunAssemblyMethod(NUnitTestRunnerArgs args)
		{
			if (args.Dlls == null || args.Dlls.Length <= 0)
			{
				RhinoApp.WriteLine("RunAssembly requires a <dll> and a <fully qualified method>");
				return Result.Failure;
			}
			if (string.IsNullOrEmpty(args.ClassAndMethod))
			{
				RhinoApp.WriteLine("RunAssembly requires a <dll> and a <fully qualified method>. Extra inputs ignored.");
				return Result.Failure;
			}

			string dll = args.Dlls.FirstOrDefault().CorrectPath();
			string typeAndMethod = args.ClassAndMethod;
			List<string> namespaced = typeAndMethod.Split('.').ToList();

			string methodName = namespaced.Last();
			namespaced.RemoveAt(namespaced.Count - 1);
			string typeName = string.Join(".", namespaced);

			if (!File.Exists(dll))
			{
				RhinoApp.WriteLine($"Could not find assembly at path {dll}");
				return Result.Failure;
			}

			Assembly assem = Assembly.LoadFrom(dll);
			Type type = assem.GetType(typeName);

			if (type == null)
			{
				RhinoApp.WriteLine($"Could not find type {typeName} in assembly {assem.GetName().Name}");
				return Result.Failure;
			}

			MethodInfo method = type.GetMethod(methodName);
			if (method == null)
			{
				RhinoApp.WriteLine($"Could not find method {methodName} in assembly {assem.GetName().Name}, is it static?");
				return Result.Failure;
			}

			object @return;

			try
			{
				@return = method.Invoke(null, null);
			}
			catch (Exception ex)
			{
				RhinoApp.WriteLine($"method {methodName} produced an error with result : {ex.InnerException}");
				return Result.Failure;
			}

			if (@return is bool returnBool)
			{
				if (returnBool)
				{
					RhinoApp.WriteLine($"Method {methodName} was run successfully!");
					return Result.Success;
				}

				RhinoApp.WriteLine($"Method {methodName} failed!");
			}

			return Result.Failure;
		}

		#endregion

	}

}
