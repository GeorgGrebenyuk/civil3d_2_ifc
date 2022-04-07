public IfcPolygonalFaceSet surf_row2;
        //public IfcFaceBasedSurfaceModel surf_row;
        public GetSolid(ObjectId input_solid_id)
        {

            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    Solid3d input_solid = acTrans.GetObject(input_solid_id, OpenMode.ForRead) as Solid3d;

                    List<Tuple<double, double, double>> points_temp = new List<Tuple<double, double, double>>();
                    List<IfcIndexedPolygonalFace> faces_indexed = new List<IfcIndexedPolygonalFace>();
                    List<Point3d> face_points = new List<Point3d>();

                    Brep brp = new Brep(input_solid);
                    using (brp)
                    {
                        for (int complex_counter = 0; complex_counter < brp.Complexes.Count(); complex_counter++)
                        {
                            List<int> coord_indexes = new List<int>(); // КУДА ЭТО????
                            Complex cmp = brp.Complexes.ElementAt(complex_counter);
                            for (int shell_counter = 0; shell_counter < cmp.Shells.Count(); shell_counter++)
                            {

                                Shell shl = cmp.Shells.ElementAt(shell_counter);
                                for (int face_counter = 0; face_counter < shl.Faces.Count(); face_counter++)
                                {
                                    Autodesk.AutoCAD.BoundaryRepresentation.Face fce = shl.Faces.ElementAt(face_counter);

                                    for (int BoundaryLoop_counter = 0; BoundaryLoop_counter < fce.Loops.Count(); BoundaryLoop_counter++)
                                    {

                                        BoundaryLoop lp = fce.Loops.ElementAt(BoundaryLoop_counter);
                                        for (int edge_counter = 0; edge_counter < lp.Edges.Count(); edge_counter++)
                                        {
                                            Edge edg = lp.Edges.ElementAt(edge_counter);
                                            vertex_opeations(edg.Vertex1);
                                            vertex_opeations(edg.Vertex2);
                                            void vertex_opeations(Autodesk.AutoCAD.BoundaryRepresentation.Vertex v)
                                            {
                                                if (!face_points.Contains(v.Point))
                                                {
                                                    face_points.Add(v.Point);
                                                    points_temp.Add(new Tuple<double, double, double>(v.Point.X, v.Point.Y, v.Point.Z));
                                                    coord_indexes.Add(points_temp.Count() - 1);
                                                }
                                                else
                                                {
                                                    int point_last = face_points.FindIndex(a => a == v.Point);
                                                    coord_indexes.Add(point_last);
                                                }
                                            }
                                        }


                                    }

                                }

                            }
                            faces_indexed.Add(new IfcIndexedPolygonalFace(ifc_db, coord_indexes));
                        }
                    }

                    IfcCartesianPointList3D collection = new IfcCartesianPointList3D(ifc_db, points_temp);
                    this.surf_row2 = new IfcPolygonalFaceSet(collection, faces_indexed);
                    acTrans.Commit();
                }
            }
            
        }