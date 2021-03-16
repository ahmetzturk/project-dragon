using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private GameObject equippedPrefab = null;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;
        public float WeaponRange { get => weaponRange;}
        public float WeaponDamage { get => weaponDamage;}

        public void Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            if (equippedPrefab != null)
            {           
                Instantiate(equippedPrefab, GetHandTransform(rightHandTransform, leftHandTransform));               
            }
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
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

        public void LaunchProjectile(Transform rightHandTransform, Transform leftHandTransform, Health target)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHandTransform, leftHandTransform).position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }
    }
}
