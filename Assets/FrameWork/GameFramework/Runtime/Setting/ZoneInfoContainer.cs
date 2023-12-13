using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    [Serializable]
    public class ZoneInfoContainer
    {
        public List<ZoneType> zoneType;
        public List<ZoneInfoSO> data;
    }

    [Serializable]
    public class ZoneType
    {
        public string name;
        public bool isAnonymousChannel;
    }
}
