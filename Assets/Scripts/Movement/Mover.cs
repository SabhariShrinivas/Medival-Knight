
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
