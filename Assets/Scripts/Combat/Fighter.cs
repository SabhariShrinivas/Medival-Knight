
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAtacks = 3f;
        [SerializeField] Transform righthandTransform;
        [SerializeField] Transform lefthandTransform;
        [SerializeField] Weapon defaultWeapon;
        [SerializeField] String defaultWeaponName = "Unarmed";
        LazyValue<Weapon> currentWeapon;
        Health target;
        Mover mover;
        Animator anim;
        float timeSinceLastAttack = Mathf.Infinity;

 
        private void Awake()
        {
            mover = GetComponent<Mover>();
            anim = GetComponent<Animator>();
            currentWeapon = new LazyValue<Weapon>(SetDefaultWeapon);
        }

        private Weapon SetDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
          //  Weapon weapon = Resources.Load<Weapon>(defaultWeaponName);
           // EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (!target) return;
            if (target.isDead())
            {
                return;
            }
            if (!GetIsInRange())
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }
        public void EquipWeapon(Weapon weapon)
        {
            if (weapon)
            {
                currentWeapon.value = weapon;
                AttachWeapon(weapon);
            }

        }

        private void AttachWeapon(Weapon weapon)
        {
            weapon.Spawn(righthandTransform, lefthandTransform, anim);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform, Vector3.up);
            if(timeSinceLastAttack > timeBetweenAtacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }

        }
        public Health GetTarget()
        {
            return target;
        }
        private void TriggerAttack()
        {
            anim.ResetTrigger("stopAttack");
            anim.SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            if (currentWeapon.value)
                return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetRange();
            else return false;
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health target = combatTarget.GetComponent<Health>();
            return target != null && !target.isDead();
        }
        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();

        }

        private void StopAttack()
        {
            anim.ResetTrigger("attack");
            anim.SetTrigger("stopAttack");
        }
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }
        void Hit() // Triggered by animation event
        {
            if (target == null) return;
             float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            print($"{gameObject.name} dealt : {damage} damage");
            if (currentWeapon.value.hasProjectile())
            {
                currentWeapon.value.LauchProjectile(righthandTransform, lefthandTransform, target, gameObject, damage);
            }
            else
            {
                if (currentWeapon.value)
                    target.TakeDamage(gameObject, damage);
            }
            
        }
        void Shoot() // Triggered by animation event
        {
            Hit();
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = state.ToString();
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            if (weapon != null)
            {
                 EquipWeapon(weapon);
            }
           
        }

        
    }
}
