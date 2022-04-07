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
//using stl = QuantumConcepts.Formats.StereoLithography;
//using dxf = netDxf;

namespace civil2ifc.ifc
{
    public class GetSolid
    {
        public IfcFaceBasedSurfaceModel surf_row;
        //public IfcFaceBasedSurfaceModel surf_row;
        public GetSolid(Solid3d input_solid)
        {
            //Temporal containers for external surface definition (faces indexes and Point3d list)
            List<List<int>> faces_indexed = new List<List<int>>();
            List<Point3d> points_temp = new List<Point3d>();

            double length = input_solid.GeometricExtents.MinPoint.GetVectorTo(input_solid.GeometricExtents.MaxPoint).Length;

            List<IfcFace> surf_faces = new List<IfcFace>();
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
                                    surf_faces.Add(new IfcFace(outerBound));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Extents3d solid_extent = input_solid.GeometricExtents;
                surf_faces = new ifc.BaseStructures(solid_extent).faces;
            }
            
            this.surf_row = new IfcFaceBasedSurfaceModel(new IfcConnectedFaceSet(surf_faces));

        }
    }
}
