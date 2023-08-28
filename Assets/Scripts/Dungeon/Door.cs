using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
	[Space(10)]
	[Header("OBJECT REFERENCES")]
	[Tooltip("DoorCollider GameObject의 BoxCollider2D component 를 넣어주세요.")]
	[SerializeField] private BoxCollider2D doorCollider;

	[HideInInspector] public bool isBossRoomDoor = false;
	private BoxCollider2D doorTrigger;
	private bool isOpen = false;
	// 이전에 열렸었던 문인지 확인 -> 모든 문이 이미 열렷던건 아님, 이전에 열렸다는건 해당하는 문 이전의 적을 다 해치웠다는거고 그 문만 다시 열어둬야함 
	private bool previouslyOpened = false;
	private Animator animator;

	private void Awake()
	{
		doorCollider.enabled = false;

		animator = GetComponent<Animator>();
		doorTrigger = GetComponent<BoxCollider2D>(); 
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
		{
			OpenDoor();
		}
	}

	private void OnEnable()
	{
		animator.SetBool(Settings.Open, isOpen);
	}

	private void OpenDoor()
	{
		if (!isOpen)
		{
			isOpen = true;
			previouslyOpened = true;
			doorCollider.enabled = false;
			doorTrigger.enabled = false;

			animator.SetBool(Settings.Open, true);
		}
	}

	public void LockDoor()
	{
		isOpen = false;
		doorCollider.enabled = true;
		doorTrigger.enabled = false;

		animator.SetBool(Settings.Open, false);

	}

	public void UnlockDoor()
	{
		doorCollider.enabled = false;
		doorTrigger.enabled = true;

		if (previouslyOpened)
		{
			OpenDoor();
		}
	}

	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
	}
#endif
	#endregion
}
