using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;
using RPG.Combat;
using RPG.Resources;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Mover mover;
        enum CursorType
        {
            None,
            Movement,
            Combat
        }

        
        
        void Awake()
        {
            mover = GetComponent<Mover>();
        }

        
        void Update()
        {
            if (GetComponent<Health>().IsDead()) return;
            if (InteractWithCombat()) return;        
            if (InteractWithMovement()) return;
            //print("Nothing to do");
            SetCursor(CursorType.None);
        }
        
        private bool InteractWithCombat()
        { 
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (var hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;
                if (!GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    continue;
                }
                if (Input.GetMouseButton(0))
                    GetComponent<Fighter>().Attack(target.gameObject);
                SetCursor(CursorType.Combat);
                return true;               
            }
            return false;
        }

        private bool InteractWithMovement()
        {                            
            RaycastHit hit;
            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(hit.point, 1);               
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private void SetCursor(CursorType movement)
        {
            //Cursor.SetCursor()
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
