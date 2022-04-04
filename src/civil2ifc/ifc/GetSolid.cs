using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace civil2ifc.ifc
{
    public class GetSolid
    {
        public IfcFaceBasedSurfaceModel surf_row;
        public GetSolid(Solid3d input_solid)
        {
            //FullSubentityPath path = new FullSubentityPath(new ObjectId[1] { input_solid.Id }, new SubentityId(SubentityType.Null, IntPtr.Zero));
            List<IfcFace> surf_faces = new List<IfcFace>();
            

            using (Brep brep = new Brep(input_solid))
            {
                BrepFaceCollection brep_faces = brep.Faces;
                int v_count = brep_faces.Count();
                foreach (Autodesk.AutoCAD.BoundaryRepresentation.Face face in brep_faces)
                {
                    foreach (BoundaryLoop lp in face.Loops)
                    {
                        List<Point3d> face_points = new List<Point3d>();
                        foreach (Edge edg in lp.Edges)
                        {
                            if (!face_points.Contains(edg.Vertex1.Point)) face_points.Add(edg.Vertex1.Point);
                            if (!face_points.Contains(edg.Vertex2.Point)) face_points.Add(edg.Vertex2.Point);
                        }
                        if (face_points.Count() > 2) surf_faces.Add(new ifc.BaseStructures(face_points).face);
                    }
                   

                    
                    //face.Dispose();
                }
            }
            surf_row = new IfcFaceBasedSurfaceModel(new IfcConnectedFaceSet(surf_faces));
        }
    }
}
