using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    [Tooltip("이 배열을 원하는 프리팹으로 채우고, 각각 생성할 게임 오브젝트의 수를 지정하세요.")]
    [SerializeField] private Pool[] poolArray = null;
    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        objectPoolTransform = this.gameObject.transform;

        for (int i = 0; i < poolArray.Length; i++)
        {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        // poolDictionary 의 Key 값
        int poolKey = prefab.GetInstanceID();

        // prefab pool 의 게임오브젝트 생성시 풀 헤더 오브젝트로 생성될 이름
        string prefabName = prefab.name;

        // 풀 헤더 오브젝트 생성
        GameObject parentGameObject = new GameObject(prefabName + "Anchor");

        // PoolManager에 프리펩 풀 헤더 를 붙이기
        parentGameObject.transform.SetParent(objectPoolTransform);

        // poolDictionary 에 이미 같은 프리펩의 풀이 있는지 확인
        if (!poolDictionary.ContainsKey(poolKey))
        {
            // 풀 딕셔너리에 새로운 프리펩의 풀 생성
            poolDictionary.Add(poolKey, new Queue<Component>());

            // 설정한 풀의 사이즈 만큼 풀에 프리펩 생성하여 채우기
            for (int i = 0; i< poolSize; i++)
            {
                // 새로운 프리팹 생성하여 풀 헤더에 붙이기
                GameObject newObject = Instantiate(prefab, parentGameObject.transform);
                newObject.SetActive(false);

                // 풀 딕셔너리에 생성한 프리펩 풀에 접근 하여 생성한 게임 오프젝트 채우기
                poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
            }
        }
    }

    public Component ReuseCimponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();
        
        if (poolDictionary.ContainsKey(poolKey))
        {
            Component componentToReUse = GetComponentFromPool(poolKey);

            ResetObject(position, rotation, componentToReUse, prefab);

            return componentToReUse;
        }
        else
        {
            Debug.Log($"{prefab}을 위한 오브젝트 풀이 없습니다.");
            return null;
        }

    }

    private Component GetComponentFromPool(int poolKey)
    {
        Component componentToReUse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReUse);

        if (componentToReUse.gameObject.activeSelf == true)
        {
            componentToReUse.gameObject.SetActive(false);
        }

        return componentToReUse;
    }

    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReUse, GameObject prefab)
    {
        componentToReUse.transform.position = position;
        componentToReUse.transform.rotation = rotation;
        componentToReUse.gameObject.transform.localScale = prefab.transform.localScale;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif
    #endregion
}
