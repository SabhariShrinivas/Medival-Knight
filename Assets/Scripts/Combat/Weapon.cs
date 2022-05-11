
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "weapons/Make new weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride;
        [SerializeField] GameObject equippedPrefab;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 15;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile;
        private const string weaponName = "weapon";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            if(equippedPrefab != null)
            {
                Transform handTransform = isRightHanded ? rightHand : leftHand;
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }
           
            if (animatorOverride != null)
            animator.runtimeAnimatorController = animatorOverride;
        }

        public bool hasProjectile()
        {
            return projectile != null;
        }
        public void LauchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float CalculatedDamage)
        {
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            Projectile projectileInstance = Instantiate(projectile, handTransform.position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, CalculatedDamage);
        }
        public float GetDamage()
        {
            return weaponDamage;
        }
        public float GetPercentageBonus()
        {
            return percentageBonus;
        }
        public float GetRange()
        {
            return weaponRange;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

    }
}
