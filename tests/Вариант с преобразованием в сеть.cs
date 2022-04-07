public IfcPolygonalFaceSet surf_row2;
        //public IfcFaceBasedSurfaceModel surf_row;
        public GetSolid(ObjectId input_solid_id)
        {
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    Solid3d input_solid = acTrans.GetObject(input_solid_id, OpenMode.ForRead) as Solid3d;

                    // Вычислим приблизительный размер нашего тела
                    double length = input_solid.GeometricExtents.MinPoint.GetVectorTo
                                    (input_solid.GeometricExtents.MaxPoint).Length;

                    List<Tuple<double, double, double>> points_temp = new List<Tuple<double, double, double>>();
                    List<IfcIndexedPolygonalFace> faces_indexed = new List<IfcIndexedPolygonalFace>();
                    List<Point3d> face_points = new List<Point3d>();

                    using (Brep brp = new Brep(input_solid))
                    {
                        // Создадим и установим параметры элемента управления сетью
                        using (Mesh2dControl mc = new Mesh2dControl())
                        {
                            mc.MaxNodeSpacing = length / 10;
                            mc.MaxSubdivisions = 100;
                            // Создадим элемент фильтра сети
                            using (Mesh2dFilter mf = new Mesh2dFilter())
                            {
                                // Используем его для отображения настроек управления
                                // в Brep
                                mf.Insert(brp, mc);
                                // Генерируем сеть при помощи фильтра
                                PolyFaceMesh pfm = new PolyFaceMesh();
                                pfm.SetDatabaseDefaults();
                                pfm.ColorIndex = 2;
                                //btr.AppendEntity(pfm);
                                short v0 = 0;
                                short v1 = 0;
                                short v2 = 0;
                                short v3 = 0;
                                short v = 0;
                                Dictionary<Point3d, short> vertexLookup
                                        = new Dictionary<Point3d, short>();
                                using (Mesh2d m = new Mesh2d(mf))
                                {
                                    // Получаем отдельные грани
                                    // из данных сети
                                    foreach (Element2d e in m.Element2ds)
                                    {
                                        foreach (Node n in e.Nodes)
                                        {
                                            if (!vertexLookup.ContainsKey(n.Point))
                                            {
                                                PolyFaceMeshVertex pfmVertex
                                                    = new PolyFaceMeshVertex(n.Point);
                                                pfm.AppendVertex(pfmVertex);
                                                vertexLookup.Add(n.Point, ++v);
                                                acTrans.AddNewlyCreatedDBObject
                                                        (pfmVertex, true);
                                            }
                                            n.Dispose();
                                        }
                                    }
                                    foreach (Element2d e in m.Element2ds)
                                    {
                                        Point3dCollection pts
                                                = new Point3dCollection();
                                        foreach (Node n in e.Nodes)
                                        {
                                            pts.Add(n.Point);
                                            n.Dispose();
                                        }
                                        e.Dispose();
                                        v0 = 0;
                                        if (vertexLookup.ContainsKey(pts[0]))
                                            v0 = vertexLookup[pts[0]];
                                        v1 = 0;
                                        if (vertexLookup.ContainsKey(pts[1]))
                                            v1 = vertexLookup[pts[1]];
                                        v2 = 0;
                                        if (vertexLookup.ContainsKey(pts[2]))
                                            v2 = vertexLookup[pts[2]];
                                        v3 = 0;
                                        if (pts.Count == 4)
                                        {
                                            if (vertexLookup.ContainsKey(pts[3]))
                                                v3 = vertexLookup[pts[3]];
                                        }
                                        FaceRecord fr
                                           = new FaceRecord(v0, v1, v2, v3);

                                        List<int> coord_indexes = new List<int>();
                                        add_point(v0);
                                        add_point(v1);
                                        add_point(v2);
                                        add_point(v3);

                                        
                                        void add_point(short p_index)
                                        {
                                            Point3d p = vertexLookup.Where(a => a.Value == p_index).First().Key;
                                            points_temp.Add(new Tuple<double, double, double>(p.X, p.Y, p.Z));
                                            coord_indexes.Add(p_index);


                                        }
                                        faces_indexed.Add(new IfcIndexedPolygonalFace(ifc_db, coord_indexes));
                                        pfm.AppendFaceRecord(fr);
                                        acTrans.AddNewlyCreatedDBObject(fr, true);
                                    }
                                }
                                acTrans.AddNewlyCreatedDBObject(pfm, true);
                                vertexLookup.Clear();
                            }
                        }
                    }

                    IfcCartesianPointList3D collection = new IfcCartesianPointList3D(ifc_db, points_temp);
                    this.surf_row2 = new IfcPolygonalFaceSet(collection, faces_indexed);
                    acTrans.Commit();
                }
            }
            
        }