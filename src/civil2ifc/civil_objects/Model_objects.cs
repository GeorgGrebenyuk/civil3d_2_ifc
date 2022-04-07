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

namespace civil2ifc.civil_objects
{
    public class Model_objects
    {
        private ObjectIdCollection objects_id;
        public Model_objects(ObjectIdCollection ids, string type)
        {
            this.objects_id = ids;
            switch (type)
            {
                case "Surfaces":
                    Surfaces();
                    break;
                case "PipeNetworks":
                    PipeNetworks();
                    break;
                case "Solids":
                    Solids();
                    break;
            }
        }
        private void Surfaces()
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in objects_id)
                    {
                        cds.TinSurface tin_surf = acTrans.GetObject(id, OpenMode.ForRead) as cds.TinSurface;
                        cds.TinSurfaceTriangleCollection trs = tin_surf.GetTriangles(false);

                        List<IfcFace> surf_faces = new List<IfcFace>();
                        foreach (cds.TinSurfaceTriangle tr in trs)
                        {
                            surf_faces.Add(new ifc.BaseStructures(tr).face);
                        }
                        new ifc.AddObject(surf_faces, ifc_site, id,  tin_surf.LayerId);
                    }
                    acTrans.Commit();
                }
            }
        }
        private void Solids()
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId solid_id in this.objects_id)
                    {
                        //Geometry
                        Solid3d model_solid = acTrans.GetObject(solid_id, OpenMode.ForRead) as Solid3d;
                        var object_solid = new ifc.BaseStructures(model_solid).faces;
                        new ifc.AddObject(object_solid, ifc_site, solid_id, model_solid.LayerId);
                    }
                    acTrans.Commit();
                }
            }
        }
        private void PipeNetworks()
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in this.objects_id)
                    {
                        cds.Network pipes_network = acTrans.GetObject(id, OpenMode.ForRead) as cds.Network;
                        string pipe_network_name = pipes_network.Name;
                        IfcSystem pipe_network_system = new IfcSystem(ifc_db, pipe_network_name);

                        //Workings with pipes and structures
                        ObjectIdCollection pipes_ids = pipes_network.GetPipeIds();
                        foreach (ObjectId pipe_id in pipes_ids)
                        {
                            cds.Pipe new_p = acTrans.GetObject(pipe_id, OpenMode.ForRead) as cds.Pipe;
                            var pipe_solid = new ifc.BaseStructures(new_p.Solid3dBody).faces;
                            new ifc.AddObject(pipe_solid, pipe_network_system, id, new_p.LayerId);
                        }

                        ObjectIdCollection structures_ids = pipes_network.GetStructureIds();
                        foreach (ObjectId pipe_id in structures_ids)
                        {
                            cds.Structure new_s = acTrans.GetObject(pipe_id, OpenMode.ForRead) as cds.Structure;
                             
                            if (!new_s.Description.Contains("улевой колоде"))
                            {
                                var structure_solid = new ifc.BaseStructures(new_s.Solid3dBody).faces;
                                new ifc.AddObject(structure_solid, pipe_network_system, id, new_s.LayerId);
                            }
                            else
                            {
                                IfcCartesianPoint p = new IfcCartesianPoint(ifc_db, (new_s.StartPoint.X + new_s.EndPoint.X) / 2d,
                                    (new_s.StartPoint.Y + new_s.EndPoint.Y) / 2d, (new_s.StartPoint.Z + new_s.EndPoint.Z) / 2d);
                                new ifc.AddObject(p, pipe_network_system, id, new_s.LayerId);
                            }
                                 
                             
                            

                        }
                    }
                    acTrans.Commit();
                }
            }
        }
    }
}
