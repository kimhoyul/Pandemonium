using EasyCharacterMovement;
using UnityEngine;

public class CharacterModelConfig : MonoBehaviour
{
    [SerializeField]
    private Vector3 eyePosition;

    [SerializeField]
    private Transform hipTF;

    public Vector3 EyePosition => eyePosition;
    
    public float FormRatioY => hipTF.localPosition.y / DEFAULT_FORM_Y;

    private const float DEFAULT_FORM_Y = 0.8613f;

    private static readonly int Forward = Animator.StringToHash("Speed");
    private static readonly int Ground = Animator.StringToHash("Grounded");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int CustomMode = Animator.StringToHash("CustomMode");
    private static readonly int FreeFall = Animator.StringToHash("FreeFall");

    private Character _character;

    public Character Character { set => _character = value; }

    private void Update()
    {
        if (_character == null)
            return;

        var deltaTime = Time.deltaTime;

        // Get Character animator

        var animator = _character.GetAnimator();
        if (animator == null)
            return;

        // Compute input move vector in local space

        var move = transform.InverseTransformDirection(_character.GetMovementDirection());

        // Update the animator parameters

        float forwardAmount = _character.useRootMotion && _character.GetRootMotionController() ? move.z : _character.GetSpeed();

        animator.SetBool(CustomMode, _character.GetMovementMode() == MovementMode.Custom);

        animator.SetFloat(Forward, forwardAmount, 0.1f, deltaTime);

        animator.SetBool(Ground, _character.IsGrounded());

        animator.SetBool(Jump, _character.IsFalling());

        animator.SetBool(FreeFall, !_character.IsJumping() && _character.IsFalling());
    }
}
