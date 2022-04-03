using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeometryGym.Ifc;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

using cas = Autodesk.Civil.ApplicationServices;
using cds = Autodesk.Civil.DatabaseServices;
using static civil2ifc.Start;

namespace civil2ifc.ifc
{
    public class BaseStructures
    {
        public static IfcCartesianPoint ifc_cp(double x, double y, double z)
        {
            return new IfcCartesianPoint(ifc_db, x, y, z);
        }
        public static IfcFace ifc_face (cds.TinSurfaceTriangle c3d_triangle)
        {
            IfcCartesianPoint get_by_vertex(cds.TinSurfaceVertex vertex)
            {
                return new IfcCartesianPoint(ifc_db, vertex.Location.X,
                    vertex.Location.Y, vertex.Location.Z);
            }
            IfcCartesianPoint[] ifc_3points = new IfcCartesianPoint[3];
            ifc_3points[0] = get_by_vertex(c3d_triangle.Vertex1);
            ifc_3points[1] = get_by_vertex(c3d_triangle.Vertex2);
            ifc_3points[2] = get_by_vertex(c3d_triangle.Vertex3);

            IfcPolyLoop ifc_loop = new IfcPolyLoop(ifc_3points);
            IfcFaceOuterBound ifc_bound = new IfcFaceOuterBound(ifc_loop, true);
            //IfcFaceBound ifc_bound = new IfcFaceBound(ifc_loop, true);
            return new IfcFace(ifc_bound);
        }

    }
}
