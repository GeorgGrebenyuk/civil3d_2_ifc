using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

using cas = Autodesk.Civil.ApplicationServices;
using cds = Autodesk.Civil.DatabaseServices;
using GeometryGym.Ifc;






namespace civil2ifc
{
    
    public class Start
    {
        public static DatabaseIfc ifc_db;
        public static IfcProject ifc_project;
        public static IfcSite ifc_site;
        public static IfcLocalPlacement base_placement;


        public static Document ac_doc;
        public static cas.CivilDocument civil_doc;
        public static Database ac_db;

        private enum civil_obj_type
        {
            Alignment = 0,
            Points = 1,
            PipeNetwork = 2,
            Surface = 3
        }
        [CommandMethod("_ifc_export")]
        public void ConvertToIfc()
        {
            //Init application
            ac_doc = Application.DocumentManager.MdiActiveDocument;
            ac_db = ac_doc.Database;
            civil_doc = cas.CivilApplication.ActiveDocument;

            ifc_db = new DatabaseIfc(ModelView.Ifc2x3NotAssigned); //ModelView.Ifc4X1NotAssigned
            ifc_site = new IfcSite(ifc_db, "civil_site");
            
            ifc_project = new IfcProject(ifc_site, "IfcProject", IfcUnitAssignment.Length.Metre) { };

            //Base placement for position other elements
            IfcCartesianPoint p0 = new IfcCartesianPoint(ifc_db, 0d, 0d, 0d);
            IfcDirection d1 = new IfcDirection(ifc_db, 0d, 0d, 1d);
            IfcDirection d2 = new IfcDirection(ifc_db, 1d, 0d, 0d);
            IfcAxis2Placement3D p2 = new IfcAxis2Placement3D(p0, d1, d2);
            base_placement = new IfcLocalPlacement(p2);

            //Start parsing file
            //Surfaces
            civil_objects.Surface.Create(civil_doc.GetSurfaceIds());
            civil_objects.PipeNetwork.Create(civil_doc.GetPipeNetworkIds());

            string path_to_ifc_file = ac_db.Filename.Replace(Path.GetExtension(ac_db.Filename), $"{Guid.NewGuid()}.ifc");
            
            ifc_db.WriteFile(path_to_ifc_file);
        }
    }
}
