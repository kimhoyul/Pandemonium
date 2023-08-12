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
	public RoomNodeTypeListSO roomNodeTypeList;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
	public Material dimmedMaterial;
}
