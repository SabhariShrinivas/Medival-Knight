using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        float healthPoints = -1;
        bool hasDied = false;
        [SerializeField] float regenerationPercentage = 70;
        private void Start()
        {
           
            if(healthPoints < 0)
            healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        private void OnEnable()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }
        private void OnDisable()
        {
            GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;

        }
        public bool isDead()
        {
            return hasDied;
        }
        public float GetHealthPoints()
        {
            return healthPoints;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        public void TakeDamage(GameObject instigator, float damage)
        {
          //  print($"{gameObject.name} took {damage} damage");
           healthPoints = Mathf.Max(healthPoints - damage, 0);
            CheckForDeath(instigator);
        }
        void CheckForDeath(GameObject instigator)
        {
            if (!hasDied && healthPoints == 0)
            {
                GetComponent<Animator>().SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
                AwardExperience(instigator);
                hasDied = true;
            }
        }

        private void AwardExperience(GameObject instigator)
        {
           Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperiencReward));
        }

        public object CaptureState()
        {
            return healthPoints;
        }
        public void RestoreState(object state)
        {
            float health = (float)state;
            healthPoints = health;
            CheckForDeath(gameObject);
        }
        public float GetPercentage()
        {
            return healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health) * 100;
        }
        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage / 100;
            healthPoints = Mathf.Max(healthPoints, regenHealthPoints);
        }

    }
}
