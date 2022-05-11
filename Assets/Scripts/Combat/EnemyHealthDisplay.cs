using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;
        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        private void Update()
        {
            if(fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
            }
            else
            {
                Health health = fighter.GetTarget();
                GetComponent<Text>().text = $"{health.GetHealthPoints()} / {health.GetMaxHealthPoints()}";
            }
           
        }
    }
}


