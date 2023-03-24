using NUnit.Framework.Interfaces;

namespace NUnitTestRunner
{
	public sealed class RhinoTestFilter : ITestFilter
	{

		public TNode AddToXml(TNode parentNode, bool recursive)
		{
			RhinoApp.WriteLine("Oh Darn");
			throw new System.NotImplementedException();
		}

		public bool IsExplicitMatch(ITest test) => true;

		public bool Pass(ITest test)
		{
			if (RhinoTestListener.PreviousTestResults.TryGetValue(test.FullName, out TestStatus status))
			{
				return status == TestStatus.Failed;
			}

			return true;
		}

		public TNode ToXml(bool recursive)
		{
			RhinoApp.WriteLine("Oh Darn 2");

			throw new System.NotImplementedException();
		}

	}

}
