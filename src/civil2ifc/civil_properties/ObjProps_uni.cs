using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Aec.PropertyData.DatabaseServices;

using cas = Autodesk.Civil.ApplicationServices;
using cds = Autodesk.Civil.DatabaseServices;
using GeometryGym.Ifc;

using static civil2ifc.Start;
using System.Reflection;

namespace civil2ifc.civil_properties
{
    public class ObjProps_uni
    {
		public Dictionary<string, object> props2name;
		public ObjProps_uni (object class_instance, string type)
		{
			this.props2name = new Dictionary<string, object>();

			FieldInfo[] Fields = class_instance.GetType().GetFields();
			
			List<string> permitted_types = new List<string>() { "System.Int32", "System.Double", "System.String",
			"System.Boolean","System.Guid"};

			if (type == "Pipe")
            {
				cds.Pipe obj = class_instance as cds.Pipe;


			}
		}
	}
}
