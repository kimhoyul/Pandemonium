using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailSO : ScriptableObject
{
    [Space(10)]
    [Header("AMMO BASE DETAILS")]
    [Tooltip("탄약 이름")]
    public string ammoName;
    public bool isPlayerAmmo;

    [Space(10)]
    [Header("AMMO SPRITE, PREFAB & MATERIALS")]
    [Tooltip("탄약의 스프라이트")]
    public Sprite ammoSprite;
    [Tooltip("탄약의 프리팹 배열 - 배열에 넣어둔 프리펩중 랜덤한 프리펩이 선택되어 발사 됩니다.")]
    public GameObject[] ammoPrefabArray;
    [Tooltip("탄약의 메테리얼 - 탄약의 EmissionShader 에 따라 탄약이 빛나는게 바뀝니다.")]
    public Material ammoMaterial;
    [Tooltip("탄약의 차지 대기시간 - 탄약이 패턴을 그리는 무기에 사용됩니다. 발사 되기전 잠시 멈춤을 위한 변수입니다.")]
    public float ammoChargeTime = 0.1f;
    [Tooltip("탄약이 충전에 빛나게될 메테리얼 입니다.")]
    public Material ammoChargeMaterial;

    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]
    [Tooltip("탄약의 데미지")]
    public int ammoDamage = 1;
    [Tooltip("탄약의 최소 속도")]
    public float ammoSpeedMin = 20f;
    [Tooltip("탄약의 최대 속도")]
    public float ammoSpeedMax = 20f;
    [Tooltip("탄약의 사거리")]
    public float ammoRange = 20f;
    [Tooltip("탄약의 회전 속도 - 탄약의 패턴이 회전 하는 속도에 사용됩니다.")]
    public float ammoRotationSpeed = 1f;

    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]
    [Tooltip("탄약의 최소 분산 각도 입니다.")]
    public float ammoSpreadMin = 0f;
    [Tooltip("탄약의 최대 분산 각도 입니다.")]
    public float ammoSpreadMax = 0f;
}   
