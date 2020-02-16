using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace RhinoPlugin
{
    public class RhinoPluginCommand : Command
    {
        public RhinoPluginCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static RhinoPluginCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "RhinoPluginCommand"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            MakeBrep(doc);
            doc.Views.Redraw();
            return Result.Success;
        }

        public Guid MakeBrep(RhinoDoc doc)
        {
            var guid = Guid.Empty;
            var bbox = new BoundingBox(new Point3d(0, 0, 0), new Point3d(5, 5, 5));
            var brep = bbox.ToBrep();
            brep.SetUserString("myData", "myValue");
            guid = doc.Objects.AddBrep(brep);
            return guid;
        }
    }



}
