using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    public enum AnchorType
    {
        LeftTop,
        CenterTop,
        RightTop,

        LeftMiddle,
        CenterMiddle,
        RightMiddle,

        LeftBottom,
        CenterBottom,
        RightBottom
    }

    public static class RectTransformAnchorSetter
    {
        private static Dictionary<AnchorType, Vector3> anchorTypePosMap = new Dictionary<AnchorType, Vector3>()
    {
        { AnchorType.LeftTop,      new Vector3(0f, 1f)     },
        { AnchorType.CenterTop,    new Vector3(0.5f, 1f)   },
        { AnchorType.RightTop,     new Vector3(1f, 1f)     },
        { AnchorType.LeftMiddle,   new Vector3(0f, 0.5f)   },
        { AnchorType.CenterMiddle, new Vector3(0.5f, 0.5f) },
        { AnchorType.RightMiddle,  new Vector3(1f, 0.5f)   },
        { AnchorType.LeftBottom,   new Vector3(0f, 0f)     },
        { AnchorType.CenterBottom, new Vector3(0.5f, 0f)   },
        { AnchorType.RightBottom,  new Vector3(1f, 0f)     },
    };

        public static void SetAnchorPreset(ref RectTransform target, AnchorType anchorType)
        {
            Vector3 minPos = anchorTypePosMap[anchorType];
            Vector3 maxPos = anchorTypePosMap[anchorType];

            target.anchorMin = minPos;
            target.anchorMax = maxPos;
        }

        public static void SetAnchorPreset(ref RectTransform target, AnchorType anchorType, bool isSetPosition)
        {
            SetAnchorPreset(ref target, anchorType);

            if (isSetPosition)
            {
                target.pivot = anchorTypePosMap[anchorType];
                target.anchoredPosition = Vector3.zero;
            }
        }

        public static void SetAnchorPreset(ref RectTransform target, AnchorType anchorType, bool isSetPosition, Vector3 targetAnchoredPosition )
        {
            SetAnchorPreset(ref target, anchorType, isSetPosition);

            target.anchoredPosition = targetAnchoredPosition;
        }
    }

}
