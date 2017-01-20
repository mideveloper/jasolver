﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Script.Serialization;

namespace Jasolver
{
    internal class Reader
    {
        internal dynamic ReadFromJson(string rawJson, string groupName)
        {
            var jsonObj = JsonConvert.DeserializeObject<dynamic>(rawJson);

            var data = jsonObj.data;

            if (data != null)
            {
                if (data is JArray )
                {
                    if(data.Count == 0)
                    {
                        return  null;
                    }
                    
                    int i = 0;
                    
                    // getting first member only to get the type so collection can be created.
                    var obj = Resolver.GetEnityObject(data[i].type.ToString(),groupName);
                    IList objectCollection =  (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(obj.GetType()));
                    objectCollection.Add(prepareObject(obj, jsonObj.data[i], jsonObj.included));

                    for (i = 1; i < data.Count; i++)
                    {
                        obj = Resolver.GetEnityObject(data[i].type.ToString(), groupName);
                        objectCollection.Add(prepareObject(obj, jsonObj.data[i], jsonObj.included));
                    }

                    return objectCollection;
                }
                else
                {
                    var obj = Resolver.GetEnityObject(data.type.ToString(), groupName);

                    prepareObject(obj, jsonObj.data, jsonObj.included);
                    return obj;
                }
            }

            return null;
        }

        private dynamic prepareObject(dynamic obj, dynamic data, dynamic included) {
            Type targetType = obj.GetType();
            PropertyInfo p = targetType.GetProperty("Id");
            if (p != null)
            {
                var value = ChangeType(data.id, p.PropertyType);
                if(value != null)
                    p.SetValue(obj, value);
            }

            setAttributesToPropertiesOfObject(obj, data.attributes);
            setRelationShipToPropertiesOfObject(obj, data.relationships, included);
            return obj;
        }

        
        private void setAttributesToPropertiesOfObject(dynamic obj, dynamic attributes)
        {
            if (attributes == null)
                return;

            Type targetType = obj.GetType();

            foreach (var item in attributes)
            {
                PropertyInfo prop = targetType.GetProperty(Resolver.Classify(item.Name, true));
                if (prop != null)
                {
                    var value = ChangeType(item.Value, prop.PropertyType);
                    if(value != null)
                        prop.SetValue(obj, value);
                }
            }
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (JValue.CreateNull().Equals(value))
                {
                    return null;
                }
                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        private dynamic findObjectInIncluded(dynamic included, string id, string type)
        {
            if (included != null && included is JArray)
            {
                for (int i = 0; i < included.Count; i++)
                {
                    if (included[i].id.ToString() == id && included[i].type.ToString() == type)
                    {
                        return included[i];
                    } 
                }
            }
            return null;
        }

        private void setRelationShipToPropertiesOfObject(dynamic obj, dynamic relationShips, dynamic included) 
        {
            Type targetType = obj.GetType();

            if (relationShips != null)
            {
                foreach (var item in relationShips)
                {
                    PropertyInfo prop = targetType.GetProperty(Resolver.Classify(item.Name, true));
                    if (prop != null)
                    {
                        // if relationships are multiple for e.g. relationships : { id : 1, type: "articles", data:[]}
                        if (item.Value.data is JArray)
                        {
                            
                            IList relationShipObj = (IList)Activator.CreateInstance(prop.PropertyType);
                            
                            // since data is array therefore it is must that relationShipObj is IEnumerable
                            // therefore to create underlying objects we need to findout IEnumerable underlying type
                            // create object from it and set complete object if it exists in include
                            // other wise just set the id and return

                            for (int i = 0; i < item.Value.data.Count; i++)
                            {
                                // creating object of Ienumerable underlying type.
                                var underlyingRelationShipObj = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);
                                var includeObj = findObjectInIncluded(included, item.Value.data[i].id.ToString(), item.Value.data[i].type.ToString());

                                if (includeObj != null)
                                {
                                    relationShipObj.Add(prepareObject(underlyingRelationShipObj, includeObj, included));
                                }
                                else
                                {
                                    var propertyType = underlyingRelationShipObj.GetType();
                                    var idProperty = propertyType.GetProperty("Id");
                                    if (idProperty == null)
                                    {
                                        idProperty = propertyType.GetProperty(propertyType.Name + "Id");
                                    }
                                    var value = ChangeType(item.Value.data[i].id.ToString(), idProperty.PropertyType);
                                    if (value != null)
                                        idProperty.SetValue(underlyingRelationShipObj, value);
                                    relationShipObj.Add(underlyingRelationShipObj);
                                }

                            }
                            prop.SetValue(obj, relationShipObj);
                        }
                        else
                        {
                            var relationShipObj = Activator.CreateInstance(prop.PropertyType);
                            var includeObj = findObjectInIncluded(included, item.Value.data.id.ToString(), item.Value.data.type.ToString());

                            if (includeObj != null)
                            {
                                prop.SetValue(obj, prepareObject(relationShipObj, includeObj, included));
                            }
                            else
                            {
                                var idProperty = prop.PropertyType.GetProperty("Id");
                                if (idProperty == null)
                                {
                                    idProperty = prop.PropertyType.GetProperty(prop.PropertyType.Name + "Id");
                                }
                                var value = ChangeType(item.Value.data.id.ToString(), idProperty.PropertyType);
                                if (value != null)
                                    idProperty.SetValue(relationShipObj, value);

                                prop.SetValue(obj, relationShipObj);
                            }
                        }
                    }
                }
            }
        }
    }
}
