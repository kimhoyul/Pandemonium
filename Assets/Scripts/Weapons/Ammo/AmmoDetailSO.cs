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
    [Tooltip("탄약의 최소 속도 입니다. - 탄약의 최소 속도와 최대 속도 사이의 랜덤한 속도가 선택됩니다.")]
    public float ammoSpeedMin = 20f;
    [Tooltip("탄약의 최대 속도 입니다. - 탄약의 최소 속도와 최대 속도 사이의 랜덤한 속도가 선택됩니다.")]
    public float ammoSpeedMax = 20f;
    [Tooltip("탄약의 사거리")]
    public float ammoRange = 20f;
    [Tooltip("탄약의 회전 속도 - 탄약의 패턴이 회전 하는 속도에 사용됩니다.")]
    public float ammoRotationSpeed = 1f;

    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]
    [Tooltip("탄약의 최소 분산 각도 입니다. - 최소 분산 각도와 최대 분산 각도 사이의 랜덤한 각도가 선택됩니다.")]
    public float ammoSpreadMin = 0f;
    [Tooltip("탄약의 최대 분산 각도 입니다. - 최소 분산 각도와 최대 분산 각도 사이의 랜덤한 각도가 선택됩니다.")]
    public float ammoSpreadMax = 0f;

    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]
    [Tooltip("탄약의 최소 생성 개수 입니다. - 최소 생성 개수와 최대 생성 개수 사이의 랜덤한 개수가 선택됩니다.")]
    public int ammoSpawnAmountMin = 1;
	[Tooltip("탄약의 최대 생성 개수 입니다. - 최소 생성 개수와 최대 생성 개수 사이의 랜덤한 개수가 선택됩니다.")]
	public int ammoSpawnAmountMax = 1;
	[Tooltip("탄약의 최소 생성 간격 입니다. - 최소 생성 간격과 최대 생성 간격 사이의 랜덤한 간격이 선택됩니다.")]
	public int ammoSpawnIntervalMin = 0;
	[Tooltip("탄약의 최대 생성 간격 입니다. - 최소 생성 간격과 최대 생성 간격 사이의 랜덤한 간격이 선택됩니다.")]
	public int ammoSpawnIntervalMax = 0;

	[Space(10)]
	[Header("AMMO TRAIL DETAILS")]
	[Tooltip("탄알 궤적이 필요하다면 True 로 선택해주세요. 선택한경우에는 탄알 궤적 값들도 채워야 합니다. 그렇지 않은 경우 선택을 해제하세요.")]
	public bool isAmmoTrail = false;
    [Tooltip("탄약 궤적의 지속 시간")]
    public float ammoTrailTime = 3f;
    [Tooltip("탄약 궤적의 메태리얼")]
    public Material ammoTrailMaterial;

}
