using Cinemachine;
using System;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

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
			radius = 1f,
			target = GameManager.Instance.GetPlyer().transform
		};

		CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineTargetGroup_player };

		cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
	}
}
