using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weaponType;
        [SerializeField] float respawnTime = 50;
        private void OnTriggerEnter(Collider collision)
        {
           if (collision.CompareTag("Player"))
            {
                collision.GetComponent<Fighter>().EquipWeapon(weaponType);
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }
        private IEnumerator HideForSeconds(float time)
        {
            HidePickup();
            yield return new WaitForSeconds(time);
            ShowPickup();
        }

        private void ShowPickup()
        {
            GetComponent<SphereCollider>().enabled = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        private void HidePickup()
        {
            GetComponent<SphereCollider>().enabled = false;
            for(int i=0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

}