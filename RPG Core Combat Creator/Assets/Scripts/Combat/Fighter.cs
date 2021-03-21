using GameDevTV.Utils;
using RPG.Core;
using RPG.Movement;
using RPG.Resources;
using RPG.Saving;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        private Health target = null;
        [SerializeField] private float timeBetweenAttacks = 1f;
        private float timeSinceLastAttack = Mathf.Infinity;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;
        private LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead()) return;
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }
        
        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        public void AttachWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            if (rightHandTransform == null) return;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }
        
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            // this will trigger the hit animation event
            GetComponent<Animator>().SetTrigger("attack");
        }

        // Animation Event
        void Hit()
        {
            if (target != null)
            {
                float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
                if (currentWeapon.value.HasProjectile())
                {
                    currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
                }
                else
                {
                    target.TakeDamage(gameObject, damage);
                }
            }                     
        }
        void Shoot()
        {
            Hit();                  
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(target.transform.position, transform.position) <= currentWeapon.value.WeaponRange;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeapon.value.WeaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.PercentageBonus;
            }
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return (targetToTest != null && !targetToTest.IsDead());
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
      
    }
}
