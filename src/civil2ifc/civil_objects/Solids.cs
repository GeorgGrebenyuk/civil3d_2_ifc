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
using System.Reflection;

using static civil2ifc.Start;
using Autodesk.AutoCAD.EditorInput;

namespace civil2ifc.civil_objects
{
    public class Solids
    {
        public static void Create ()
        {
            List<ObjectId> solid_ids = new List<ObjectId>();
            TypedValue[] search_conditions = new TypedValue[1];
            search_conditions[0] = new TypedValue((int)DxfCode.Start, "3DSOLID");
            PromptSelectionResult obj_group = Application.DocumentManager.MdiActiveDocument.Editor.SelectAll(new SelectionFilter(search_conditions));
            if (obj_group.Status == PromptStatus.OK) solid_ids = obj_group.Value.GetObjectIds().ToList();

            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId solid_id in solid_ids)
                    {
                        //Geometry
                        Solid3d model_solid = acTrans.GetObject(solid_id, OpenMode.ForRead) as Solid3d;
                        var object_solid = new ifc.GetSolid(model_solid).surf_row;

                        IfcStyledItem object_style = new IfcStyledItem(object_solid, new civil_properties.SetMaterial(model_solid.LayerId).style_assignm);
                        IfcShapeRepresentation solid_shape_pipe = new IfcShapeRepresentation(object_solid);
                        IfcProductDefinitionShape solid_shape2_pipe = new IfcProductDefinitionShape(solid_shape_pipe);
                        IfcBuildingElementProxy pipe_item = new IfcBuildingElementProxy(ifc_site, base_placement, solid_shape2_pipe);
                        new PropSet(solid_id, pipe_item); 
                    }
                    acTrans.Commit();
                }
            }


        }
    }
}
