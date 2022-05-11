using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharecterClass[] charecterClasses;
        Dictionary<CharecterClass, Dictionary<Stat, float[]>> lookupTable;
        public float GetStat(Stat stat, CharecterClass charecterClass, int level)
        {
            BuildLookup();
            if (lookupTable.ContainsKey(charecterClass))
            {
                if (lookupTable[charecterClass].ContainsKey(stat))
                {
                    if (lookupTable[charecterClass][stat].Length > level)
                    {
                       // Debug.Log($"{charecterClass} {stat} {level}");
                        return lookupTable[charecterClass][stat][level-1];
                    }
                    else
                    {
                        //level array length less than level
                        Debug.Log($"{charecterClass} {stat} {level} level array length less than level");
                        return 0;
                    }
                }
                else
                {
                    //stat not found
                    Debug.Log("stat not found");
                    return 0;
                }
            }
            else
            {
                //charecterclass not found
                Debug.Log("charecterclass not found");
                return 0;
            }
        }
        public int GetLevels(Stat stat, CharecterClass charecterClass)
        {
            BuildLookup();
            return lookupTable[charecterClass][stat].Length;
        }

        private void BuildLookup()
        {
            if(lookupTable != null)
            {
                return;
            }
            lookupTable = new Dictionary<CharecterClass, Dictionary<Stat, float[]>>();
            foreach(ProgressionCharecterClass progressionCharecterClass in charecterClasses)
            {
                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();
                foreach(ProgressionStat progressionStat in progressionCharecterClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                lookupTable[progressionCharecterClass.charecterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharecterClass
        {
            public CharecterClass charecterClass;
            public ProgressionStat[] stats;

        }
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}