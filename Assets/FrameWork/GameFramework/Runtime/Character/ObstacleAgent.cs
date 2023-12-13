using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace TOONIPLAY
{
    [RequireComponent(typeof(NavMeshAgent), typeof(NavMeshObstacle))]
    public class ObstacleAgent : MonoBehaviour
    {
        [SerializeField]
        private float CarvingTime = 0.5f;

        [SerializeField]
        private float CarvingMoveThreshold = 0.1f;

        private float _lastMoveTime;
        private Vector3 _lastPosition;

        private NavMeshAgent _navMeshAgent;
        private NavMeshObstacle _navMeshObstacle;
        private InputHandler _inputHandler;

        public float RemainingDistance => _navMeshAgent != null && _navMeshAgent.enabled ? _navMeshAgent.remainingDistance : 0.0f;
        public float StoppingDistance
        {
            set => _navMeshAgent.stoppingDistance = value;
            get => _navMeshAgent.stoppingDistance;
        }
        public Vector3 DesiredVelocity => _navMeshAgent.desiredVelocity;
        public Vector3 Velocity => _navMeshAgent.velocity;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            _inputHandler = GetComponent<InputHandler>();

            _navMeshObstacle.enabled = false;
            _navMeshObstacle.carveOnlyStationary = false;
            _navMeshObstacle.carving = true;

            _lastPosition = gameObject.transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Vector3.Distance(_lastPosition, transform.position) > CarvingMoveThreshold)
            {
                _lastMoveTime = Time.time;
                _lastPosition = transform.position;
            }
            if (_lastMoveTime + CarvingTime < Time.time)
            {
                _navMeshAgent.enabled = false;
                _navMeshObstacle.enabled = true;
            }
        }

        public void SetDestination(Vector3 position)
        {
            if (Vector3.Distance(_lastPosition, position) <= CarvingMoveThreshold)
                return;

            _navMeshObstacle.enabled = false;

            _lastMoveTime = Time.time;
            _lastPosition = transform.position;

            StartCoroutine(CoSetDestination(position));
        }

        private IEnumerator CoSetDestination(Vector3 position)
        {
            yield return null;

            // 모드가 바뀌었을 때, 진행중인 코루틴이 함수를 실행시키지 못하도록 방지
            if (_inputHandler.CurrentInputType == InputHandler.InputType.AI)
            {
                SetEnableAgent(true);
                _navMeshAgent.SetDestination(position);
            }
        }

        public void SetEnableAgent(bool enableAgent)
        {
            _navMeshAgent.enabled = enableAgent;
            if (enableAgent)
                _navMeshObstacle.enabled = false;
        }
    }
}
