using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharecterClass charecterClass;
        [SerializeField] Progression progression;
        [SerializeField] GameObject levelUpEffect;
        [SerializeField] bool shouldUseModifier = false;
        LazyValue<int> currentLevel;
        public event Action OnLevelUp;
        Experience experience;
        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }
        private void OnEnable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
        }
        private void OnDisable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained -= UpdateLevel;
            }
        }
        private void Start()
        {
            currentLevel.ForceInit();
                   
        }
        ///<summary>
        ///Updates Level of Player
        ///</summary>
        private void UpdateLevel()
        {
            ///updates level
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                OnLevelUp?.Invoke();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * ( 1 + GetPercentageModifier(stat) / 100);
        }

       
        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, charecterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifier) return 0; // only player should use Should use modifier. we dont want enemy to use modifiers
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifier) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }


        public int GetLevel()
        { 
            return currentLevel.value;
        }
        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;
            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, charecterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, charecterClass, level);
                if(XpToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
            
        }
    }
    
}

