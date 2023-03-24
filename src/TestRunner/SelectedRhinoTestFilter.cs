using NUnit.Framework.Interfaces;

namespace NUnitTestRunner
{

	/// <summary>Only runs the given rhino tests</summary>
	public sealed class SelectedRhinoTestFilter : ITestFilter
	{
		private readonly string _methodName;
		private readonly string _className;

		public SelectedRhinoTestFilter(string methodName, string className)
		{
			_methodName = methodName;
			_className = className;
		}

		public bool IsExplicitMatch(ITest test) => true;

		public bool Pass(ITest test)
		{
			bool classCondition = test.ClassName.EndsWith(_className);
			bool methodCondition = test.MethodName == _methodName;

			if (!string.IsNullOrEmpty(_methodName))
			{
				return classCondition && methodCondition;
			}

			return classCondition;
		}

		public TNode AddToXml(TNode parentNode, bool recursive)
		{
			throw new System.NotImplementedException();
		}

		public TNode ToXml(bool recursive)
		{
			throw new System.NotImplementedException();
		}

	}

}
