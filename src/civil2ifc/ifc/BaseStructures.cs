using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeometryGym.Ifc;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.BoundaryRepresentation;

using cas = Autodesk.Civil.ApplicationServices;
using cds = Autodesk.Civil.DatabaseServices;
using static civil2ifc.Start;
using Autodesk.AutoCAD.Geometry;

namespace civil2ifc.ifc
{
    public class BaseStructures
    {
        public static IfcCartesianPoint ifc_cp(double x, double y, double z)
        {
            return new IfcCartesianPoint(ifc_db, x, y, z);
        }
        public IfcFace face;
        public List<IfcFace> faces;
        public IfcFaceBasedSurfaceModel finish_surface;
        public BaseStructures (cds.TinSurfaceTriangle c3d_triangle)
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
            this.face =  new IfcFace(ifc_bound);
        }
        public BaseStructures (List<Point3d> points)
        {
            IfcCartesianPoint[] ifc_3points = new IfcCartesianPoint[points.Count];
            for (int i1 = 0; i1 < points.Count; i1++)
            {
                ifc_3points[i1] = new IfcCartesianPoint(ifc_db, points[i1].X, points[i1].Y, points[i1].Z);
            }
            IfcPolyLoop ifc_loop = new IfcPolyLoop(ifc_3points);
            IfcFaceOuterBound ifc_bound = new IfcFaceOuterBound(ifc_loop, true);
            this.face = new IfcFace(ifc_bound);
        }
        
        public BaseStructures (List<Point3d> points, List<List<int>> faces_indexed)
        {
            List<IfcCartesianPoint> ifc_points = new List<IfcCartesianPoint>();
            List<IfcFace> surf_faces = new List<IfcFace>();
            foreach (Point3d p in points)
            {
                ifc_points.Add(new IfcCartesianPoint(ifc_db, p.X, p.Y, p.Z));
            }

            foreach (List<int> indexes_group in faces_indexed)
            {
                List<IfcCartesianPoint> face_points = new List<IfcCartesianPoint>(indexes_group.Count());
                foreach (int point_index in indexes_group)
                {
                    face_points.Add(ifc_points[point_index]);
                }
                IfcPolyLoop ifc_loop = new IfcPolyLoop(face_points);
                IfcFaceOuterBound ifc_bound = new IfcFaceOuterBound(ifc_loop, true);
                surf_faces.Add( new IfcFace(ifc_bound));
            }
            this.finish_surface = new IfcFaceBasedSurfaceModel(new IfcConnectedFaceSet(surf_faces));
        }
        public BaseStructures(Extents3d e)
        {
            this.faces = new List<IfcFace>();
            IfcCartesianPoint p1 = new IfcCartesianPoint(ifc_db, e.MinPoint.X, e.MinPoint.Y, e.MinPoint.Z);
            IfcCartesianPoint p2 = new IfcCartesianPoint(ifc_db, e.MaxPoint.X - e.MinPoint.X, e.MinPoint.Y, e.MinPoint.Z);
            IfcCartesianPoint p3 = new IfcCartesianPoint(ifc_db, e.MinPoint.X, e.MaxPoint.Y - e.MinPoint.Y, e.MinPoint.Z);
            IfcCartesianPoint p4 = new IfcCartesianPoint(ifc_db, e.MaxPoint.X - e.MinPoint.X, e.MaxPoint.Y - e.MinPoint.Y, e.MinPoint.Z);

            IfcCartesianPoint p5 = new IfcCartesianPoint(ifc_db, e.MinPoint.X, e.MinPoint.Y, e.MaxPoint.Z);
            IfcCartesianPoint p6 = new IfcCartesianPoint(ifc_db, e.MaxPoint.X - e.MinPoint.X, e.MinPoint.Y, e.MaxPoint.Z);
            IfcCartesianPoint p7 = new IfcCartesianPoint(ifc_db, e.MinPoint.X, e.MaxPoint.Y - e.MinPoint.Y, e.MaxPoint.Z);
            IfcCartesianPoint p8 = new IfcCartesianPoint(ifc_db, e.MaxPoint.X - e.MinPoint.X, e.MaxPoint.Y - e.MinPoint.Y, e.MaxPoint.Z);

            this.faces.Add(new IfcFace(new IfcFaceOuterBound(new IfcPolyLoop(new List<IfcCartesianPoint> { p1, p2, p3, p4 }), true)));
            this.faces.Add(new IfcFace(new IfcFaceOuterBound(new IfcPolyLoop(new List<IfcCartesianPoint> { p1, p3, p5, p7 }), true)));
            this.faces.Add(new IfcFace(new IfcFaceOuterBound(new IfcPolyLoop(new List<IfcCartesianPoint> { p5, p4, p8, p7 }), true)));
            this.faces.Add(new IfcFace(new IfcFaceOuterBound(new IfcPolyLoop(new List<IfcCartesianPoint> { p2, p4, p8, p6 }), true)));
            this.faces.Add(new IfcFace(new IfcFaceOuterBound(new IfcPolyLoop(new List<IfcCartesianPoint> { p1, p5, p6, p2 }), true)));
            this.faces.Add(new IfcFace(new IfcFaceOuterBound(new IfcPolyLoop(new List<IfcCartesianPoint> { p5, p6, p8, p7 }), true)));
        }
        public BaseStructures (Solid3d input_solid)
        {
            //Temporal containers for external surface definition (faces indexes and Point3d list)
            List<List<int>> faces_indexed = new List<List<int>>();
            List<Point3d> points_temp = new List<Point3d>();
            
            double length = input_solid.GeometricExtents.MinPoint.GetVectorTo(input_solid.GeometricExtents.MaxPoint).Length;

            this.faces = new List<IfcFace>();
            try
            {
                using (Brep brep = new Brep(input_solid))
                {
                    using (Mesh2dControl mc = new Mesh2dControl())
                    {
                        //Set mesh control about how to strip the mesh
                        mc.MaxNodeSpacing = length;
                        mc.MaxSubdivisions = 1;
                        using (Mesh2dFilter mf = new Mesh2dFilter())
                        {
                            mf.Insert(brep, mc);
                            using (Mesh2d m = new Mesh2d(mf))
                            {
                                Dictionary<Point3d, IfcCartesianPoint> pts = new Dictionary<Point3d, IfcCartesianPoint>();
                                foreach (Element2d e in m.Element2ds)
                                {
                                    foreach (Node n in e.Nodes)
                                    {
                                        if (pts.ContainsKey(n.Point))
                                            continue;

                                        //Get each point
                                        pts.Add(n.Point, new IfcCartesianPoint(ifc_db, n.Point.X, n.Point.Y, n.Point.Z));
                                        n.Dispose();
                                    }
                                    e.Dispose();
                                }

                                foreach (Element2d e in m.Element2ds)
                                {
                                    List<IfcCartesianPoint> points_face = new List<IfcCartesianPoint>();
                                    foreach (Node n in e.Nodes)
                                    {
                                        //Group each polygon
                                        points_face.Add(pts[n.Point]);
                                    }
                                    IfcFaceOuterBound outerBound = new IfcFaceOuterBound(new IfcPolyLoop(points_face), true);
                                    this.faces.Add(new IfcFace(outerBound));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                this.faces = null;
                //Extents3d solid_extent = input_solid.GeometricExtents;
                //this.faces = new ifc.BaseStructures(solid_extent).faces;
                
            }
        }
        public BaseStructures (Point3d center, double height)
        {
            
        }
    }
}
