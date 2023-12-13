using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TOONIPLAY
{
    public struct MoveZoneInfo
    {
        public ZoneType zoneType;
        public ZoneInfoSO zoneInfo;
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;
    }

    public abstract class GameManager : TSingletonMonoBehaviour<GameManager>
    {
        private readonly Dictionary<string, IDataStorage> _dataStorageMap = new();

        public void RegisterDataStorage(IDataStorage storageInstance)
        {
            _dataStorageMap.Add(storageInstance.ToString(), storageInstance);
        }

        public static T GetDataStorage<T>() where T : IDataStorage
        {
            if (Instance._dataStorageMap.TryGetValue(typeof(T).ToString(), out var storage))
                return (T)storage;

            return default;
        }

        public static T As<T>() where T : GameManager
        {
            var result = Instance as T;
            Debug.Assert(result != null);

            return result;
        }

        /// <summary>
        /// 씬을 변경하며, spawnPosition 위치에 캐릭터를 스폰 시킨다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="spawnRotation"></param>
        public void MoveZone(string sceneName, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            Debug.Log($"Loading scene {sceneName}");
            int targetSceneIdx = SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{sceneName}.unity");
            Debug.Log($"Load Target Index : {targetSceneIdx}");

            var zoneInfo = PhaseManager.Instance.GetZoneInfo(sceneName);
#if (UNITY_ANDROID || UNITY_IOS)
            string phaseName = (zoneInfo != null) ? "ZonePhase" : sceneName;
#else
            string phaseName = (zoneInfo != null) ? "PCZonePhase" : sceneName;
#endif
            var moveZoneInfo = new MoveZoneInfo()
            {
                zoneType = PhaseManager.Instance.GetZoneTypeByInfo(zoneInfo),
                zoneInfo = zoneInfo,
                
                spawnPosition = spawnPosition,
                spawnRotation = spawnRotation
            };

            PhaseManager.Instance.LoadPhaseAsync(phaseName, moveZoneInfo, zoneInfo.blockingTypeName);
        }

        public void MoveZone(string sceneName)
        {
            Debug.Log($"Loading scene {sceneName}");
            int targetSceneIdx = SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{sceneName}.unity");
            Debug.Log($"Load Target Index : {targetSceneIdx}");

            var zoneInfo = PhaseManager.Instance.GetZoneInfo(sceneName);
#if (UNITY_ANDROID || UNITY_IOS)
            string phaseName = (zoneInfo != null) ? "ZonePhase" : sceneName;
#else
            string phaseName = (zoneInfo != null) ? "PCZonePhase" : sceneName;
#endif

            var spawnPosition = zoneInfo.spawnPoints.Count > 0 ? zoneInfo.spawnPoints[0].position : Vector3.zero;
            var spawnRotation = zoneInfo.spawnPoints.Count > 0 ? zoneInfo.spawnPoints[0].rotation : Quaternion.identity;

            var moveZoneInfo = new MoveZoneInfo()
            {
                zoneType = PhaseManager.Instance.GetZoneTypeByInfo(zoneInfo),
                zoneInfo = zoneInfo,
                spawnPosition = spawnPosition,
                spawnRotation = spawnRotation
            };

            PhaseManager.Instance.LoadPhaseAsync(phaseName, moveZoneInfo, zoneInfo.blockingTypeName);
        }

        public void OnSingleAction(string userId, int code, Vector3 position, int plusArrIndex = -1, DateTime? startAt = null)
        {
            var characterController = CharacterManager.Instance.GetCharacterController(userId);
            if (characterController != null)
                characterController.SetAction((Packets.ActionCmd)code, position, Quaternion.identity, 0.0f);//.SingleAction(code, position, startAt);
        }
    }
}
