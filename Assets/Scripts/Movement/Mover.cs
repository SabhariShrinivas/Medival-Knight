using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Combat;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
    {
      //  [SerializeField] Transform target;

        NavMeshAgent navMeshAgent;
        Animator animator;
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            UpdateAnimator();
        }
        public void StartMoveAction(Vector3 destination) //move the player as well but only called while moving after handling different interaction like attacking to cancel the attack and start to move
        {
            GetComponent<Fighter>().Cancel();
            MoveTo(destination);
        }
        public void MoveTo(Vector3 destination) // this is called mainly by by fighting class. If we were to call StartMoveAction instead, everytime Fighter.Cancel() will be called while we are fighting.
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(destination);
        }

        public void Stop()
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
    }
}
