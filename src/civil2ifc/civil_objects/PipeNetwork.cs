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
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(ac_db.BlockTableId,
                                                       OpenMode.ForRead) as BlockTable;
                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                          OpenMode.ForWrite) as BlockTableRecord;


                    foreach (ObjectId id in ids)
                    {
                        cds.Network pipes_network = acTrans.GetObject(id, OpenMode.ForRead) as cds.Network;
                        string pipe_network_name = pipes_network.Name;
                        IfcSystem pipe_network_system = new IfcSystem(ifc_db, pipe_network_name);
                        
                        parent_pipe_network = pipe_network_system;
                        

                        //Workings with pipes and structures
                        ObjectIdCollection pipes_ids = pipes_network.GetPipeIds();
                        pipes = new List<cds.Pipe>();
                        foreach (ObjectId pipe_id in pipes_ids)
                        {
                            cds.Pipe new_p = acTrans.GetObject(pipe_id, OpenMode.ForRead) as cds.Pipe;
                            //var class_fields = new_p.GetType().GetFields();
                            pipes.Add(new_p);
                        }
                        
                        ObjectIdCollection structures_ids = pipes_network.GetStructureIds();
                        structures = new List<cds.Structure>();
                        foreach (ObjectId pipe_id in structures_ids)
                        {
                            cds.Structure new_s = acTrans.GetObject(pipe_id, OpenMode.ForRead) as cds.Structure;
                            
                            if (!new_s.Description.Contains("улевой колод")) structures.Add(new_s);

                        }

                        WorksWithPipes();
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
                var pipe_solid = new ifc.GetSolid(one_pipe.Solid3dBody).surf_row;

                IfcStyledItem object_style = new IfcStyledItem(pipe_solid, new civil_properties.SetMaterial(one_pipe.LayerId).style_assignm);
                IfcShapeRepresentation solid_shape_pipe = new IfcShapeRepresentation(pipe_solid);
                IfcProductDefinitionShape solid_shape2_pipe = new IfcProductDefinitionShape(solid_shape_pipe);

                //IfcDistributionSystem pipe_system = new IfcDistributionSystem(, one_pipe.Name, IfcDistributionSystemEnum.FUEL);
                //IfcPipeSegment pipe_item = new IfcPipeSegment(ifc_site, base_placement, solid_shape2_pipe, pipe_system);
                IfcBuildingElementProxy pipe_item = new IfcBuildingElementProxy(parent_pipe_network, base_placement, solid_shape2_pipe);
                pipe_item.Representation = solid_shape2_pipe;



                new PropSet(one_pipe.Id, pipe_item); //new_site.GlobalId

                Dictionary<string, object> pipes_properties = new Dictionary<string, object>
                {
                    {"Radius",one_pipe.Radius },
                    {"WallThickness",one_pipe.WallThickness },
                    {"Name",one_pipe.Name },
                    {"Description",one_pipe.Description },
                    //{"Length3D",one_pipe.Length3D },
                   // {"PartFamilyName",one_pipe.PartFamilyName },
                    //{"PartSizeName",one_pipe.PartSizeName }
                };
                Dictionary<string, Dictionary<string, object>> internal_surf_props = new Dictionary<string, Dictionary<string, object>>();
                internal_surf_props.Add("Pipes properties", pipes_properties);
                new ifc.IfcProps(internal_surf_props, pipe_item);
            }
        }
        private static void WorksWithStructures()
        {
            foreach (cds.Structure one_structure in structures)
            {
                //cds.Structure one_structure = one_structure_record.Key;
                //Geometry
                var structure_solid = new ifc.GetSolid(one_structure.Solid3dBody).surf_row;

                IfcStyledItem object_style = new IfcStyledItem(structure_solid, new civil_properties.SetMaterial(one_structure.LayerId).style_assignm);
                IfcShapeRepresentation solid_shape_pipe = new IfcShapeRepresentation(structure_solid);
                IfcProductDefinitionShape solid_shape2_pipe = new IfcProductDefinitionShape(solid_shape_pipe);

                IfcBuildingElementProxy proxy_solid = new IfcBuildingElementProxy(parent_pipe_network, base_placement, solid_shape2_pipe);
                proxy_solid.Representation = solid_shape2_pipe;


                new PropSet(one_structure.Id, proxy_solid); //new_site.GlobalId

                Dictionary<string, object> structure_properties = new Dictionary<string, object>
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
                    //{"PartFamilyId",one_structure.PartFamilyId },
                    //{"PartFamilyName",one_structure.PartFamilyName},
                    //{"PartSizeName",one_structure.PartSizeName },
                    //{"PartSubType",one_structure.PartSubType },
                    //{"PartType",one_structure.PartType },
                    //{"SumpDepth",one_structure.SumpDepth },
                    //{"SumpElevation",one_structure.SumpElevation }
                };
                Dictionary<string, Dictionary<string, object>> internal_surf_props = new Dictionary<string, Dictionary<string, object>>();
                //Dictionary<string, object> class_props = new civil_properties.ObjProps_uni(one_structure).props2name;
                internal_surf_props.Add("Pipes properties", structure_properties);
                new ifc.IfcProps(internal_surf_props, proxy_solid);
            }
        }
        private static void DeleteTempSolids()
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    acTrans.Commit();
                }
            }

        }
    }
}
