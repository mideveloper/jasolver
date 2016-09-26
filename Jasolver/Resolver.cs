using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;

namespace Jasolver
{
    
    internal class Resolver
    {
        private static Dictionary<string, Type> _assemblies = new Dictionary<string, Type>();
        
        public static void LoadAssemblies(){

            if (_assemblies.Count > 0) return;

            var listOfAssemblies = _dtoAssembliesNameSpace.Select(x => x.Value.Item1).ToList();
            var listOfAllTypesToRegister = _dtoAssembliesNameSpace.Select(x => x.Value.Item2).ToList();
            IEnumerable<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies().Where(x => listOfAssemblies.Contains(x.FullName.Split(',')[0])).SelectMany(a =>
                                        a.GetTypes());

            foreach (var type in allTypes)
            {
                if (!_assemblies.ContainsKey(type.FullName) && listOfAllTypesToRegister.Where(x=> type.FullName.StartsWith(x)).Count() > 0)
                    _assemblies.Add(type.FullName, type);
            }

        }

        
        /// <summary>
        /// Tuple<string, string>
        /// item1 means DTO Entities assembly reference
        /// e.g ProjectName.DTO.Entities
        /// item2 means Main assembly referenec
        /// e.g ProjectName, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
        /// </summary>
        private static Dictionary<string, Tuple<string, string>> _dtoAssembliesNameSpace = new Dictionary<string, Tuple<string, string>>();
        public static void AddDtoAssembliesNamespaceGroupWise(string groupName, string mainAssemblyName, string dtoAssemblyNamespace) {
            if(!_dtoAssembliesNameSpace.ContainsKey(groupName))
                _dtoAssembliesNameSpace.Add(groupName, new Tuple<string, string>(mainAssemblyName, dtoAssemblyNamespace));
        }

        public static string Classify(string name, bool dontSingularize = false)
        {
            if (dontSingularize) { return name.Underscore().Pascalize(); }
            else
            {
                if (String.IsNullOrEmpty(name.Singularize()))
                    return name.Underscore().Pascalize();
                else
                    return name.Underscore().Singularize().Pascalize();
            }
        }

        public static string UnClassify(string name, bool dontPluralize = false)
        {
            return (dontPluralize) ? name.Underscore().Hyphenate().ToLower() : name.Underscore().Hyphenate().ToLower().Pluralize();
        }

        public static object GetEnityObject(string jsonApiTypeName, string groupName)
        {
            return createObject(_assemblies[_dtoAssembliesNameSpace[groupName].Item2 + "." + Classify(jsonApiTypeName)]);
        }

        private static object createObject (Type type){
            return Activator.CreateInstance(type);
        }

    }
}
