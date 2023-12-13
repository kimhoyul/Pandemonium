using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TOONIPLAY
{
    public class SpawnPointManagerEditorWindow : EditorWindow
    {
        private const string ZONEINFO_PATH = "Assets/Datas/ZoneInfo";

        private readonly List<ZoneInfoSO> _zoneInfoAssets = new();
        private ZoneInfoSO _currentZoneInfo;

        [MenuItem("Tools/Spawn Point Manager")]
        public static void Init()
        {
            SpawnPointManagerEditorWindow window = (SpawnPointManagerEditorWindow)GetWindow(typeof(SpawnPointManagerEditorWindow));
            window.Show();

            window.titleContent.text = "Spawn Point Manager";

            window.minSize = new Vector2(400f, 100f);
            window.maxSize = new Vector2(400f, 150f);
        }

        private void OnEnable()
        {
            string[] assetPaths = AssetDatabase.FindAssets("t:ZoneInfoSO", new[] { ZONEINFO_PATH });

            _zoneInfoAssets.Clear();
            foreach (var assetPath in assetPaths)
            {
                var zoneInfo = AssetDatabase.LoadAssetAtPath<ZoneInfoSO>(AssetDatabase.GUIDToAssetPath(assetPath));
                _zoneInfoAssets.Add(zoneInfo);
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Spawn Point 저장", GUILayout.Height(100f)))
            {
                if (_zoneInfoAssets.Count == 0)
                {
                    Debug.Log("ZoneInfo.asset을 불러오지 못했습니다.");
                    return;
                }

                var currentScene = SceneManager.GetActiveScene();
                foreach (var zoneInfo in _zoneInfoAssets.Where(zoneInfo => currentScene.name == zoneInfo.zoneSceneName))
                {
                    _currentZoneInfo = zoneInfo;
                    break;
                }

                if (_currentZoneInfo == null)
                {
                    Debug.Log("현재 씬과 관련된 ZoneInfo가 없습니다.");
                    return;
                }

                if (_currentZoneInfo.spawnPoints.Count != 0)
                    _currentZoneInfo.spawnPoints.Clear();

                var spawnPoints = FindObjectsOfType(typeof(SpawnPoint)) as SpawnPoint[];
                switch (spawnPoints)
                {
                    case { Length: 0 }:
                        Debug.Log("현재 씬에는 스폰 지점이 없습니다.");
                        return;
                    
                    case null:
                        return;
                }

                foreach (var t in spawnPoints)
                {
                    var transform = t.transform;
                    SpawnPont point = new()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    };

                    _currentZoneInfo.spawnPoints.Add(point);
                }

                Debug.Log(_currentZoneInfo.name + "에 " + spawnPoints.Length + "개의 스폰 지점이 저장되었습니다.");
            }
        }
    }
}