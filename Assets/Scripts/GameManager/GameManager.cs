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

    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;

	protected override void Awake()
	{
        base.Awake();
        
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;
        
        InstantiatePlayer();
	}

	private void InstantiatePlayer()
	{
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);
	}

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    private void Update()
    {
        HandleGameState();

        ////---------------- 테스트용 코드----------------//
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    gameState = GameState.gameStarted;
        //}
        ////----------------------------------------------//
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

    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSucessfully)
        {
            Debug.LogError("지정된 방과 노드 그래프에서 던전을 구축할 수 없습니다.");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // 현재 방의 중간 위치 설정
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // 현재 위치와 가장 가까운 방 스폰지점으로 위치 변경
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);


    }

    public Player GetPlyer()
    {
		return player;
	}

	public Room GetCurrentRoom()
	{
        return currentRoom;
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
