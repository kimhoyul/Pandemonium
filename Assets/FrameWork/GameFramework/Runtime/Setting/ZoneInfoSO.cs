using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    [CreateAssetMenu()]
    public class ZoneInfoSO : ScriptableObject
    {
        public string title;
        public string sceneTypeCd;
        public string spaceCd;
        public string zoneSceneName;
        public string blockingTypeName;
        public ZoneContentsInfo zoneContentsInfo;
        public List<SpawnPont> spawnPoints;
        public List<SpawnPont> entrancePoints;

        public bool IsEnableChanneling() => sceneTypeCd.Equals("PLAY") || sceneTypeCd.Equals("CONF");

        public bool IsEnableRandomAvatar() => !sceneTypeCd.Equals("MEET") && !sceneTypeCd.Equals("WORK");
    }

    [System.Serializable]
    public struct SpawnPont
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [System.Serializable]
    public struct ZoneContentsInfo
    {
        public int zoneType;
    }
}
