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



namespace civil2ifc.ifc
{
    public class IfcProps
    {
        private IfcObject to_set;
        private Dictionary<string, Dictionary<string, object>> prop2name;
        public IfcProps(Dictionary<string, Dictionary<string, object>> prop2name_input, IfcObject to_set_input)
        {
            this.prop2name = prop2name_input;
            this.to_set = to_set_input;
            SetPropsToObject();
        }
        private void SetPropsToObject()
        {
            foreach (KeyValuePair<string, Dictionary<string, object>> one_props_group in this.prop2name)
            {
                List<IfcPropertySingleValue> props = new List<IfcPropertySingleValue>();
                foreach (KeyValuePair<string, object> properties2name in one_props_group.Value)
                {
                    switch (properties2name.Value.GetType().Name)
                    {
                        case "Double":
                            props.Add(new IfcPropertySingleValue(ifc_db, properties2name.Key, (double)properties2name.Value));
                            break;
                        case "Int32":
                            props.Add(new IfcPropertySingleValue(ifc_db, properties2name.Key, (int)properties2name.Value));
                            break;
                        case "Boolean":
                            props.Add(new IfcPropertySingleValue(ifc_db, properties2name.Key, (bool)properties2name.Value));
                            break;
                        default:
                            props.Add(new IfcPropertySingleValue(ifc_db, properties2name.Key, properties2name.Value.ToString()));
                            break;

                    }
                }
                IfcPropertySet new_set = new IfcPropertySet(to_set, one_props_group.Key, props);
            }
        }
    }
}
