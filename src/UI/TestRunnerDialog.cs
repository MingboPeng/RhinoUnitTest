using System.Reflection;

using Eto.Drawing;
using Eto.Forms;

namespace NUnitTestRunner.UI
{

	/// <summary>Offers up a UI for the Test Runner</summary>
	internal class TestRunnerDialog : Dialog<DialogResult>
	{
		class NUnitRunArgs : EventArgs
		{
			public enum TraceLevels
			{
				Off, Error, Warning, Info, Verbose
			}
			public TraceLevels TraceLevel { get; set; }
			public List<IListItem> TestItems { get; set; }
			public NUnitRunArgs(List<IListItem> TestItems, TraceLevels Level)
			{
				this.TestItems = TestItems;
				TraceLevel = Level;
			}
		}

		ListBox _testListBox;
		public TestRunnerDialog(Assembly testAssembly, NUnitTestRunnerArgs args)
		{
			Padding = new Padding(5);
			Resizable = true;
			Title = "NUnit Test Runner";
			WindowStyle = WindowStyle.Default;
			MinimumSize = new Size(400, 400);

			var RunAll_button = new Button { Text = "RunAll" };
			RunAll_button.Click += (sender, e) => Run_button_Click(args, new NUnitRunArgs(_testListBox.Items.ToList(), NUnitRunArgs.TraceLevels.Off));

			var Run_button = new Button { Text = "Run" };
			Run_button.Click +=
				(sender, e) => Run_button_Click(args, new NUnitRunArgs(new List<IListItem>() { _testListBox.Items[Math.Max(0, _testListBox.SelectedIndex)] }, NUnitRunArgs.TraceLevels.Off));

			var Debug_button = new Button { Text = "Debug" };
			Debug_button.Click +=
				(sender, e) => Run_button_Click(args, new NUnitRunArgs(new List<IListItem>() { _testListBox.Items[Math.Max(0, _testListBox.SelectedIndex)] }, NUnitRunArgs.TraceLevels.Verbose));

			DefaultButton = new Button { Text = "Close" };
			DefaultButton.Click += (sender, e) => Close(DialogResult.Ok);

			var tests = testAssembly.GetExportedTypes()
				.Select(type => type.GetMethods().Where(typ =>
							Attribute.IsDefined(typ, typeof(NUnit.Framework.TestAttribute)) ||
							Attribute.IsDefined(typ, typeof(NUnit.Framework.TestCaseAttribute)) ||
							Attribute.IsDefined(typ, typeof(NUnit.Framework.IgnoreAttribute))
					));

			_testListBox = new ListBox();

			var items = tests;
			_testListBox.Height = 75;
			if (items != null)
			{
				foreach (var item in items)
				{
					var methods = item;
					if (!methods.Any())
						continue;

					var value = item.First().DeclaringType;
					var clsAttr = value.Attributes;
					string typeName = $"{value.Name} ({item.Count()})";
					_testListBox.Items.Add(new ListItem() { Text = typeName, Key = value.FullName, Tag = methods });
					foreach (var method in methods)
					{
						string text = string.Empty;
						if (method.GetCustomAttribute<NUnit.Framework.IgnoreAttribute>() is NUnit.Framework.IgnoreAttribute ignoreAttribute)
						{
							text = $"  ├── {method.Name} (Ignored)";
						}
						else
						{
							text = $"  ├── {method.Name}";
						}

						var testItem = new ListItem() { Text = text, Key = method.Name, Tag = method };
						_testListBox.Items.Add(testItem);
					}
				}

			}

			var defaults_layout = new TableLayout
			{
				Padding = new Padding(5, 10, 5, 5),
				Spacing = new Size(5, 5),
				Rows = { new TableRow(RunAll_button, Run_button, Debug_button, DefaultButton) }
			};

			Content = new TableLayout
			{
				Padding = new Padding(5),
				Spacing = new Size(5, 5),
				Rows =
						{
						  new TableRow(defaults_layout),
						  _testListBox
						}
			};

		}

		private void Run_button_Click(NUnitTestRunnerArgs args, NUnitRunArgs e)
		{
			Visible = false;

			try
			{
				if (e.TestItems.Count == 1)
				{
					string className = null;
					string methodName = null;

					var tests = (e.TestItems.First() as ListItem).Tag;
					if (tests is MethodInfo t)
					{
						methodName = t.Name;
						className = t.DeclaringType.Name;
					}
					else
					{
						Type declaringType = (tests as IEnumerable<MethodInfo>).First().DeclaringType;
						className = declaringType.Name;
					}

					var runner = new RhinoTestRunner(args);
					runner.Run(new SelectedRhinoTestFilter(methodName, className));
				}


			}
			catch (Exception ex)
			{
				RhinoApp.Write(ex.ToString());
			}
			finally
			{
				Visible = true;
			}

		}

	}
}
