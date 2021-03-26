using RPG.Attributes;
using RPG.Control;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToTestore = 0;
        [SerializeField] float respawnTime = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject subject)
        {
            if(weapon != null) 
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            if(healthToTestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToTestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (var child in GetAllChildren())
            {
                child.SetActive(shouldShow);
            }
        }

        private GameObject [] GetAllChildren()
        {
            GameObject[] allChildren = new GameObject[transform.childCount];
            for(int i=0; i<transform.childCount; i++)
            {
                allChildren[i] = transform.GetChild(i).gameObject;
            }
            return allChildren;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PickUp(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
