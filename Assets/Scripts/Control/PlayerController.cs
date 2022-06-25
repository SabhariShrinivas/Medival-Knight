
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
        [SerializeField] private float maxNavPathLengeth = 10f;

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
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
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
            NavMeshPath path = new NavMeshPath();
            bool haspath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathComplete) return false; // if navmesh is not reachable, for example navmesh present on top of a flat surface such as roof of a building, then player should not be able to move
            if (GetPathLength(path) >= maxNavPathLengeth) return false;
            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float distance = 0;
            if (path.corners.Length < 2) return distance;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i], path.corners[i+1]);
            }
            return distance;
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
