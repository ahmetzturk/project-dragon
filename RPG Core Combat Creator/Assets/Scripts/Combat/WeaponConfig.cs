using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private Weapon equippedPrefab = null;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float percentageBonus = 0f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;
        const string weaponName = "weapon";
        public float WeaponRange { get => weaponRange;}
        public float WeaponDamage { get => weaponDamage;}
        public float PercentageBonus { get => percentageBonus; }

        public Weapon Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {          
            DestroyOldWeapon(rightHandTransform, leftHandTransform);

            Weapon weapon = null;

            if (equippedPrefab != null)
            {           
                weapon = Instantiate(equippedPrefab, GetHandTransform(rightHandTransform, leftHandTransform));            
;               weapon.gameObject.name = weaponName;
            }
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else
            {
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if(overrideController != null)
                {
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                }
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform oldWeapon = rightHandTransform.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHandTransform.Find(weaponName);
            }
            if (oldWeapon == null) return;
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            if (isRightHanded)
                return rightHandTransform;
            else              
                return leftHandTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHandTransform, Transform leftHandTransform, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHandTransform, leftHandTransform).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

    }
}
