using NUnit.Framework;
using NUnitTestRunner.Console;
using System;
using System.Collections;
using System.Linq;

namespace NUnitTestRunnerTests
{

    public sealed class Tests
    {

        [Test]
        public void NoOutputLogArgument()
        {
            string noOutputLogArgs = $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.NoOutputLog)}";
            var testArgs = ArgHandler.Parse(noOutputLogArgs);
            Assert.That(testArgs.NoOutputLog, Is.True);
        }

        [Test]
        public void QuietArgument()
        {
            string quietArgs = $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Quiet)}";
            var testArgs = ArgHandler.Parse(quietArgs);
            Assert.That(testArgs.Quiet, Is.True);
        }

        [Test]
        public void StopOnFailArgument()
        {
            string stopOnFailArgs = $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.StopOnFail)}";
            var testArgs = ArgHandler.Parse(stopOnFailArgs);
            Assert.That(testArgs.StopOnFail, Is.True);
        }

        [Test]
        public void DebugArgument()
        {
            string debugArgs = $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Debug)}";
            var testArgs = ArgHandler.Parse(debugArgs);
            Assert.That(testArgs.Debug, Is.True);
        }

        [Test]
        public void ExitOnFinishArgument()
        {
            string exitOnFinishArgs = $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.ExitOnFinish)}";
            var testArgs = ArgHandler.Parse(exitOnFinishArgs);
            Assert.That(testArgs.ExitOnFinish, Is.True);
        }

        [Test]
        public void HelpArgument()
        {
            string helpArgs = $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Help)}";
            var testArgs = ArgHandler.Parse(helpArgs);
            Assert.That(testArgs.Help, Is.True);
        }

        [Test]
        public void FormatArgument()
        {
            string formatArgs = $"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Format)}+{nameof(OutputFormat.NUnit)}";
            var testArgs = ArgHandler.Parse(formatArgs);
            Assert.That(testArgs.Format, Is.EqualTo(OutputFormat.NUnit));
        }

        [Test]
        public void DllArgument()
        {
            string dllLocation = typeof(Tests).Assembly.Location;
            string dllArgs = $"{dllLocation}";
            var testArgs = ArgHandler.Parse(dllArgs);
            Assert.That(testArgs.Dlls.FirstOrDefault().ToLower(), Is.EqualTo(dllLocation.ToLower()));
        }

        [TestCaseSource(typeof(TestRunnerArgs), nameof(TestRunnerArgs.ValidTestCases))]
        public void ValidCommands(string args)
        {
            NUnitTestRunnerArgs testArgs = ArgHandler.Parse(args);
            // testArgs.Quiet
        }

        [TestCaseSource(typeof(TestRunnerArgs), nameof(TestRunnerArgs.InValidTestCases))]
        public void InvalidCommands(string args)
        {
            Assert.Throws<ArgumentException>(() => ArgHandler.Parse(args));
        }

        public sealed class TestRunnerArgs
        {
            public static IEnumerable ValidTestCases
            {
                get
                {
                    yield return new TestCaseData($"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.Quiet)}");
                    yield return new TestCaseData($"{ArgHandler.PreCommand}{nameof(NUnitTestRunnerArgs.NoOutputLog)}");
                }
            }
            public static IEnumerable InValidTestCases
            {
                get
                {
                    yield return new TestCaseData("-halp");
                    yield return new TestCaseData("----");
                    yield return new TestCaseData("--nonexistantarg");
                    yield return new TestCaseData(null);
                    yield return new TestCaseData("😉");
                    yield return new TestCaseData("--😉");
                    yield return new TestCaseData("\\");
                    yield return new TestCaseData("{}");
                    yield return new TestCaseData("//");
                    yield return new TestCaseData($"{ArgHandler.Delimiter}");
                    yield return new TestCaseData($"{ArgHandler.DllDelimiter}");
                    yield return new TestCaseData($"{ArgHandler.PreCommand}");
                    yield return new TestCaseData("|");
                    yield return new TestCaseData($"|{nameof(NUnitTestRunnerArgs.Quiet)}");
                    yield return new TestCaseData($"{ArgHandler.PreCommand[0]}{nameof(NUnitTestRunnerArgs.Quiet)}");
                    yield return new TestCaseData($"{ArgHandler.Delimiter}{nameof(NUnitTestRunnerArgs.Quiet)}");
                    yield return new TestCaseData($"{ArgHandler.DllDelimiter}{nameof(NUnitTestRunnerArgs.Quiet)}");
                    yield return new TestCaseData($"{nameof(NUnitTestRunnerArgs.Quiet)}");
                }
            }
        }

    }

}