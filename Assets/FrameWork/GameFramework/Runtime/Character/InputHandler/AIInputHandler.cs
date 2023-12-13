using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

namespace TOONIPLAY
{
    public class AIInputHandler : BaseInputHandler
	{
        public override InputHandler.InputType InputType => InputHandler.InputType.AI;
        public override Vector3 Velocity => (_obstacleAgent != null) ? _obstacleAgent.DesiredVelocity * Mathf.Clamp(_obstacleAgent.RemainingDistance, 0.0f, 1.0f) : Vector3.zero;

        public override bool Sprint => _sprint;

        private ObstacleAgent _obstacleAgent;

		private Transform _targetUser;

        private bool _sprint;

        public AIInputHandler()
		{
		}

        private void Awake()
        {
            _obstacleAgent = GetComponentInParent<ObstacleAgent>();
        }

        public void SetFollowTarget(Transform target)
        {
            Debug.Log("타겟 : " + target);
            _targetUser = target;
            _obstacleAgent.StoppingDistance = 1.5f;
        }

        private void Update()
        {
            Vector3 targetPosition = _obstacleAgent.gameObject.transform.position;
            if (_targetUser != null)
                targetPosition = _targetUser.position;
            
            if (Vector3.Distance(transform.position, targetPosition) < _obstacleAgent.StoppingDistance)
            {
                _targetUser = null;
                GetComponentInParent<InputHandler>().SetInputType(InputHandler.InputType.Human);
            }

            if (_obstacleAgent != null)
            {
                _obstacleAgent.SetDestination(targetPosition);
                
                _sprint = _obstacleAgent.RemainingDistance > 3f;
            }
        }
    }
}
