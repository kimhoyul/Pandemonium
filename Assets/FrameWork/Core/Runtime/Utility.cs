using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace TOONIPLAY.Utilities
{
    public static class Utility
    {
        public static float MillisecondToSecond(int millisecond) => millisecond / 1000f;

        // NOTE(jaeil) : ContentSizeFitter가 LayoutGroup 타입 아래 있으면 Update가 두 번 일어나야 제대로 계산이 된다.
        // 그래서 EndOfFrame에 강제로 한 번 더 Update를 돌려주는 루틴 (https://forum.unity.com/threads/the-correct-way-to-use-content-size-fitter-inside-scrollarea.1327944/#post-8393082)
        public static IEnumerator CoForceUpdateLayout(GameObject rootObject)
        {
            yield return new WaitForEndOfFrame();

            var layoutGroup = rootObject.GetComponentsInChildren<ContentSizeFitter>();
            foreach (var layout in layoutGroup)
            {
                if (layout != null)
                {
                    layout.enabled = false;
                    layout.enabled = true;
                }
            }
        }

        public static T ToEnum<T>(object enumValue)
        {
            var index = Convert.ToInt32(enumValue.ToString());
            var array = Enum.GetValues(typeof(T));
            return (T)array.GetValue(index);
        }

        public static Type GetTypeByClassName(string fullClassName)
        {
            var currentAssemblyName = Assembly.GetAssembly(typeof(Utility)).GetName().Name;

            ParseClassName(fullClassName, out var className);

            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where assembly.GetReferencedAssemblies().Length != 0
                where IsReferencedAssembly(currentAssemblyName, assembly)
                from exportedType in assembly.ExportedTypes
                select exportedType).FirstOrDefault(exportedType => exportedType.Name.Equals(className));
        }

        private static bool IsReferencedAssembly(string assemblyName, Assembly assembly)
        {
            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                if (!assemblyName.Equals(referencedAssembly.Name))
                    continue;

                return true;
            }

            return false;
        }

        private static void ParseClassName(string fullClassName, out string className)
        {
            var lastDotIndex = fullClassName.LastIndexOf('.');
            className = lastDotIndex >= 0 ? fullClassName.Substring(lastDotIndex + 1) : fullClassName;
        }
    }
}
