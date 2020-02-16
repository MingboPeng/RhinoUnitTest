using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Common;
using NUnitLite;
using Rhino;
using Rhino.Commands;
using Rhino.UI;
using Command = Rhino.Commands.Command;

namespace NUnitTestRunner
{
    public class NUnitTestRunnerCommand : Command
    {
        public NUnitTestRunnerCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static NUnitTestRunnerCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "NUnitTestRunnerCommand"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            var rc = Result.Cancel;

            if (mode == RunMode.Interactive)
            {

                var assembly = typeof(RhinoPlugin.Test.TestClass).Assembly;
                var dialog = new TestRunnerDialog(assembly);
                dialog.RestorePosition();
                var dialog_rc = dialog.ShowSemiModal(doc, RhinoEtoApp.MainWindow);
                dialog.SavePosition();
                if (dialog_rc == DialogResult.Ok)
                    rc = Result.Success;
            }
            else
            {
                var msg = string.Format("Scriptable version of {0} command not implemented.", EnglishName);
                RhinoApp.WriteLine(msg);
            }

            return rc;
        }
    }

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
            this.TraceLevel = Level;
        }
    }

    class TestRunnerDialog : Dialog<DialogResult>
    {
        ListBox _testListBox;
        public TestRunnerDialog(System.Reflection.Assembly testAssembly)
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = "NUnit Test Runner";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(400, 400);
            var assembly = testAssembly;

            var RunAll_button = new Button { Text = "RunAll" };
            RunAll_button.Click += (sender, e) => Run_button_Click(assembly, new NUnitRunArgs(_testListBox.Items.ToList(), NUnitRunArgs.TraceLevels.Off));

            var Run_button = new Button { Text = "Run" };
            Run_button.Click +=
                (sender, e) => Run_button_Click(assembly, new NUnitRunArgs(new List<IListItem>() { _testListBox.Items[Math.Max(0, _testListBox.SelectedIndex)] }, NUnitRunArgs.TraceLevels.Off));

            var Debug_button = new Button { Text = "Debug" };
            Debug_button.Click +=
                (sender, e) => Run_button_Click(assembly, new NUnitRunArgs(new List<IListItem>() { _testListBox.Items[Math.Max(0, _testListBox.SelectedIndex)] }, NUnitRunArgs.TraceLevels.Verbose));

            DefaultButton = new Button { Text = "Close" };
            DefaultButton.Click += (sender, e) => Close(DialogResult.Ok);

            var tests = assembly.GetTypes()
                .Where(_ => Attribute.IsDefined(_, typeof(NUnit.Framework.TestFixtureAttribute)))
                .Select(type => type.GetMethods().Where(_ => Attribute.IsDefined(_, typeof(NUnit.Framework.TestAttribute))));


            _testListBox = new ListBox();
            var items = tests;
            _testListBox.Height = 90;
            if (items != null)
            {
                foreach (var item in items)
                {
                    var methods = item;
                    if (!methods.Any())
                        continue;

                    var value = item.First().DeclaringType;
                    var clsAttr = value.Attributes;
                    _testListBox.Items.Add(new ListItem() { Text = value.Name, Key = value.FullName, Tag = methods });
                    foreach (var method in methods)
                    {
                        var attr = method.Attributes;
                        var testItem = new ListItem() { Text = "  |-- " + method.Name, Key = method.Name, Tag = method };
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


        private void Run_button_Click(System.Reflection.Assembly testAssembly, NUnitRunArgs e)
        {

            try
            {
                //https://github.com/nunit/docs/wiki/Console-Command-Line
                string[] args = new string[] { };
                if (e.TestItems.Count == 1)
                {
                    var tests = (e.TestItems.First() as ListItem).Tag;
                    if (tests is System.Reflection.MethodInfo t)
                    {
                        args = new string[] { $"--where=test=={ t.DeclaringType.FullName}.{t.Name}", $"--trace={e.TraceLevel}" };
                    }
                    else
                    {
                        var className = (tests as IEnumerable<System.Reflection.MethodInfo>).First().DeclaringType.FullName;
                        args = new string[] { $"--where=class=={className}", $"--trace={e.TraceLevel}" };
                    }

                    var assembly = testAssembly;
                    new AutoRun(assembly).Execute(args, new RhinoConsoleTextWriter(), null);
                }
             

            }
            catch (Exception ex)
            {
                RhinoApp.Write(ex.ToString());
            }

        }

    }


    //https://github.com/JoinCAD/RhinoNUnitTestRunner
    class RhinoConsoleTextWriter : NUnit.Common.ExtendedTextWriter
    {
        public override Encoding Encoding
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Write(ColorStyle style, string value)
        {
            Rhino.RhinoApp.Write(value);
        }

        public override void WriteLabel(string label, object option)
        {
            Rhino.RhinoApp.Write(label + option?.ToString());
        }

        public override void WriteLabel(string label, object option, ColorStyle valueStyle)
        {
            Rhino.RhinoApp.Write(label + option?.ToString());
        }

        public override void WriteLabelLine(string label, object option)
        {
            Rhino.RhinoApp.WriteLine(label + option?.ToString());
        }

        public override void WriteLabelLine(string label, object option, ColorStyle valueStyle)
        {
            Rhino.RhinoApp.WriteLine(label + option?.ToString());
        }

        public override void WriteLine(ColorStyle style, string value)
        {
            Rhino.RhinoApp.WriteLine(value);
        }
    }
}
