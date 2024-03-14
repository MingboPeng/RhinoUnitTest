using System;
using NUnit.Framework;
using Rhino;
using Rhino.Geometry;

namespace RhinoPlugin.Test
{
    [TestFixture]
    public class TestClass: Rhino.Testing.Fixtures.RhinoTestFixture
    {
        [Test]
        public void TestMethod_Rhino()
        {
            var doc = RhinoDoc.Create(null);
            var guid = Guid.Empty;
            var bbox = new BoundingBox(new Point3d(0, 0, 0), new Point3d(5, 5, 5));
            var brep = bbox.ToBrep();
            brep.SetUserString("myData", "myValue");
            guid = doc.Objects.AddBrep(brep);

            var rhinoObj = doc.Objects.FindId(guid);
            var data = rhinoObj.Geometry.GetUserString("myData");
     
            Assert.AreEqual(data, "myValue");

        }

        [Test]
        public void TestMethod_MyPlugin()
        {
            var doc = RhinoDoc.Create(null);
            var guid = RhinoPluginCommand.Instance.MakeBrep(doc);
            var rhinoObj = doc.Objects.FindId(guid);
            var data = rhinoObj.Geometry.GetUserString("myData");

            Assert.AreEqual(data, "myValue");

        }
    }
}
