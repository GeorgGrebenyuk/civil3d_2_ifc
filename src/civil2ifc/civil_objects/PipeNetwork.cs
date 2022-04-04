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
    public class PipeNetwork
    {
        private static List<cds.Pipe> pipes;
        private static IfcObject parent_pipe_network;
        public static void Create(ObjectIdCollection ids)
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in ids)
                    {
                        cds.Network pipes_network = acTrans.GetObject(id, OpenMode.ForRead) as cds.Network;
                        string pipe_network_name = pipes_network.Name;
                        IfcSystem pipe_network_system = new IfcSystem(ifc_db, pipe_network_name);
                        parent_pipe_network = pipe_network_system;
                        

                        //Workings with pipes and structures
                        //cds.SectionPipeNetwork tin_surf = acTrans.GetObject(id, OpenMode.ForRead) as cds.SectionPipeNetwork;
                        ObjectIdCollection pipes_ids = pipes_network.GetPipeIds();
                        pipes = new List<cds.Pipe>();
                        foreach (ObjectId pipe_id in pipes_ids)
                        {
                            pipes.Add(acTrans.GetObject(pipe_id, OpenMode.ForRead) as cds.Pipe);
                        }
                        ObjectIdCollection structures_ids = pipes_network.GetStructureIds();
                    }
                    acTrans.Commit();
                }
            }
        }
        private void WorksWithPipes()
        {
            foreach (cds.Pipe one_pipe in pipes)
            {
                //Geometry
                IfcCartesianPoint pipe_start = new IfcCartesianPoint(ifc_db, one_pipe.StartPoint.X,
                    one_pipe.StartPoint.Y, one_pipe.StartPoint.Z);
                //IfcDirection pipe_axis = new IfcDirection(ifc_db, one_pipe.EndPoint.X - one_pipe.StartPoint.X,
                //    one_pipe.EndPoint.Y - one_pipe.StartPoint.Y, one_pipe.EndPoint.Z - one_pipe.StartPoint.Z);
                IfcDirection axis_start = new IfcDirection(ifc_db, 0d, 0d, 0d);
                IfcDirection axis_end = new IfcDirection(ifc_db, one_pipe.EndPoint.X - one_pipe.StartPoint.X,
                    one_pipe.EndPoint.Y - one_pipe.StartPoint.Y, one_pipe.EndPoint.Z - one_pipe.StartPoint.Z);


                IfcAxis2Placement3D axis_place = new IfcAxis2Placement3D(pipe_start, axis_start, axis_end);
                IfcToroidalSurface pipe_geometry = new IfcToroidalSurface(axis_place, one_pipe.Radius, one_pipe.Radius - one_pipe.WallThickness);
                IfcShapeRepresentation pipe_geometry2 = new IfcShapeRepresentation(pipe_geometry);
                IfcProductDefinitionShape pipe_geometry3 = new IfcProductDefinitionShape(pipe_geometry2);

                

                IfcDistributionSystem ifc_pipe1 = new IfcDistributionSystem(pipe_geometry3, one_pipe.Name, IfcDistributionSystemEnum.FUEL) ;
                IfcPipeSegment ifc_pipe2 = new IfcPipeSegment(parent_pipe_network, ifc_site.ObjectPlacement, )
            }
        }
    }
}
