using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Jasolver
{
    internal class Writer
    {
       
        private Dictionary<string, string[]> _fields = null;
        private string[] _includes = null;
        
        internal dynamic CreateJsonFromObject<T>(T obj, string[] includes = null, Dictionary<string,string[]> fields = null, dynamic meta = null) where T : class
        {

            _fields = fields;
            _includes = includes;
            dynamic dynamicObject = new ExpandoObject();
            dynamicObject.data = createJsonApi(obj);

            if (includes != null && includes.Length > 0 && included.Count > 0)
                dynamicObject.included = included;

            if (meta != null)
                getMeta(meta, dynamicObject);

            return dynamicObject;
        }

        private void getMeta<T>(T obj, dynamic finalObject) where T : class
        {
            Type type = obj.GetType();
            dynamic relationShipObjects = new ExpandoObject() as IDictionary<string, Object>;
            var meta = new ExpandoObject() as IDictionary<string, Object>;
            var typeName = Resolver.UnClassify(type.Name);
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
                setProperties<T>(obj, null, relationShipObjects, typeName, null, properties[i], meta);

            finalObject.meta = meta;
        }

        List<dynamic> included = new List<dynamic>();
        private dynamic createJsonApi<T>(T obj, string parentPropertyName = null) where T : class
        {
            if (obj is IList)
            {
                List<dynamic> objectCollection = new List<dynamic>();
                foreach (var item in (IList)obj)
                {

                    objectCollection.Add(getJsonApiObject(item, parentPropertyName));
                    
                }
                return objectCollection;
            }
            else
            {
                return getJsonApiObject(obj, parentPropertyName);
            }
        }

        private dynamic getJsonApiObject<T>(T obj, string parentPropertyName = null) where T : class
        {
            dynamic relationShipObjects = new ExpandoObject() as IDictionary<string, Object>;

            Type type = obj.GetType();

            dynamic finalObj = new ExpandoObject();

            PropertyInfo[] properties = type.GetProperties();
            var attributes = new ExpandoObject() as IDictionary<string, Object>;
            var typeName = Resolver.UnClassify(type.Name);
            for (int i = 0; i < properties.Length; i++)
                setProperties<T>(obj, parentPropertyName, relationShipObjects, typeName, finalObj, properties[i], attributes);

            
            finalObj.attributes = attributes;
            if (((IDictionary<string, Object>)relationShipObjects).Count > 0)
            {
                finalObj.relationships
                      = relationShipObjects;
            }
            return finalObj;
        }

        private void setProperties<T>(T obj, string parentPropertyName, dynamic relationShipObjects, string typeName,
            dynamic finalObj, PropertyInfo p, IDictionary<string, object> attributes) where T : class
        {
            var propertyName = Resolver.UnClassify(p.Name, true);
            if (isPrimitive(p.PropertyType))
            {
                
                if (propertyName == "id")
                {
                    finalObj.id = p.GetValue(obj).ToString();
                    finalObj.type = typeName;
                }
                else
                {
                    if (_fields != null && _fields.Count > 0)
                    {
                        if (_fields.ContainsKey(typeName) && _fields[typeName].Contains(propertyName))
                            attributes[propertyName] = p.GetValue(obj);
                    }
                    else
                    {
                        attributes[propertyName] = p.GetValue(obj);
                    }
                }
            }
            else
            {

                // if type is not primitive this means type is object therefore it will fit inside relationships
                if (_fields != null && _fields.Count > 0)
                {
                    if (_fields.ContainsKey(typeName) && _fields[typeName].Contains(propertyName))
                        addRelationShip(relationShipObjects, p, obj, parentPropertyName);
                }
                else
                {
                    addRelationShip(relationShipObjects, p, obj, parentPropertyName);
                }
            }
        }

        private void addRelationShip<T>(IDictionary<string, Object> relationShips, PropertyInfo p, T obj, string parentPropertyName = null ) where T : class
        {
            // check if property is ILIST that means this will contains array inside data property
            var relationShipObjectValue = p.GetValue(obj);

            if (relationShipObjectValue == null)
                return;

            Type relationShipObjectType = relationShipObjectValue.GetType();

            var propertyName = Resolver.UnClassify(p.Name, true);
            var includePropertyName = (string.IsNullOrEmpty(parentPropertyName)) ?
                        propertyName : (parentPropertyName + "." + propertyName);
            if (relationShipObjectValue is IList)
            {
                List<dynamic> arr = new List<dynamic>();
               
                foreach (var item in (IList)relationShipObjectValue)
                {
                    PropertyInfo IdProperty = item.GetType().GetProperty("Id");
                    if (IdProperty == null)
                    {
                        IdProperty = item.GetType().GetProperty(item.GetType().Name + "Id");
                    }
                    var id = IdProperty.GetValue(item);
                    var type = Resolver.UnClassify(item.GetType().Name);

                    arr.Add(new
                    {
                        id = id.ToString(),
                        type = type
                    }
                    );

                    if (_includes != null && _includes.Contains(includePropertyName) 
                            && included.Where(x => x.id == id && x.type == type).Count() == 0)
                    {
                        var includeObject = createJsonApi(item, propertyName);
                        includeObject.id = id.ToString();
                        includeObject.type = type;
                        included.Add(includeObject);
                    }
                    
                }
                relationShips.Add(propertyName, new { data = arr });
            }
            else
            {
                PropertyInfo IdProperty = relationShipObjectType.GetProperty("Id");
                if (IdProperty == null)
                {
                    IdProperty = relationShipObjectType.GetProperty(relationShipObjectType.Name + "Id");
                }
                var id = IdProperty.GetValue(relationShipObjectValue);

                var type = Resolver.UnClassify(relationShipObjectType.Name);

                relationShips.Add(Resolver.UnClassify(p.Name, true), new { data = new {
                    id = id.ToString(),
                    type = type
                }
                });
                // check If it is required to Include
                if(_includes != null && _includes.Contains(includePropertyName) &&
                    !included.Where(x => x.id == id.ToString() && x.type == type).Any())
                {
                    var includeObject = createJsonApi(relationShipObjectValue, propertyName);
                    includeObject.id = id.ToString();
                    includeObject.type = type;
                    included.Add(includeObject);
                }
            }
        }

        private bool isPrimitive (Type type)
        {
            return (type.IsValueType || type == typeof(string));
        }
    }
}