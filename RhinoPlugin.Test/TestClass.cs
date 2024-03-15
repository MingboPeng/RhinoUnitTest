using System.Linq;
using NUnit.Framework;
using Rhino;
using Rhino.Geometry;


namespace RhinoPlugin.Test
{
    [TestFixture]
    public class TestClass: Rhino.Testing.Fixtures.RhinoTestFixture
    {
        [Test]
        public void TestRhinoCommon()
        {
            var doc = RhinoDoc.Create(null);
            var bbox = new BoundingBox(new Point3d(0, 0, 0), new Point3d(5, 5, 5));
            var brep = bbox.ToBrep();
            brep.SetUserString("myData", "myValue");
            var guid = doc.Objects.AddBrep(brep);

            var rhinoObj = doc.Objects.FindId(guid);
            var data = rhinoObj.Geometry.GetUserString("myData");

            Assert.That(data, Is.EqualTo("myValue"));

        }
        [Test]
        public void Load_MyPlugin()
        {
            var id = new System.Guid("c5969102-399d-4ba4-be1f-141f18947009");
            var pluginInfo =  Rhino.PlugIns.PlugIn.GetPlugInInfo(id);
            Assert.That(pluginInfo, Is.Not.Null);
            if (!pluginInfo.IsLoaded)
            {
                Rhino.PlugIns.PlugIn.LoadPlugIn(id);
            }

            var plugin = RhinoPlugin.Instance;
            if (plugin == null)
            {
                var p = Rhino.Runtime.HostUtils.CreatePlugIn(typeof(RhinoPlugin), true);
                plugin = p as RhinoPlugin;
                Rhino.Runtime.HostUtils.CreateCommands(plugin);
            }

            Assert.That(plugin, Is.Not.Null);

            // test command instance
            var cmd = RhinoPluginCommand.Instance;
            Assert.That(cmd, Is.Not.Null);

        }

        [Test]
        public void TestMethod_MyPlugin()
        {
           
            var doc = RhinoDoc.Create(null);
            RhinoPluginCommand.Instance.ExecuteCommand(doc);

            var rhinoObj = doc.Objects.FirstOrDefault();
            var data = rhinoObj.Geometry.GetUserString("myData");

            Assert.That(data, Is.EqualTo("myValue"));
        }

    }
}
