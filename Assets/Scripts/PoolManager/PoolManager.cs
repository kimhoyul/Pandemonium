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

    /// <summary>
    /// 주어진 프리팹, 위치 및 회전을 기반으로 컴포넌트를 재사용
    /// </summary>
    /// <param name="prefab">재사용할 컴포넌트의 원본 프리팹</param>
    /// <param name="position">재사용할 컴포넌트의 대상 위취</param>
    /// <param name="rotation">재사용할 컴포넌트의 대상 회전값</param>
    /// <returns>재설정된 컴포넌트 또는 풀에 해당 프리팹이 없는 경우 null</returns>
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // 주어진 프리팹의 인스턴스 ID를 조회
        int poolKey = prefab.GetInstanceID();

        // poolDictionary 에서 해당 프리팹의 오브젝트 풀이 있는지 확인
        if (poolDictionary.ContainsKey(poolKey))
        {
            // 해당 프리팹의 오브젝트 풀에서 사용 가능한 컴포넌트를 가져오기
            Component componentToReUse = GetComponentFromPool(poolKey);

            // 가져온 컴포넌트의 위치, 회전 및 크기를 재설정
            ResetObject(position, rotation, componentToReUse, prefab);

            return componentToReUse;
        }
        else
        {
            // 해당 프리팹을 위한 오브젝트 풀이 없을 경우 로그를 출력하고 null을 반환
            Debug.Log($"{prefab}을 위한 오브젝트 풀이 없습니다.");
            return null;
        }
    }


    /// <summary>
    /// 특정 키에 따른 컴포넌트를 객체 풀에서 가져오기
    /// 가져온 컴포넌트는 풀에 다시 추가되어 순환 구조를 유지하며, 이미 활성화된 경우에는 비활성화됨
    /// </summary>
    /// <param name="poolKey">풀에 접근하기 위한 prefab 의 instanceID 키</param>
    /// <returns>풀에서 가져온 컴포넌트</returns>
    private Component GetComponentFromPool(int poolKey)
    {
        // 주어진 키를 사용하여 poolDictionary에서 해당 컴포넌트의 풀에 접근
        // 이 때, Dequeue 를 사용하여 풀의 첫 번째 요소(프리팹)를 가져옴
        Component componentToReUse = poolDictionary[poolKey].Dequeue();

        // 가져온 컴포넌트(프리팹)을 다시 풀의 끝에 넣기. 이렇게 하면 풀의 순환 구조를 유지 가능
        poolDictionary[poolKey].Enqueue(componentToReUse);

        // 가져온 컴포넌트(프리팹)가 현재 활성화되어 있는 경우, 비활성화
        // 이는 재사용을 위해 프리팹을 초기 상태로 돌리기 위한 것
        if (componentToReUse.gameObject.activeSelf == true)
        {
            componentToReUse.gameObject.SetActive(false);
        }

        // 최종적으로 가져온 컴포넌트(프리팹)를 반환
        return componentToReUse;
    }

    /// <summary>
    /// 주어진 위치, 회전 및 프리팹의 크기 정보를 사용하여 컴포넌트를 재설정
    /// </summary>
    /// <param name="position">컴포넌트를 배치할 대상 위치</param>
    /// <param name="rotation">컴포넌트에 적용할 회전값</param>
    /// <param name="componentToReUse">재설정할 컴포넌트</param>
    /// <param name="prefab">크기 정보를 가져올 대상 프리팹</param>
    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReUse, GameObject prefab)
    {
        // 컴포넌트의 위치를 주어진 위치로 재설정
        componentToReUse.transform.position = position;
        // 컴포넌트의 회전을 주어진 회전값으로 재설정
        componentToReUse.transform.rotation = rotation;
        // 컴포넌트의 크기를 주어진 프리팹의 크기로 재설정
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
