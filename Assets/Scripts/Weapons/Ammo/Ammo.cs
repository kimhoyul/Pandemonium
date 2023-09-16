using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    [Tooltip("TrailRenderer 컴포넌트를 가진 자식 오브젝트를 찾아서 할당해 주세요")]
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetailsSO;
    private float ammoChageTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (ammoChageTimer > 0f)
        {
            ammoChageTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetailsSO.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;
        transform.position += distanceVector;

        ammoRange -= distanceVector.magnitude;

        if (ammoRange <= 0f)
        {
            DisableAmmo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DisableAmmo();
    }

    /// <summary>
    /// `IFireable` 인터페이스를 통한 함수 구현. 총알 발사 전에 필요한 설정들을 여기서 해줌
    /// </summary>
    /// <param name="ammoDetails">총알에 대한 상세 정보</param>
    /// <param name="aimAngle">캐릭터 몸으로부터의 조준 각도</param>
    /// <param name="weaponAimAngle">무기로부터의 조준 각도</param>
    /// <param name="ammoSpeed">총알 속도</param>
    /// <param name="weaponAimDirectionVector">무기 조준 방향 벡터</param>
    /// <param name="overrideAmmoMovement">총알 움직임을 오버라이드 할 것인지 여부</param>
    public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo
        ammoDetailsSO = ammoDetails;

        // 발사 방향 설정해 줌.
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // 총알 스프라이트 이미지 설정해 줌.
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        // 총알의 충전 시간이 있으면 충전 타이머와 재질 설정
        if (ammoDetails.ammoChargeTime > 0f)
        {
            ammoChageTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        }
        else // 없으면 그냥 총알 재질 설정
        {
            ammoChageTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // 총알의 범위 설정
        ammoRange = ammoDetails.ammoRange;

        // 총알의 속도 설정
        this.ammoSpeed = ammoSpeed;

        // 총알 움직임 오버라이드 설정
        this.overrideAmmoMovement = overrideAmmoMovement;

        // 총알 게임 오브젝트 활성화
        gameObject.SetActive(true);
        #endregion

        #region TrailRenderer
        // 총알에 트레일(꼬리)이 있으면 관련 설정을 해준다.
        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else // 없으면 트레일(꼬리)를 꺼준다.
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }
        #endregion
    }

    /// <summary>
    /// 총알의 발사 방향을 설정
    /// </summary>
    /// <param name="ammoDetails">총알의 세부 사항</param>
    /// <param name="aimAngle">플레이어의 목표 각도</param>
    /// <param name="weaponAimAngle">무기의 목표 각도</param>
    /// <param name="weaponAimDirectionVector">무기의 목표 방향 벡터</param>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // 총알이 랜덤하게 퍼지는 강도 정함
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        // 총알이 위로 퍼질지, 아래로 퍼질지 토글. 1이면 위, -1이면 아래
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        // 무기의 방향 벡터의 크기가 설정된 거리보다 작으면 플레이어의 목표 각도 사용
        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        // 랜덤 퍼짐 범위와 위/아래 토글로 발사 각도 설정
        fireDirectionAngle += randomSpread * spreadToggle;

        // 총알의 방향 정함
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        // 각도로부터 방향 벡터를 구하기
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }


    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    private void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
#endif
    #endregion
}
