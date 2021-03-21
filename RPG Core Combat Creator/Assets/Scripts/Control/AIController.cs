using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private PatrolPath patrolPath = null;
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointDwellTime = 5f;
        [Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;

        private GameObject player;
        private Fighter fighter;
        private Mover mover;
        private Health health;
        private LazyValue<Vector3> guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = 0;
        private int currentWaypointIndex = 0;

        private void Awake()
        {          
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if (health.IsDead()) return;
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player.gameObject))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();             
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspiciousBehaviour();
            }
            else
            {              
                PatrolBehaviour();
            }
            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            fighter.Attack(player);
        }
        private void SuspiciousBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {
                nextPosition = GetNextWaypoint();
            }
            mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            if (AtWayPoint())
            {
                if (timeSinceArrivedAtWaypoint > waypointDwellTime)
                {
                    currentWaypointIndex++;
                    timeSinceArrivedAtWaypoint = 0;
                }
                timeSinceArrivedAtWaypoint += Time.deltaTime;
            }
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetNextWaypoint());
            return distanceToWaypoint < waypointTolerance;    
        }
        private Vector3 GetNextWaypoint()
        {
            if (patrolPath == null) return Vector3.positiveInfinity;
            return patrolPath.GetWaypoint(currentWaypointIndex % patrolPath.transform.childCount);
        }
        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;                   
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
