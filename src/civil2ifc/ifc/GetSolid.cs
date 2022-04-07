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
        public GetSolid(ObjectId input_solid_id)
        {

            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    Solid3d input_solid = acTrans.GetObject(input_solid_id, OpenMode.ForRead) as Solid3d;
                    //Temporal containers for external surface definition (faces indexes and Point3d list)
                    List<List<int>> faces_indexed = new List<List<int>>();
                    List<Point3d> points_temp = new List<Point3d>();

                    using (Brep brp = new Brep(input_solid))
                    {
                        foreach (Autodesk.AutoCAD.BoundaryRepresentation.Face fce in brp.Faces)
                        {
                            List<int> coord_indexes = new List<int>(); // КУДА ЭТО????
                            foreach (BoundaryLoop lp in fce.Loops)
                            {
                                foreach (Edge edg in lp.Edges)
                                {
                                    vertex_opeations(edg.Vertex1);
                                    vertex_opeations(edg.Vertex2);
                                    void vertex_opeations(Autodesk.AutoCAD.BoundaryRepresentation.Vertex v)
                                    {
                                        if (!points_temp.Contains(v.Point))
                                        {
                                            points_temp.Add(v.Point);
                                            coord_indexes.Add(points_temp.Count() - 1);
                                        }
                                        else
                                        {
                                            int point_last = points_temp.FindIndex(a => a == v.Point);
                                            coord_indexes.Add(point_last);
                                        }
                                    }
                                }
                            }
                            faces_indexed.Add(coord_indexes);
                        }
                    }
                    this.surf_row = new ifc.BaseStructures(points_temp, faces_indexed).finish_surface;
                    acTrans.Commit();
                }

            }
        }
    }
}
