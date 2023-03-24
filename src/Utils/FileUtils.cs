namespace NUnitTestRunner.Utils
{

	internal static class FileUtils
	{

		internal static string[] AcceptedExtensions = new string[]
		{
			".dll",
			".gha",
			".rhp"
		};

		internal static string CorrectPath(this string path)
			=> path.Replace("\\\\", "\\").Replace("/", "\\");

	}

}
