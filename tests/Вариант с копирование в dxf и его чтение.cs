public IfcFaceBasedSurfaceModel surf_row;
        //public IfcFaceBasedSurfaceModel surf_row;
        public GetSolid(ObjectId input_solid_id)
        {
            string tmp_path = @"C:\Users\Georg\AppData\Local\Autodesk\C3D 2022\enu\Template\acadiso.dwt";
            string folder_path = $@"C:\Users\{Environment.UserName}\AppData\Local\Temp\DXFs";
            if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);
            string solid_path = $@"{folder_path}\{Guid.NewGuid()}.dxf";



            using (var db = new Database(false, true))
            {
                // read the template dwg file
                db.ReadDwgFile(tmp_path, FileOpenMode.OpenForReadAndAllShare, false, null);

                // start a transaction
                using (var acTrans = db.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable acBlkTblNewDoc;
                    acBlkTblNewDoc = acTrans.GetObject(db.BlockTableId,
                                                       OpenMode.ForRead) as BlockTable;
                    // Open the Block table record Model space for read
                    BlockTableRecord acBlkTblRecNewDoc;
                    acBlkTblRecNewDoc = acTrans.GetObject(acBlkTblNewDoc[BlockTableRecord.ModelSpace],
                                                       OpenMode.ForRead) as BlockTableRecord;
                    // Clone the objects to the new database
                    IdMapping acIdMap = new IdMapping();
                    ac_db.WblockCloneObjects(new ObjectIdCollection() { input_solid_id }, acBlkTblRecNewDoc.ObjectId, acIdMap,
                                               DuplicateRecordCloning.Ignore, false);
                    // Save the copied objects to the database
                    acTrans.Commit();
                }

                // save the new drawing
                db.DxfOut(solid_path, 0, DwgVersion.AC1021);
            }

            dxf.DxfDocument dxf_doc = dxf.DxfDocument.Load(solid_path);
            dxf.Collections.EntityCollection dxf_entities = dxf_doc.Blocks[netDxf.Blocks.Block.DefaultModelSpaceName].Entities;
            foreach (dxf.Entities.EntityObject one_entity in dxf_entities)
            {
                var yee = one_entity.Type;
            }
            dxf.Entities.Solid single_solid = dxf_doc.Solids.First();
            int y = 9;



        }