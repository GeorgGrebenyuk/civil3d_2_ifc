using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

using cas = Autodesk.Civil.ApplicationServices;
using cds = Autodesk.Civil.DatabaseServices;
using GeometryGym.Ifc;

using static civil2ifc.Start;

namespace civil2ifc.civil_properties
{
    public class SetMaterial
    {
        //private IfcObject base_object;
        public IfcPresentationStyleAssignment style_assignm;
        public SetMaterial(Autodesk.AutoCAD.Colors.Color color)
        {
            IfcColourRgb ifc_color = new IfcColourRgb(ifc_db, color.Red, color.Green, color.Blue);
            IfcSurfaceStyleShading ifc_style = new IfcSurfaceStyleShading(ifc_color);
            IfcSurfaceStyle ifc_style2 = new IfcSurfaceStyle(ifc_style);
            this.style_assignm = new IfcPresentationStyleAssignment(ifc_style2);
            
        }
    }
}
