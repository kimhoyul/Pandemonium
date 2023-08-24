using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetailsSO", order = 1)]
public class MovementDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("COMMON")]
    [Tooltip("최소 이동 속도입니다. GetMoveSpeed 메서드는 최소값과 최대값 사이의 무작위 값을 계산합니다.")]
    public float minMoveSpeed = 8f;
    [Tooltip("최대 이동 속도입니다. GetMoveSpeed 메서드는 최소값과 최대값 사이의 무작위 값을 계산합니다.")]
    public float maxMoveSpeed = 8f;

    [Space(10)]
    [Header("PLAYER ONLY")]
    [Tooltip("Roll 중 이동속도 입니다.")]
    public float rollSpeed;
    [Tooltip("Roll 의 이동거리 입니다.")]
    public float rollDistance;
    [Tooltip("롤 액션의 반복방지를 위한 CoolDown 시간 입니다.")]
    public float rollCooldownTime;

    // 최소 이동 속도와 최대 이동 속도 를 비교후 같으면 최소 이동 속도를, 다르면 최소와 최대값 사이의 무작위 값을 반환
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        // 최소 이동 속도와 최대 이동 속도에 대한 유효성 검사
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        // Roll 관련 값에 대한 유효성 검사
        // PLAYER ONLY 값이기 때문에 값이 0이 아닐경우만 체크 진행 
        if (rollDistance != 0f || rollSpeed != 0f || rollCooldownTime != 0f)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif
    #endregion
}
