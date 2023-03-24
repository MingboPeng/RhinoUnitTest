/// <summary>The TestRunner Args</summary>
public sealed class NUnitTestRunnerArgs
{

	///<summary>Produces no output file</summary>
	public bool NoOutputLog { get; set; } = false;

	///<summary>Writes no lines to the console (within the runners control)</summary>
	public bool Quiet { get; set; } = false;

	///<summary>Stops running tests when a single failure is hit</summary>
	public bool StopOnFail { get; set; } = false;

	///<summary>Writes more info to the console</summary>
	public bool Debug { get; set; } = false;

	///<summary>Uses the given filter when looking for tests</summary>
	public string Filter { get; set; } = null;

	///<summary>The output format of the output file</summary>
	public OutputFormat Format { get; set; } = OutputFormat.NUnit;

	///<summary>Exits Rhino once tests are complete</summary>
	public bool ExitOnFinish { get; set; } = false;

	///<summary>Captures the Rhino RunMode of the Command</summary>
	public Rhino.Commands.RunMode Mode { get; set; } = Rhino.Commands.RunMode.Interactive;

	///<summary>All of the dlls to test</summary>
	public string[] Dlls { get; set; } = Array.Empty<string>();

	///<summary>The RhinoDoc that called for this test</summary>
	public RhinoDoc Doc { get; set; }

	///<summary>The Method to Run</summary>
	public string ClassAndMethod { get; set; } = string.Empty;

	/// <summary>Ask for Help?</summary>
	public bool Help { get; set; } = false;

}

