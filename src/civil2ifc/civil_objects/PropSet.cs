using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Aec.PropertyData.DatabaseServices;

using cas = Autodesk.Civil.ApplicationServices;
using cds = Autodesk.Civil.DatabaseServices;
using GeometryGym.Ifc;

using static civil2ifc.Start;

namespace civil2ifc.civil_objects
{
    /// <summary>
    /// Property sets of civil's objects
    /// </summary>
    public class PropSet
    {
        private ObjectId model_object_id;
        private IfcObject current_object;
        public PropSet (ObjectId model_object_id, IfcObject obj)
        {
            current_object = obj;
            this.model_object_id = model_object_id;
            GetPropsByObject();

        }
        private void GetPropsByObject()
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
                        int y = 9;
                    }
                    acTrans.Commit();
                }
            }
            new ifc.IfcProps(props2name, this.current_object);
        }
    }
}
