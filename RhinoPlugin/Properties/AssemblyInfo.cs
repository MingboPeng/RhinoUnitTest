using System.Runtime.InteropServices;
using Rhino.PlugIns;

// Plug-in Description Attributes - all of these are optional.
// These will show in Rhino's option dialog, in the tab Plug-ins.
[assembly: PlugInDescription(DescriptionType.Address, "-")]
[assembly: PlugInDescription(DescriptionType.Country, "-")]
[assembly: PlugInDescription(DescriptionType.Email, "-")]
[assembly: PlugInDescription(DescriptionType.Phone, "-")]
[assembly: PlugInDescription(DescriptionType.Fax, "-")]
[assembly: PlugInDescription(DescriptionType.Organization, "-")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "-")]
[assembly: PlugInDescription(DescriptionType.WebSite, "-")]

// Icons should be Windows .ico files and contain 32-bit images in the following sizes: 16, 24, 32, 48, and 256.
// This is a Rhino 6-only description.
[assembly: PlugInDescription(DescriptionType.Icon, "RhinoPlugin.EmbeddedResources.plugin-utility.ico")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c5969102-399d-4ba4-be1f-141f18947009")] // This will also be the Guid of the Rhino plug-in

