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
        public IfcPresentationStyleAssignment style_assignm = null;
        public SetMaterial(ObjectId layer_id)
        {
            Autodesk.AutoCAD.Colors.Color color = layer2color[layer_id];

            //List<double> colod_data = new List<double>();
            //double color_index()
            //{
            //    return new Random().Next(0, 256)/256d;
            //}
            //if (color.Blue == 0 && color.Red == 0 && color.Green == 0) //
            //{
            //    colod_data.Add(color_index());
            //    colod_data.Add(color_index());
            //    colod_data.Add(color_index());
            //}
            //else
            //{
            //    colod_data.Add(color.Red);
            //    colod_data.Add(color.Green);
            //    colod_data.Add(color.Blue);
            //}
            //IfcColourRgb ifc_color = new IfcColourRgb(ifc_db, colod_data[0], colod_data[1], colod_data[2]);
            IfcColourRgb ifc_color = new IfcColourRgb(ifc_db, color.Red, color.Green, color.Blue);
            IfcSurfaceStyleShading ifc_style = new IfcSurfaceStyleShading(ifc_color);
            IfcSurfaceStyle ifc_style0 = new IfcSurfaceStyle(ifc_style);
            this.style_assignm = new IfcPresentationStyleAssignment(ifc_style0);

        }
    }
}
