using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> healthPoints;
        bool hasDied = false;
        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        //Unity doesnt serialize generic classes
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }
        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        private void Start()
        {
            healthPoints.ForceInit();
            //if(healthPoints < 0)
            //healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
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
            return healthPoints.value;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        public void TakeDamage(GameObject instigator, float damage)
        {
            //  print($"{gameObject.name} took {damage} damage");
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            
            if (healthPoints.value == 0)
            {
                onDie.Invoke(); //so that sfx is not called everytime oncapture is called and enemy is dead already
            }
            else
            {
                takeDamage.Invoke(damage); // so that hit and die audio source are not called at same time
            }
            CheckForDeath(instigator);
        }
        void CheckForDeath(GameObject instigator)
        {
            if (!hasDied && healthPoints.value == 0)
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
            return healthPoints.value;
        }
        public void RestoreState(object state)
        {
            float health = (float)state;
            healthPoints.value = health;
            CheckForDeath(gameObject);
        }
        public float GetPercentage()
        {
            return GetFraction() * 100;
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage / 100;
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

    }
}
