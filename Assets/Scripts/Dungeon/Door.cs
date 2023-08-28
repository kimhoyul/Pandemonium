using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
	[Space(10)]
	[Header("OBJECT REFERENCES")]
	[Tooltip("DoorCollider GameObject의 BoxCollider2D component 를 넣어주세요.")]
	[SerializeField] private BoxCollider2D doorCollider;
}
