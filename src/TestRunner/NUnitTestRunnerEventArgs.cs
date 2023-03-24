/// <summary>Event Args for NUnitTestRunner Events</summary>
public sealed class NUnitTestRunnerEventArgs : EventArgs
{

	public readonly NUnitTestRunnerArgs Args;

	public NUnitTestRunnerEventArgs(NUnitTestRunnerArgs args)
	{
		this.Args = args;
	}

}
