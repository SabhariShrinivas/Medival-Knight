
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;


namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        Health health;
        NavMeshAgent navMeshAgent;
        Animator animator;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] private float maxNavPathLengeth = 10f;

        private void Awake()
        {
            health = GetComponent<Health>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
        void Start()
        {
           
        }
        void Update()
        {
           navMeshAgent.enabled = !health.isDead();
            UpdateAnimator();
        }
        public void StartMoveAction(Vector3 destination, float speedFraction) //move the player as well but only called while moving after handling different interaction like attacking to cancel the attack and start to move
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }
        public void MoveTo(Vector3 destination, float speedFraction) // this is called mainly by by fighting class. If we were to call StartMoveAction instead, everytime Fighter.Cancel() will be called while we are fighting.
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(destination);
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        }
        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool haspath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathComplete) return false; // if navmesh is not reachable, for example navmesh present on top of a flat surface such as roof of a building, then player should not be able to move
            if (GetPathLength(path) >= maxNavPathLengeth) return false;
            return true;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }    
        private void UpdateAnimator() // this method updates the player animation during locomotion
        {
            Vector3 velocity = navMeshAgent.velocity;
            velocity = transform.InverseTransformDirection(velocity); //converts global velocity to local velocity 
            float speed = velocity.z;
            animator.SetFloat("forwardspeed", speed);
        }
        private float GetPathLength(NavMeshPath path)
        {
            float distance = 0;
            if (path.corners.Length < 2) return distance;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return distance;
        }
        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
