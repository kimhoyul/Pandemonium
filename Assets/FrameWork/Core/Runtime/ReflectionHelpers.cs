using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TOONIPLAY.Utilities
{
    public static class ReflectionHelpers
    {
        private static IEnumerable<Type> GetTypesInAssembly(Assembly assembly, Predicate<Type> predicate)
        {
            if (assembly == null)
                return null;

            var types = Type.EmptyTypes;
            try
            {
                types = assembly.GetTypes();
            }
            catch (Exception)
            {
                // Can't load the types in this assembly
            }
            
            types = (from t in types
                     where t != null && predicate(t)
                     select t).ToArray();
            return types;
        }

        public static Type GetTypeInAllDependentAssemblies(string typeName) => GetTypesInAllDependentAssemblies(t => t.Name == typeName).FirstOrDefault();

        public static IEnumerable<Type> GetTypesInAllDependentAssemblies(Predicate<Type> predicate)
        {
            var foundTypes = new List<Type>(100);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var definedIn = typeof(SafeArea).Assembly.GetName().Name;
            foreach (var assembly in assemblies)
            {
                // Note that we have to call GetName().Name.  Just GetName() will not work.  
                if (assembly.GlobalAssemblyCache || (assembly.GetName().Name != definedIn && assembly.GetReferencedAssemblies().All(a => a.Name != definedIn)))
                    continue;
                
                try
                {
                    foundTypes.AddRange(GetTypesInAssembly(assembly, predicate));
                }
                catch (Exception)
                {
                    // ignored
                } // Just skip uncooperative assemblies
            }
            return foundTypes;
        }
    }
}
