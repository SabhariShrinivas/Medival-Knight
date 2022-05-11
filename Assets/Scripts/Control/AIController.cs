
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspesionTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float WaypointTolerance;
        [SerializeField] float waypointDwellTime = 3f;
        [Range(0, 1)][SerializeField] float patrolSpeedFraction = 0.2f;
        int CurrentWaypointIndex = 0;
        GameObject player;
        Fighter fighter;
        Health health;
        Mover mover;
        Vector3 guardPosition;
        float timeSinceLastsawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
        }
        private void Start()
        {
            
            guardPosition = transform.position;
        }
        private void Update()
        {
            if (health.isDead()) return;
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
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

        private void UpdateTimers()
        {
            timeSinceLastsawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;
            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    CycleWaypoint();
                    timeSinceArrivedAtWaypoint = 0;
                }
                nextPosition = GetCurrentWayPoint();
            }
            if(timeSinceArrivedAtWaypoint > waypointDwellTime)
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
            fighter.Attack(player);
        }

        bool InAttackRangeOfPlayer()
        {
           return Vector3.Distance(transform.position, player.transform.position) < chaseDistance;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
