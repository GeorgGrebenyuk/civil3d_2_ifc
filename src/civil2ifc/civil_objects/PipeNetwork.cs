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
        private static List<cds.Structure> structures;

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
                        IfcSystem pipe_network_system = new IfcSystem(ifc_site, pipe_network_name);
                        
                        parent_pipe_network = pipe_network_system;
                        

                        //Workings with pipes and structures
                        ObjectIdCollection pipes_ids = pipes_network.GetPipeIds();
                        pipes = new List<cds.Pipe>();
                        foreach (ObjectId pipe_id in pipes_ids)
                        {
                            pipes.Add(acTrans.GetObject(pipe_id, OpenMode.ForRead) as cds.Pipe);
                        }
                        WorksWithPipes();

                        ObjectIdCollection structures_ids = pipes_network.GetStructureIds();
                        structures = new List<cds.Structure>();
                        foreach (ObjectId pipe_id in structures_ids)
                        {
                            structures.Add(acTrans.GetObject(pipe_id, OpenMode.ForRead) as cds.Structure);
                        }
                        WorksWithStructures();
                    }
                    acTrans.Commit();
                }
            }
        }
        private static void WorksWithPipes()
        {
            foreach (cds.Pipe one_pipe in pipes)
            {
                //Geometry
                IfcFaceBasedSurfaceModel pipe_solid = new ifc.GetSolid(one_pipe.Solid3dBody).surf_row;

                //IfcCartesianPoint pipe_start = new IfcCartesianPoint(ifc_db, one_pipe.StartPoint.X,
                //    one_pipe.StartPoint.Y, one_pipe.StartPoint.Z);
                //IfcCartesianPoint pipe_end = new IfcCartesianPoint(ifc_db, one_pipe.EndPoint.X,
                //    one_pipe.EndPoint.Y, one_pipe.EndPoint.Z);
                //IfcPolyline line_pipe = new IfcPolyline(pipe_start, pipe_end);
                //IfcCompositeCurveSegment curve_pipe = new IfcCompositeCurveSegment(IfcTransitionCode.CONTINUOUS, true, line_pipe);
                //IfcCompositeCurve curve_composit_pipe = new IfcCompositeCurve(curve_pipe);
                //IfcSweptDiskSolid solid_pipe = new IfcSweptDiskSolid(curve_composit_pipe, 
                //    one_pipe.InnerDiameterOrWidth / 2, one_pipe.InnerDiameterOrWidth / 2 - one_pipe.WallThickness);

                IfcStyledItem object_style = new IfcStyledItem(pipe_solid, new civil_properties.SetMaterial(one_pipe.Color).style_assignm);
                IfcShapeRepresentation solid_shape_pipe = new IfcShapeRepresentation(pipe_solid);
                IfcProductDefinitionShape solid_shape2_pipe = new IfcProductDefinitionShape(solid_shape_pipe);
                
                IfcBuildingElementProxy proxy_solid = new IfcBuildingElementProxy(ifc_site, base_placement, solid_shape2_pipe);
                proxy_solid.Representation = solid_shape2_pipe;
                

                new PropSet(one_pipe.Id, proxy_solid); //new_site.GlobalId

                Dictionary<string, object> pipes_properties = new Dictionary<string, object>
                {
                    {"Radius",one_pipe.Radius },
                    {"WallThickness",one_pipe.WallThickness },
                    {"Length3D",one_pipe.Length3D },
                    {"PartFamilyName",one_pipe.PartFamilyName },
                    {"PartSizeName",one_pipe.PartSizeName }
                };
                Dictionary<string, Dictionary<string, object>> internal_surf_props = new Dictionary<string, Dictionary<string, object>>();
                internal_surf_props.Add("Pipes properties", pipes_properties);
                new ifc.IfcProps(internal_surf_props, proxy_solid);
            }
        }
        private static void WorksWithStructures()
        {
            foreach (cds.Structure one_structure in structures)
            {
                //Geometry
                IfcFaceBasedSurfaceModel structure_solid = new ifc.GetSolid(one_structure.Solid3dBody).surf_row;

                //IfcCartesianPoint pipe_start = new IfcCartesianPoint(ifc_db, one_structure.StartPoint.X,
                //    one_structure.StartPoint.Y, one_structure.StartPoint.Z);
                //IfcCartesianPoint pipe_end = new IfcCartesianPoint(ifc_db, one_structure.EndPoint.X,
                //    one_structure.EndPoint.Y, one_structure.EndPoint.Z);
                //IfcPolyline line_pipe = new IfcPolyline(pipe_start, pipe_end);
                //IfcCompositeCurveSegment curve_pipe = new IfcCompositeCurveSegment(IfcTransitionCode.CONTINUOUS, true, line_pipe);
                //IfcCompositeCurve curve_composit_pipe = new IfcCompositeCurve(curve_pipe);
                //IfcSweptDiskSolid solid_pipe = new IfcSweptDiskSolid(curve_composit_pipe,
                //    one_structure.InnerDiameterOrWidth / 2, one_structure.InnerDiameterOrWidth / 2 - one_structure.WallThickness);
                IfcStyledItem object_style = new IfcStyledItem(structure_solid, new civil_properties.SetMaterial(one_structure.Color).style_assignm);
                IfcShapeRepresentation solid_shape_pipe = new IfcShapeRepresentation(structure_solid);
                IfcProductDefinitionShape solid_shape2_pipe = new IfcProductDefinitionShape(solid_shape_pipe);

                IfcBuildingElementProxy proxy_solid = new IfcBuildingElementProxy(ifc_site, base_placement, solid_shape2_pipe);
                proxy_solid.Representation = solid_shape2_pipe;


                new PropSet(one_structure.Id, proxy_solid); //new_site.GlobalId

                Dictionary<string, object> pipes_properties = new Dictionary<string, object>
                {
                    {"Description",one_structure.Description },
                    //{"DiameterOrWidth",one_structure.DiameterOrWidth },
                    //{"DisplayName",one_structure.DisplayName },
                    //{"FloorThickness",one_structure.FloorThickness },
                    //{"FrameDiameter",one_structure.FrameDiameter },
                    //{"FrameHeight",one_structure.FrameHeight },
                    //{"Height",one_structure.Height },
                    //{"InnerDiameterOrWidth",one_structure.InnerDiameterOrWidth },
                    //{"InnerLength ",one_structure.InnerLength },
                    {"PartFamilyId",one_structure.PartFamilyId },
                    {"PartFamilyName",one_structure.PartFamilyName},
                    //{"PartSizeName",one_structure.PartSizeName },
                    //{"PartSubType",one_structure.PartSubType },
                    //{"PartType",one_structure.PartType },
                    //{"SumpDepth",one_structure.SumpDepth },
                    //{"SumpElevation",one_structure.SumpElevation }
                };
                Dictionary<string, Dictionary<string, object>> internal_surf_props = new Dictionary<string, Dictionary<string, object>>();
                internal_surf_props.Add("Pipes properties", pipes_properties);
                new ifc.IfcProps(internal_surf_props, proxy_solid);
            }
        }
    }
}
