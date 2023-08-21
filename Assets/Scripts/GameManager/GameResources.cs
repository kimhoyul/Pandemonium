using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
	private static GameResources instance;
	
	public static GameResources Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load<GameResources>("GameResources");
			}
			return instance;
		}
	}

	#region Header DUNGEON
	[Space(10)]
	[Header("DUNGEON")]
	[Tooltip("RoomNodeTypeListSO 로 채우세요")]
	#endregion
	public RoomNodeTypeListSO typeList;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    [Tooltip("현재 플레이어 Scriptable Object - 장면 간에 현재 플레이어를 참조하는 데 사용됩니다.")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
	public Material dimmedMaterial;
}
