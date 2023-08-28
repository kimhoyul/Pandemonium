using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
	#region UNITS
    public const float pixelPerUnit = 16f;
    public const float tileSizePixel = 16;
	#endregion

	#region ROOM NODE GRAPH EDITOR SETTINGS
	// Node Layout 값 설정
	public const float roomNodeWidth = 160f;
    public const float roomNodeHeight = 75f;

    public const int nodePadding = 25;
    public const int nodeBorder = 12;

    // Node 연결선 값 설정
    public const float connectingLineWidth = 3f;
    public const float connectingLineArrowSize = 6f;

    // 그리드 간격 값 설정
    public const float gridLarge = 100f;
    public const float gridSmall = 25f;
    #endregion

    #region DUNGEON BUILD SETTINGS
    // Room Node Graph 의 최대 던전 재구축 시도 횟수
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    // 최대 던전 구축 시도 횟수
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region ROOM SETTINGS
    public const int MaxChildCorridors = 3;
	#endregion

	#region ANIMATOR PARAMETERS
    public static int aimUp = Animator.StringToHash("aimUp");
	public static int aimDown = Animator.StringToHash("aimDown");
	public static int aimUpRight = Animator.StringToHash("aimUpRight");
	public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
	public static int aimRight = Animator.StringToHash("aimRight");
	public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollDown = Animator.StringToHash("rollDown");

    public static int Open = Animator.StringToHash("open");
    public static int close = Animator.StringToHash("close");
	#endregion

	#region GAMEOBJECT TAGS
	public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
		#endregion
}
