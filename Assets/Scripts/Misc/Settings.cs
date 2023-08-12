using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{

    #region DUNGEON BUILD SETTINGS
    // Room Node Graph 의 최대 던전 재구축 시도 횟수
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    // 최대 던전 구축 시도 횟수
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region ROOM SETTINGS
    public const int MaxChildCorridors = 3;
	#endregion
}
