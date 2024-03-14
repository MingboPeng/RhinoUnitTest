using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Rhino.DocObjects;

namespace RhinoPlugin.Test
{
    [SetUpFixture]
    public sealed class SetupFixture : Rhino.Testing.Fixtures.RhinoSetupFixture
    {
        public override void OneTimeSetup()
        {
            base.OneTimeSetup();

            // load custom plugin
            LoadPlugins();
        }

        public override void OneTimeTearDown()
        {
            base.OneTimeTearDown();

            // remove plugins
            //ClearPlugins();
        }

        private static IEnumerable<string> GetPlugins()
        {
            var assembly = typeof(SetupFixture).Assembly;
            string dir = Path.GetDirectoryName(assembly.Location);
            var plugins = Directory.GetFiles(dir, "*.rhp");
            return plugins;
        }

        private static void LoadPlugins()
        {
            var plugins = GetPlugins(); 
            foreach (var item in plugins)
                LoadPlugin(item);
        }

        private static void ClearPlugins()
        {
            var plugins = GetPlugins();
            foreach (var item in plugins)
                File.Delete(item);
        }

        private static void LoadPlugin(string path)
        {
            var rhp = path;
            var res = Rhino.PlugIns.PlugIn.LoadPlugIn(rhp, out var guid);

            if (res == Rhino.PlugIns.LoadPlugInResult.ErrorUnknown)
                throw new System.ArgumentException($"Failed to load {rhp}");
            else if (res == Rhino.PlugIns.LoadPlugInResult.Success)
            {
                TestContext.WriteLine($"{rhp} is loaded");
            }
            else if (res == Rhino.PlugIns.LoadPlugInResult.SuccessAlreadyLoaded)
            {
                //Do nothing
            }
        }
    }
}
