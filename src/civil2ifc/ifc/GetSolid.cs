using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Aec.PropertyData.DatabaseServices;
using Autodesk.AutoCAD.BoundaryRepresentation;

using cas = Autodesk.Civil.ApplicationServices;
using cds = Autodesk.Civil.DatabaseServices;
using GeometryGym.Ifc;

using static civil2ifc.Start;
using Autodesk.AutoCAD.Geometry;
using stl = QuantumConcepts.Formats.StereoLithography;

namespace civil2ifc.ifc
{
    public class GetSolid
    {
        public IfcFaceBasedSurfaceModel surf_row;
        public IfcPolygonalFaceSet surf_row2;
        public GetSolid(ObjectId input_solid_id)
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    List<IfcFace> surf_faces = new List<IfcFace>();

                    string solid_path = $@"C:\Users\{Environment.UserName}\AppData\Local\Temp\{Guid.NewGuid()}.stl";
                    Solid3d input_solid = acTrans.GetObject(input_solid_id, OpenMode.ForRead) as Solid3d;
                    input_solid.StlOut(solid_path, true);
                    stl.STLDocument solid_stl = stl.STLDocument.Open(solid_path);
                    List<stl.Facet> stl_facets = solid_stl.Facets.ToList();
                    foreach (stl.Facet stl_one_facet in stl_facets)
                    {
                        surf_faces.Add(new ifc.BaseStructures(stl_one_facet).face);
                    }
                    File.Delete(solid_path);
                    this.surf_row = new IfcFaceBasedSurfaceModel(new IfcConnectedFaceSet(surf_faces));
                    acTrans.Commit();
                }
            }
            //FullSubentityPath path = new FullSubentityPath(new ObjectId[1] { input_solid.Id }, new SubentityId(SubentityType.Null, IntPtr.Zero));



            //List<Tuple<double, double, double>> points_temp = new List<Tuple<double, double, double>>();
            //List<IfcIndexedPolygonalFace> faces_indexed = new List<IfcIndexedPolygonalFace>();
            //List<Point3d> face_points = new List<Point3d>();




            //IfcCartesianPointList3D collection = new IfcCartesianPointList3D(ifc_db, points_temp);
            //this.surf_row = new IfcPolygonalFaceSet(collection, faces_indexed);
            
        }
    }
}
