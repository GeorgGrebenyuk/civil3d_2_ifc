        public IfcFaceBasedSurfaceModel surf_row;
        public GetSolid(ObjectId input_solid_id)
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    List<IfcFace> surf_faces = new List<IfcFace>();

                    string folder_path = $@"C:\Users\{Environment.UserName}\AppData\Local\Temp\STLs";
                    if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);
                    string solid_path = $@"{folder_path}\{Guid.NewGuid()}.stl";
                    Solid3d input_solid = acTrans.GetObject(input_solid_id, OpenMode.ForRead) as Solid3d;
                    
                    input_solid.StlOut(solid_path, true);
                    stl.STLDocument solid_stl = stl.STLDocument.Open(solid_path);
                    List<stl.Facet> stl_facets = solid_stl.Facets.ToList();
                    foreach (stl.Facet stl_one_facet in stl_facets)
                    {
                        surf_faces.Add(new ifc.BaseStructures(stl_one_facet).face);
                    }
                    //File.Delete(solid_path);
                    this.surf_row = new IfcFaceBasedSurfaceModel(new IfcConnectedFaceSet(surf_faces));
                    acTrans.Commit();
                }
            }
            
        }