using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;
using RPG.Attributes;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Mover mover;
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavhMeshProjectionDistance = 1f;      
        [SerializeField] float raycastRadius = 1f;

        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }      
        
        void Awake()
        {
            mover = GetComponent<Mover>();
        }
        
        void Update()
        {
            if (InteractWithUI()) return;
            if (GetComponent<Health>().IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;     
            if (InteractWithMovement()) return;
            //print("Nothing to do");
            SetCursor(CursorType.None);
        }      

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (var hit in hits)
            {
                IRaycastable [] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (var raycastable in raycastables)
                {                 
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }                            
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit [] raycastHits =  Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[raycastHits.Length];
            for(int i=0; i< raycastHits.Length; i++)
            {
                distances[i] = raycastHits[i].distance;
            }
            Array.Sort(distances, raycastHits);
            return raycastHits;
        }

        private bool InteractWithMovement()
        {                            
            RaycastHit hit;
            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                if (mover.CanMoveTo(hit.point) == false)
                {
                    return false;
                }
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(hit.point, 1);               
                }             
                if (RaycastNavmesh(hit.point))
                {
                    SetCursor(CursorType.Movement);
                    return true;
                }            
            }
            return false;
        }

        private bool RaycastNavmesh(Vector3 position)
        {
            NavMeshHit hit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(position, out hit, maxNavhMeshProjectionDistance, NavMesh.AllAreas);
            return hasCastToNavMesh;
        }             

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var item in cursorMappings)
            {
                if (item.type == type) return item;
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
