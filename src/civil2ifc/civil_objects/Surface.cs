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


namespace civil2ifc.civil_objects
{
    
    public class Surface
    {

        public static void Create(ObjectIdCollection ids)
        {
            
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in ids)
                    {
                        cds.TinSurface tin_surf = acTrans.GetObject(id, OpenMode.ForRead) as cds.TinSurface;
                        cds.TinSurfaceTriangleCollection trs = tin_surf.GetTriangles(false);
                        List<IfcFace> surf_faces = new List<IfcFace>();
                        foreach (cds.TinSurfaceTriangle tr in trs)
                        {
                            surf_faces.Add(ifc.BaseStructures.ifc_face(tr));
                        }
                        IfcFaceBasedSurfaceModel ifc_surf_model = new IfcFaceBasedSurfaceModel(new IfcConnectedFaceSet(surf_faces));
                        IfcShapeRepresentation surface_repr = new IfcShapeRepresentation(ifc_surf_model);

                        IfcSite new_site = new IfcSite(ifc_site, tin_surf.Name);
                        new_site.Representation = new IfcProductDefinitionShape(surface_repr);

                        PropSet props = new PropSet(id, new_site); //new_site.GlobalId
                        cds.TerrainSurfaceProperties surf_terr_props = tin_surf.GetTerrainProperties();
                        Dictionary<string, Dictionary<string, object>> internal_surf_props = new Dictionary<string, Dictionary<string, object>>();
                        internal_surf_props.Add("Terrarian_Properties", get_terrarian_properties(tin_surf.GetTerrainProperties()));
                        internal_surf_props.Add("General_Properties", get_general_properties(tin_surf.GetGeneralProperties()));
                        internal_surf_props.Add("Tin_Properties", get_tin_properties(tin_surf.GetTinProperties()));
                        new ifc.IfcProps(internal_surf_props, new_site);
                    }
                    acTrans.Commit();
                }
            }
        }
        private static Dictionary<string,object> get_terrarian_properties(cds.TerrainSurfaceProperties surf_terr_props)
        {
            return new Dictionary<string, object>
            {
                {"MaximumGradeOrSlope",surf_terr_props.MaximumGradeOrSlope },
                {"MeanGradeOrSlope",surf_terr_props.MeanGradeOrSlope},
                {"MinimumGradeOrSlope",surf_terr_props.MinimumGradeOrSlope },
                {"SurfaceArea2D",surf_terr_props.SurfaceArea2D },
                {"SurfaceArea3D",surf_terr_props.SurfaceArea2D }
            };
        }
        private static Dictionary<string, object> get_general_properties(cds.GeneralSurfaceProperties surf_general_props)
        {
            return new Dictionary<string, object>
            {
                {"MaximumCoordinateX",surf_general_props.MaximumCoordinateX },
                {"MaximumCoordinateY",surf_general_props.MaximumCoordinateY},
                {"MaximumElevation",surf_general_props.MaximumElevation },
                {"MeanElevation",surf_general_props.MeanElevation },
                {"MinimumCoordinateX",surf_general_props.MinimumCoordinateX },
                {"MinimumCoordinateY",surf_general_props.MinimumCoordinateY },
                {"MinimumElevation",surf_general_props.MinimumElevation },
                {"MNumberOfPoints",surf_general_props.NumberOfPoints },
                {"RevisionNumber",surf_general_props.RevisionNumber }
            };
        }
        private static Dictionary<string, object> get_tin_properties(cds.TinSurfaceProperties surf_tin_props)
        {
            return new Dictionary<string, object>
            {
                {"MaximumTriangleArea",surf_tin_props.MaximumTriangleArea },
                {"MaximumTriangleLength",surf_tin_props.MaximumTriangleLength},
                {"MinimumTriangleArea",surf_tin_props.MinimumTriangleArea },
                {"MinimumTriangleLength",surf_tin_props.MinimumTriangleLength},
                {"NumberOfTriangles",surf_tin_props.NumberOfTriangles }
            };
        }


    }
}
