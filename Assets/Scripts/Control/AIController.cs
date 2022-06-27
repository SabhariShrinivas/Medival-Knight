
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspesionTime = 5f;
        [SerializeField] float agroCooldownTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float WaypointTolerance;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] UnityEvent onEnemyDetection;
        [SerializeField] float shoutRadius = 5f;
        [Range(0, 1)] [SerializeField] float patrolSpeedFraction = 0.2f;
        int CurrentWaypointIndex = 0;
        GameObject player;
        Fighter fighter;
        Health health;
        Mover mover;
        bool isAggrevated = false;
        LazyValue<Vector3> guardPosition;
        float timeSinceLastsawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;


        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }
        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }
        private void Start()
        {

            guardPosition.ForceInit();
        }
        private void Update()
        {
            if (health.isDead()) return;
            if (IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastsawPlayer < suspesionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        public void Aggeravate()
        {
            timeSinceAggrevated = 0;
        }
        private void UpdateTimers()
        {
            timeSinceLastsawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    CycleWaypoint();
                    timeSinceArrivedAtWaypoint = 0;
                }
                nextPosition = GetCurrentWayPoint();
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWayPoint(CurrentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            CurrentWaypointIndex = patrolPath.GetNextIndex(CurrentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWayPoint()) < WaypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastsawPlayer = 0;
            if (!isAggrevated)
            {
                onEnemyDetection.Invoke();
                isAggrevated = true;
            }
            fighter.Attack(player);
            AggrevateNearbyEnemies();
        }
        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutRadius, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null || ai == this) continue;
                ai.Aggeravate();
            }
        }
        bool IsAggrevated()
        {
            return Vector3.Distance(transform.position, player.transform.position) < chaseDistance || timeSinceAggrevated < agroCooldownTime;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
