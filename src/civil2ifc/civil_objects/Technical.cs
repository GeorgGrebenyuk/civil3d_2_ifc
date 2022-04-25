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
using Autodesk.AutoCAD.Colors;

namespace civil2ifc.civil_objects
{
    public class Technical
    {
        public static void CreateSpheresInIncorrectPlacesInModelSpace()
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    // Open the Block table record for read
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(ac_db.BlockTableId,
                                                 OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                    OpenMode.ForWrite) as BlockTableRecord;
                    LayerTable acLyrTbl;
                    acLyrTbl = acTrans.GetObject(ac_db.LayerTableId,
                                                 OpenMode.ForRead) as LayerTable;
                    string sLayerName = "Spheres_objects_layer";

                    if (acLyrTbl.Has(sLayerName) == false)
                    {
                        LayerTableRecord acLyrTblRec = new LayerTableRecord();

                        // Assign the layer the ACI color 1 and a name
                        acLyrTblRec.Color = Color.FromRgb(1,0,0);
                        acLyrTblRec.Name = sLayerName;

                        // Upgrade the Layer table for write
                        acLyrTbl.UpgradeOpen();

                        // Append the new layer to the Layer table and the transaction
                        acLyrTbl.Add(acLyrTblRec);
                        acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                        ac_db.Clayer = acLyrTbl[sLayerName];
                    }

                    foreach (Point3d center_point in temp_points_invalid_objects)
                    {
                        Sphere model_sphere = new Sphere(10, center_point);
                        
                    }
                    acTrans.Commit();
                }
            }
        }
    }
}
