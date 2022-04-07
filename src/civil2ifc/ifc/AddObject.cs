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

namespace civil2ifc.ifc
{
    /// <summary>
    /// Class for combine appreaches (converting to ifc geometry, setting color/material, creating ifc class for every civil object
    /// </summary>
    public class AddObject
    {
        private IfcObject ifc_parent_object;
        private ObjectId model_object_id;
        private ObjectId object_layer_assotiated_id;
        private IfcRepresentationItem to_set_style;
        private IfcProductDefinitionShape result_geometry;
        private IfcProduct result_ifc_element;
        private Dictionary<string, Dictionary<string, object>> model_object_internal_properties;
        public AddObject(List<IfcFace> obj_geometry, IfcObject parent_element, ObjectId obj_id, ObjectId layer_id, 
            Dictionary<string, Dictionary<string,object>> aux_props = null)
        {
            IfcFaceBasedSurfaceModel ifc_surf_model = new IfcFaceBasedSurfaceModel(new IfcConnectedFaceSet(obj_geometry));
            this.to_set_style = ifc_surf_model;
           
            IfcShapeRepresentation surface_repr = new IfcShapeRepresentation(ifc_surf_model);
            this.result_geometry = new IfcProductDefinitionShape(surface_repr);

            this.model_object_internal_properties = aux_props;
            this.ifc_parent_object = parent_element;
            this.model_object_id = obj_id;
            this.object_layer_assotiated_id = layer_id;
            this.StartWork();
        }
        public AddObject(IfcPoint obj_geometry, IfcObject parent_element, ObjectId obj_id, ObjectId layer_id,
    Dictionary<string, Dictionary<string, object>> aux_props = null)
        {
            this.to_set_style = obj_geometry;

            IfcShapeRepresentation surface_repr = new IfcShapeRepresentation(obj_geometry);
            this.result_geometry = new IfcProductDefinitionShape(surface_repr);

            this.model_object_internal_properties = aux_props;
            this.ifc_parent_object = parent_element;
            this.model_object_id = obj_id;
            this.object_layer_assotiated_id = layer_id;
            this.StartWork();
        }

        private void StartWork()
        {
            IfcBuildingElementProxy ifc_element = new IfcBuildingElementProxy(this.ifc_parent_object, ifc_site.ObjectPlacement, this.result_geometry);
            ifc_element.Representation = this.result_geometry;
            this.result_ifc_element = ifc_element;
            GetCivilPropertySets();
            SetColor();

        }
        private void SetColor()
        {
            Autodesk.AutoCAD.Colors.Color color_cad = layer2color[this.object_layer_assotiated_id];


            IfcColourRgb ifc_color = new IfcColourRgb(ifc_db, color_cad.ColorValue.R/256d, color_cad.ColorValue.G / 256d, color_cad.ColorValue.B / 256d);
            IfcSurfaceStyleShading ifc_style = new IfcSurfaceStyleShading(ifc_color);
            IfcSurfaceStyle ifc_style0 = new IfcSurfaceStyle(ifc_style);
            IfcPresentationStyleAssignment style_assignm = new IfcPresentationStyleAssignment(ifc_style0);
            IfcStyledItem object_style = new IfcStyledItem(this.to_set_style, style_assignm);


        }
        private void GetCivilPropertySets()
        {
            Dictionary<string, Dictionary<string, object>> props2name = new Dictionary<string, Dictionary<string, object>>();
            using (DocumentLock acDocLock = ac_doc.LockDocument())
            {
                using (Transaction acTrans = ac_db.TransactionManager.StartTransaction())
                {
                    DBObject model_object = acTrans.GetObject(model_object_id, OpenMode.ForRead) as DBObject;
                    ObjectIdCollection object_props_all = PropertyDataServices.GetPropertySets(model_object);

                    foreach (ObjectId object_props_id in object_props_all)
                    {
                        Dictionary<string, object> prop_group = new Dictionary<string, object>();
                        PropertySet object_props = acTrans.GetObject(object_props_id, OpenMode.ForWrite, false) as PropertySet;



                        ObjectId object_props_def_id = object_props.PropertySetDefinition;
                        PropertySetDefinition object_props_def = (PropertySetDefinition)acTrans.GetObject(object_props_def_id, OpenMode.ForWrite);
                        string prop_group_name = object_props_def.AlternateName;

                        PropertyDefinitionCollection propDefColl = object_props_def.Definitions;
                        //PropertySetDataCollection psetDataColl = object_props.PropertySetData;

                        foreach (PropertyDefinition propDef in propDefColl)
                        {
                            object prop_value = object_props.GetAt(propDef.Id);
                            switch (propDef.DataType)
                            {
                                case Autodesk.Aec.PropertyData.DataType.Integer:
                                    prop_group.Add(propDef.Name, (int)prop_value);
                                    break;
                                case Autodesk.Aec.PropertyData.DataType.Real:
                                    prop_group.Add(propDef.Name, (double)prop_value);
                                    break;
                                case Autodesk.Aec.PropertyData.DataType.TrueFalse:
                                    prop_group.Add(propDef.Name, (bool)prop_value);
                                    break;
                                default:
                                    if (prop_value != null) prop_group.Add(propDef.Name, prop_value.ToString());
                                    else prop_group.Add(propDef.Name, "");
                                    break;
                            }
                        }
                        props2name.Add(prop_group_name, prop_group);
                    }
                    acTrans.Commit();
                }
            }
            new ifc.IfcProps(props2name, this.result_ifc_element);
        }



    }
}
