// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.DocObjects;

namespace RhinoPlugin.Test
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            var doc = Rhino.RhinoDoc.ActiveDoc;
            var guid = RhinoPluginCommand.Instance.MakeBrep(doc);
            var rhinoObj = doc.Objects.FindId(guid);
            var data = rhinoObj.Geometry.GetUserString("myData");

            Assert.AreEqual(data, "myValue");
        }
    }
}
