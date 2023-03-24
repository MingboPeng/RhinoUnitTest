using Eto.Forms;

using NUnitTestRunner.Console;
using NUnitTestRunner.Utils;

using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;

namespace NUnitTestRunner.Commands
{

	public sealed class NUnitTestRunnerCommand : Rhino.Commands.Command
	{

		static bool IsReady { get; set; }

		internal NUnitTestRunnerArgs Args { get; private set; }

		public static NUnitTestRunnerCommand Instance { get; private set; }

		public override string EnglishName => "NUnitTestRunner";

		public NUnitTestRunnerCommand()
		{
			Instance = this;
			RhinoApp.Initialized += RhinoApp_Initialized;
		}

		protected override Result RunCommand(RhinoDoc doc, RunMode mode)
		{
			RhinoApp.CommandWindowCaptureEnabled = true;
			Args = null;

			if (mode == RunMode.Scripted)
			{
				GetString gs = new GetString();
				gs.AcceptEnterWhenDone(true);
				gs.AcceptString(true);
				gs.AcceptNothing(true);
				gs.SetCommandPrompt("Input arguments");

				bool cond = true;
				while (cond)
				{
					switch (gs.Get())
					{
						case GetResult.String:
							string _input = gs.StringResult().CorrectPath();

							Args = ArgHandler.Parse(_input);
							Args.Doc = doc;

							cond = false;
							break;
						case GetResult.Nothing:
							break;
					}
				}
			}
			else
			{
				var selFolderDialog = new Eto.Forms.OpenFileDialog()
				{
					CheckFileExists = true,
					MultiSelect = true,
					Title = "Choose a Test DLL",
				};
				selFolderDialog.Filters.Add(new FileFilter("DLL", FileUtils.AcceptedExtensions.Select(ext => $"*{ext}").ToArray()));
				var result = selFolderDialog.ShowDialog(RhinoEtoApp.MainWindow);

				if (result == DialogResult.Ok)
				{
					string _input = selFolderDialog.Filenames?.FirstOrDefault()?.CorrectPath();

					Args = ArgHandler.Parse(_input);
					Args.Doc = doc;
				}

			}

			// Tests etc are only run AFTER idle has been hit to ensure Rhino has initialized.
			RhinoApp.Idle += RhinoApp_Idle;

			return Result.Success;
		}

		// Ensure that Rhino is ready before we start running tests!
		private void RhinoApp_Initialized(object sender, EventArgs e)
		{
			RhinoApp.Initialized -= RhinoApp_Initialized;
			IsReady = true;
		}

		private void RhinoApp_Idle(object sender, EventArgs e)
		{
			RhinoApp.Idle -= RhinoApp_Idle;

			if (IsReady && Args is object)
			{
				Result result = Matrix.RunCommandMatrix(Args);

				Logging.WriteLog();

				if (Args.ExitOnFinish)
				{
					Environment.FailFast("Finished!");
				}

			}
		}

	}

}
