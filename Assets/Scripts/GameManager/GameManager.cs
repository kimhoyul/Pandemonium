using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    [Tooltip("던전 레벨 스크립터블 오브젝트로 채우세요.")]
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    [Tooltip("테스트를 위해 시작 던전 레벨로 채우세요. 첫 번째 레벨 = 0")]
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    [HideInInspector] public GameState gameState;

    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    private void Update()
    {
        HandleGameState();

        //---------------- 테스트용 코드----------------//
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
        //----------------------------------------------//
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {

    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
       HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}
