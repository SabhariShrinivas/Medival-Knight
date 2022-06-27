
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover mover;
        private CombatTarget target;
        Health health;

        [System.Serializable]
        struct cursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }
        [SerializeField] cursorMapping[] cursorMappings;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float rayCastRadius = 1f;


        // Start is called before the first frame update
        void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithUI())
            {
                SetCursor(CursorType.UI);
                return;
            }
            if (health.isDead())
            {
                SetCursor(CursorType.none);
                return;
            }
            if (InteractWithComponent()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.none);


        }

     
        private bool InteractWithUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
               IRaycastable[] raycastable = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastableItem in raycastable)
                {
                    if (raycastableItem.HandleRaycast(this))
                    {
                        SetCursor(raycastableItem.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }
        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), rayCastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }
        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!mover.CanMoveTo(target))
                {
                    return false;
                }
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.movement);
                return true;
            }
            return false;          
        }
        private bool RaycastNavMesh(out Vector3 target)
        {
            RaycastHit hitInfo;
            target = new Vector3();
            bool hasHit = Physics.Raycast(GetMouseRay(), out hitInfo);
            if (!hasHit) return false;
            NavMeshHit hit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hitInfo.point, out hit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;
            target = hit.position; 
            return true;
        }

       

        private void SetCursor(CursorType type)
        {
            cursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        private cursorMapping GetCursorMapping(CursorType type)
        {
            foreach(cursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
 
    }
}
