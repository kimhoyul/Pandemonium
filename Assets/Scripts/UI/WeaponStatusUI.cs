using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class WeaponStatusUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    [Tooltip("자식 WeaponImage GameObject 의 Image 컴포넌트를 넣어주세요.")]
    [SerializeField] private Image weaponImage;
    [Tooltip("자식 AmmoHloder GameObject 의 Transform 컴포넌트를 넣어주세요.")]
    [SerializeField] private Transform ammoHolderTransform;
    [Tooltip("자식 ReloadText GameObject 의 TextMeshProUGUI 컴포넌트를 넣어주세요.")]
    [SerializeField] private TextMeshProUGUI reloadText;
    [Tooltip("자식 AmmoRemainingText GameObject 의 TextMeshProUGUI 컴포넌트를 넣어주세요.")]
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    [Tooltip("자식 WeaponNameText GameObject 의 TextMeshProUGUI 컴포넌트를 넣어주세요.")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [Tooltip("자식 ReloadBar GameObject 의 RectTransform 컴포넌트를 넣어주세요.")]
    [SerializeField] private RectTransform reloadBar;
    [Tooltip("자식 BarImage GameObject 의 Image 컴포넌트를 넣어주세요.")]
    [SerializeField] private Image barImage;
    #endregion

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlyer(); 
    }

    private void OnEnable()
    {
        // 무기가 변경되면 트리거
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        // 무기가 발사되면 트리거
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        // 무기기 재장전 시작할 때 트리거
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        // 무기가 재장전 끝나면 되면 트리거
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent wWeaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void WeaponReloaded(Weapon weapon)
    {
        // 재장전이 끝난 무기가 현재 무기와 같은지
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar(weapon);
        }
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetailsSO);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        // 무기가 재장전 중이었는지 확인 
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar(weapon);
        }

        UpdateReloadText(weapon);
    }

    /// <summary>
    /// 무기의 이미지를 업데이트
    /// </summary>
    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetailsSO)
    {
        weaponImage.sprite = weaponDetailsSO.weaponSprite;
    }

    /// <summary>
    /// 무기리스트에서의 위치와 무기의 이름을 업데이트
    /// </summary>
    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ")" + weapon.weaponDetailsSO.weaponName.ToUpper();
    }

    /// <summary>
    /// 무기의 남은 탄약수를 업데이트
    /// </summary>
    private void UpdateAmmoText(Weapon weapon)
    {
        // 무기가 무한 탄약 인지 확인
        if (weapon.weaponDetailsSO.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + "/" + weapon.weaponDetailsSO.weaponAmmoCapacity.ToString();
        }
    }

    /// <summary>
    /// 총알 아이콘을 업데이트
    /// </summary>
    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            // 총알 아이콘 인스턴스화
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }
    }

    /// <summary>
    /// 만들어진 총알 아이콘 인스턴스 파괴 및 리스트 초기화
    /// </summary>
    private void ClearAmmoLoadedIcons()
    {
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    /// <summary>
    /// 무기 재장전 프로그래스를 업데이트
    /// </summary>
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        // 무기가 무한 탄창 인지 확인
        if (weapon.weaponDetailsSO.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    /// <summary>
    /// 무기 재장전의 프로그래스를 표현
    /// </summary>    
    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        // Reload bar 의 색을 빨강으로 변경
        barImage.color = Color.red;

        // Reload bar 의 애니메이션 처리
        while (currentWeapon.isWeaponReloading)
        {
            // 현재 재장전 시간을 무기의 재장전 시간으로 나눠 0 ~ 1 사이의 값을 얻음
            float barfill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetailsSO.weaponReloadTime;

            // y(세로) 와 z(깊이) 값은 고정한채 x(가로) 값을 늘려 표현 
            reloadBar.transform.localScale = new Vector3(barfill, 1f, 1f);

            yield return null;
        }
    }

    /// <summary>
    /// 무기 재장전 프로그래스 초기화
    /// </summary>
    private void ResetWeaponReloadBar(Weapon weapon)
    {
        StopReloadWeaponCoroutine();

        // Reload bar 의 색을 초록으로 변경
        barImage.color = Color.green;

        // x(가로) 값을 1 로 늘려 완전히 채워짐을 표현
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// 재장전 코루틴 중지
    /// </summary>
    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    /// <summary>
    /// 재장전 텍스트 업데이트
    /// </summary>
    private void UpdateReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetailsSO.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    /// <summary>
    /// 깜빡이는 글자 애니메이션 효과
    /// </summary>
    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 깜빡이는 글자 애니메이션 종료
    /// </summary>
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }
    /// <summary>
    /// 깜빡이는 글자 코루틴 중지
    /// </summary>
    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }
#endif
    #endregion
}
