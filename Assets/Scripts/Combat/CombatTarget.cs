using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
           return CursorType.combat;
        }

        public bool HandleRaycast(PlayerController callingPlayerController)
        {
                if (!callingPlayerController.GetComponent<Fighter>().CanAttack(gameObject))
                {
                    return false;
                }
                if (Input.GetMouseButton(0))
                {
                callingPlayerController.GetComponent<Fighter>().Attack(gameObject);
                }             
                return true;
        }

    }
}
