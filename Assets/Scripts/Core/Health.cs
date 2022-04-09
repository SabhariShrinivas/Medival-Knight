using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 100;
        bool hasDied = false;

        public bool isDead()
        {
            return hasDied;
        }
        public void TakeDamage(float damage)
        {
           healthPoints = Mathf.Max(healthPoints - damage, 0);
            CheckForDeath();
        }
        void CheckForDeath()
        {
            if (!hasDied && healthPoints == 0)
            {
                GetComponent<Animator>().SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
                hasDied = true;
            }
        }
    }
}
