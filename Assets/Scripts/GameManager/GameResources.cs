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
	public RoomNodeTypeListSO typeListSO;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    [Tooltip("현재 플레이어 Scriptable Object - 장면 간에 현재 플레이어를 참조하는 데 사용됩니다.")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
	public Material dimmedMaterial;

	[Tooltip("Sprite-Lit-Default Material")]
	public Material litMaterial;

	[Tooltip("Lit Shader 변수를 넣어주세요.")]
	public Shader variableLitShader;
	#endregion

	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(typeListSO), typeListSO);
		HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
		HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
		HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
		HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
	}
#endif
	#endregion

}
