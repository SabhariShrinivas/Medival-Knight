using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab;
        void Start()
        {
        }
        public void Spawn(float damageAmount) //called by unity Event
        {
            DamageText instance = Instantiate(damageTextPrefab, transform);
            instance.SetValue(damageAmount);
        }
    }
}
