using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/던전/던전 레벨")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS
    [Space(10)]
    [Header("기본 레벨 정보")]
    [Tooltip("레벨의 이름")]
    #endregion
    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL
    [Space(10)]
    [Header("레벨의 방 템플릿들")]
    [Tooltip("레벨에 포함되길 원하는 방 템플릿들로 리스트를 채워넣으세요. " +
        "Room Node Graph List 에 지정된 모든 Room Node 들의 Type 별 템플릿 을 넣어주어야 합니다.")]
    #endregion
    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPHS FOR LEVEL
    [Space(10)]
    [Header("레벨의 ROOM NODE GRAPH")]
    [Tooltip("이 리스트에는 레벨에서 무작위로 선택되어야 할 노드 그래프들을 채워넣으세요.")]
    #endregion
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        //-------------------------- 내부 변수 유효성 확인 --------------------------//
        // 레벨 이름이 비어있는지 확인
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);

        // roomTemplateList 가 비어있지 않은지 확인
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;

        // roomNodeGraphList 가 비어있지 않은지 확인
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;
        //--------------------------------------------------------------------//

        //-------------------------- 내부 변수 검증 --------------------------//
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        // 등록한 템플릿 리스트 에서 E/W Corridor, N/S Corridor, Entrance 가 지정되었는지 확인.
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (isEWCorridor == false)
        {
            Debug.Log($"<b><color=yellow>{this.name.ToString()}</color></b> 에 E/W Corridor Room Type 이 지정되지 않았습니다.");
        }

        if (isNSCorridor == false)
        {
            Debug.Log($"<b><color=yellow>{this.name.ToString()}</color></b> 에 N/S Corridor Room Type 이 지정되지 않았습니다.");
        }

        if (isEntrance == false)
        {
            Debug.Log($"<b><color=yellow>{this.name.ToString()}</color></b> 에 Entrance Room Type 이 지정되지 않았습니다.");
        }
        //--------------------------------------------------------------------//


        // 모든 노드 그래프를 순환.
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            // 노드 그래프 내 모든 노드를 순환.
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                // 각 방 노드 유형에 대해 방 템플릿이 지정되었는지 확인
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS
                    || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                bool isRoomNodeTypeFound = false;

                // 이 노드 유형이 지정되었는지 확인하기 위해 모든 방 템플릿을 순환.
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }
                if (!isRoomNodeTypeFound)
                    Debug.Log($"<b><color=yellow>{roomNodeGraph.name.ToString()}</color></b>의 <b><color=yellow>{roomNodeSO.roomNodeType.name.ToString()}</color></b>의 탬플릿을 찾을수 없습니다. <b><color=yellow>Room Template List</color></b> 에 추가해 주세요.");
            }
        }
    }
#endif
    #endregion
}
