using System.IO;

namespace NUnitTestRunner.Utils
{
	public static class Logging
	{

		public static void WriteLog()
		{
			string[] lines = RhinoApp.CapturedCommandWindowStrings(false);
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string nu_log_dir = Path.Combine(appData, "nunittestrunner");
			string nu_log = Path.Combine(nu_log_dir, $"{DateTime.Now:yyyy_M_dd-HHmmss}.log");

			if (!Directory.Exists(nu_log_dir))
			{
				Directory.CreateDirectory(nu_log_dir);
			}

			try
			{
				File.WriteAllLines(nu_log, lines, System.Text.Encoding.ASCII);
			}
			catch (Exception ex)
			{
				RhinoApp.WriteLine(ex.Message);
			}

			RhinoApp.CommandWindowCaptureEnabled = false;

			RhinoApp.WriteLine($"Log written to {nu_log}");
		}

	}
}
