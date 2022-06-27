using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using RPG.Attributes;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weaponType;
        [SerializeField] float healthToRestore = 0;
        [SerializeField] float respawnTime = 50;
        private void OnTriggerEnter(Collider collision)
        {
           if (collision.CompareTag("Player"))
            {
                PickUp(collision.gameObject);
            }
        }

        private void PickUp(GameObject subject)
        {
            if (weaponType == null) return;
            if (healthToRestore > 0) subject.GetComponent<Health>().Heal(healthToRestore);
            subject.GetComponent<Fighter>().EquipWeapon(weaponType);
            StartCoroutine(HideForSeconds(respawnTime));
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

        public bool HandleRaycast(PlayerController callingPlayerController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PickUp(callingPlayerController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }

}
