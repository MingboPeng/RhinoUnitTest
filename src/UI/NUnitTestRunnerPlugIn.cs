/// <summary>Runs tests</summary>
public class NUnitTestRunnerPlugIn : Rhino.PlugIns.PlugIn
{

	/// <summary>Required for testing</summary>
	public override Rhino.PlugIns.PlugInLoadTime LoadTime => Rhino.PlugIns.PlugInLoadTime.AtStartup;


	/// <summary>Rhino Constructor</summary>
	public NUnitTestRunnerPlugIn()
	{
		Instance = this;
	}

	///<summary>Gets the only instance of the NUnitTestRunnerPlugIn plug-in.</summary>
	public static NUnitTestRunnerPlugIn Instance { get; private set; }

}
