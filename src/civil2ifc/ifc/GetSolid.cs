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
        //public IfcFaceBasedSurfaceModel surf_row;
        public GetSolid(ObjectId input_solid_id)
        {

            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    List<IfcFace> surf_faces = new List<IfcFace>();

                    string folder_path = $@"C:\Users\{Environment.UserName}\AppData\Local\Temp\DXFs";
                    if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);
                    string solid_path = $@"{folder_path}\{Guid.NewGuid()}.dxf";
                    Solid3d input_solid = acTrans.GetObject(input_solid_id, OpenMode.ForRead) as Solid3d;

                    using (var db = new Database(false, true))
                    {
                        // read the template dwg file
                        string tmp_path = @"C:\Users\Georg\AppData\Local\Autodesk\C3D 2022\enu\Template\acadiso.dwt";
                        db.ReadDwgFile(tmp_path, FileOpenMode.OpenForReadAndAllShare, false, null);

                        // start a transaction
                        using (var tr = db.TransactionManager.StartTransaction())
                        {
                            // check if a layer named "Character" already exists and create it if not
                            string layerName = "solid";
                            ObjectId layerId;
                            var layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                            if (layerTable.Has(layerName))
                            {
                                layerId = layerTable[layerName];
                            }
                            else
                            {
                                tr.GetObject(db.LayerTableId, OpenMode.ForWrite);
                                var layer = new LayerTableRecord
                                {
                                    Name = layerName,
                                };
                                layerId = layerTable.Add(layer);
                                tr.AddNewlyCreatedDBObject(layer, true);
                            }

                            // get the Model space
                            var modelSpace = (BlockTableRecord)tr.GetObject(
                                SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite);


                            modelSpace.AppendEntity(input_solid);
                            tr.AddNewlyCreatedDBObject(input_solid, true);

                            // save changes to the database
                            tr.Commit();
                        }

                        // save the new drawing
                        db.DxfOut(solid_path, 0, DwgVersion.AC1027);
                    }

                    
                    //File.Delete(solid_path);
                    this.surf_row = new IfcFaceBasedSurfaceModel(new IfcConnectedFaceSet(surf_faces));
                    acTrans.Commit();
                }
            }
            
        }
    }
}
