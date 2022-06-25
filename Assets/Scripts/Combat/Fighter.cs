
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
        [SerializeField] WeaponConfig defaultWeapon;
        [SerializeField] String defaultWeaponName = "Unarmed";
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        Health target;
        Mover mover;
        Animator anim;
        float timeSinceLastAttack = Mathf.Infinity;

 
        private void Awake()
        {
            mover = GetComponent<Mover>();
            anim = GetComponent<Animator>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetDefaultWeapon);
        }

        private Weapon SetDefaultWeapon()
        {
            
            return AttachWeapon(defaultWeapon);
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
        public void EquipWeapon(WeaponConfig weapon)
        {
            if (weapon)
            {
                currentWeaponConfig = weapon;
                currentWeapon.value = AttachWeapon(weapon);
            }

        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
           return weapon.Spawn(righthandTransform, lefthandTransform, anim);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform, Vector3.up);
            if(timeSinceLastAttack > timeBetweenAtacks)
            {
                TriggerAttack();AttachWeapon(defaultWeapon);
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
            if (currentWeaponConfig)
                return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetRange();
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
                yield return currentWeaponConfig.GetDamage();
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
        void Hit() // Triggered by animation event
        {
            if (target == null) return;
             float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
           // if(currentWeapon.value != null)
                currentWeapon.value.OnHit();
            if (currentWeaponConfig.hasProjectile())
            {
                currentWeaponConfig.LauchProjectile(righthandTransform, lefthandTransform, target, gameObject, damage);

            }
            else
            {
                if (currentWeaponConfig)
                    target.TakeDamage(gameObject, damage);
            }
            
        }
        void Shoot() // Triggered by animation event
        {
            Hit();
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = state.ToString();
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            if (weapon != null)
            {
                 EquipWeapon(weapon);
            }
           
        }

        
    }
}
