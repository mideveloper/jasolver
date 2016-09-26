using System.Collections.Generic;

namespace Jasolver
{
    public class JsonApiSetting
    {

        public void AddDtoAssembliesNamespaceGroupWise(string groupName, string assemblyName, string dtoNamespace)
        {
            Resolver.AddDtoAssembliesNamespaceGroupWise(groupName, assemblyName, dtoNamespace);
        }
    }
    public class JsonApi
    {
        
        public static void Register(){
           Resolver.LoadAssemblies();
        }
        
        public static T Decode<T>(string rawJson, string groupName) where T : class{
            return (T)new Reader().ReadFromJson(rawJson, groupName);
        }

        public static dynamic Decode(string rawJson, string groupName) 
        {
            return new Reader().ReadFromJson(rawJson, groupName);
        }
        public static dynamic Encode<T>(T obj, string[] includes = null, Dictionary<string,string[]> fields = null, dynamic meta = null) where T : class{
            return new Writer().CreateJsonFromObject(obj, includes, fields, meta);
        }
        public static dynamic Encode(dynamic obj,string[] includes = null, Dictionary<string, string[]> fields = null, dynamic meta = null)
        {
            return new Writer().CreateJsonFromObject(obj, includes, fields, meta);
        }

        private static JsonApiSetting _settings = null;
        public static JsonApiSetting Settings { get {
                if(_settings == null)
                {
                    _settings = new JsonApiSetting();
                }
                return _settings;
        } }
    }
}
