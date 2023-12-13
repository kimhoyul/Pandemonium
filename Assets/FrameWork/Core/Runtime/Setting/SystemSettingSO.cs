using UnityEngine;
using UnityEngine.UIElements;

namespace TOONIPLAY
{
    public abstract class SystemSettingSO : ScriptableObject
    {
        public string displayName;

#if UNITY_EDITOR
        public abstract void OnPropertyDraw(ref ScrollView rootElement);
#endif

        public abstract void ApplySetting();
    }
}
