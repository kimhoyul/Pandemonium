using Cinemachine;
using System;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

	[Tooltip("CursorTarget GameObject 를 넣어주세요")]
	[SerializeField] private Transform cursorTarget;

    private void Awake()
    {
		cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
	}

	private void Start()
	{
		SetCinemachineTargetGroup();
	}

	private void SetCinemachineTargetGroup()
	{
		CinemachineTargetGroup.Target cinemachineTargetGroup_player = new CinemachineTargetGroup.Target 
		{
			weight = 1f,
			radius = 2.5f,
			target = GameManager.Instance.GetPlyer().transform
		};

		CinemachineTargetGroup.Target cinemachineTargetGroup_cursor = new CinemachineTargetGroup.Target
		{
			weight = 1f,
			radius = 1f,
			target = cursorTarget
		};

		CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineTargetGroup_player, cinemachineTargetGroup_cursor };

		cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
	}

	private void Update()
	{
		cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
	}
}
