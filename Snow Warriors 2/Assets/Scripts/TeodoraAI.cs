using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TeodoraAI : Player {
   public float CloseDistance = 10.0f;
   public Transform BaseTransform;
   private bool isTaskFinished;

   private void Start() {
      isTaskFinished = true;
   }

   private void Update() {
      if (isTaskFinished) {
         StartCoroutine(ExecuteTask());
      }
   }

   private IEnumerator ExecuteTask() {
      isTaskFinished = false;
      if (playerHealth < 50 && FindCollectable("Health", out var health)) {
         yield return GetCollectable(health);
      } else if (playerStamina < 5 && FindCollectable("Stamina", out var stamina)) {
         yield return GetCollectable(stamina);
      } else if(EnemyFiring()) {
         yield return DodgeSnowballs();
      } else if (FindClosestEnemy(out var closestEnemy, out var squareDistance)) {
         if (squareDistance > CloseDistance * CloseDistance) {
            if (!holdingLargeSnowball) CreateLargeSnowball(); 
            yield return FireSnowball(closestEnemy);
         } else {
            if (playerSmallSnowballCount > 0) {
               yield return FireSnowball(closestEnemy);
            } else {
               yield return GoToBaseAndMakeSnowball();
            }
         }
      } else {
         yield return RandomWalk(3.0f);
      }

      yield return new WaitUntil(() => isTaskFinished);
   }

   

   private Vector3 GetDodgeDirection(GameObject firstSnowball) {
      var playerTransform = transform;
      transform.LookAt(firstSnowball.transform);
      var position = playerTransform.position;
      var playerPlane = new Plane(playerTransform.right + position, position, playerTransform.up + position);
      var snowballDirection = firstSnowball.transform.forward;
      
      var ray = new Ray(firstSnowball.transform.position, snowballDirection);
      if (playerPlane.Raycast(ray, out var distance)) {
         var hitPoint = ray.GetPoint(distance);
         var sqrDist = (transform.position - hitPoint).sqrMagnitude;
         if (sqrDist > 5.0f) {
            // don't dodge, snowball won't hit
            return Vector3.zero;
         }

         var dodgeDirection = (transform.position - hitPoint).normalized;
         return dodgeDirection;
      }
      return Vector3.zero;
   }

   private bool EnemyFiring() {
      return snowballsInSight.Count > 0;
   }

   private bool FindClosestEnemy(out GameObject closestEnemy, out float distance) {
      var anyEnemy = enemiesInSight.Count > 0;
      if (!anyEnemy) {
         closestEnemy = null;
         distance = 0.0f;
         return false;
      }

      var enemies = enemiesInSight.Select(enemy => (enemy, (enemy.transform.position - transform.position).sqrMagnitude)).ToList();
      enemies.Sort((enemy1, enemy2) => enemy1.sqrMagnitude.CompareTo(enemy2.sqrMagnitude));
      closestEnemy = enemies[0].enemy;
      distance = enemies[0].sqrMagnitude;
      return true;
   }

   private bool FindCollectable(string type, out GameObject collectable) {
      if (type.Equals("Health", StringComparison.InvariantCulture)) {
         collectable = collectablesInRange.FirstOrDefault(obj => obj.GetComponent<HealthPickUp>());
         return collectable != null;
      }

      collectable = collectablesInRange.FirstOrDefault(obj => obj.GetComponent<StaminaPickUp>());
      return collectable != null;
   }

   private IEnumerator GetCollectable(GameObject collectable) {
      debugoutput = $"Getting collectable {collectable}";
      var success = navMeshAgent.SetDestination(collectable.transform.position);
      if (!success) {
         isTaskFinished = true;
         debugoutput = "None";
         yield break;
      }

      yield return new WaitUntil(() => collectable == null);
      isTaskFinished = true;
      debugoutput = "None";
   }

   private IEnumerator FireSnowball(GameObject enemy) {
      debugoutput = "FireSnowball";
      ThrowSnowball(enemy.transform.position);
      yield return new WaitForSeconds(1.5f);
      debugoutput = "None";
      isTaskFinished = true;
   }
   
   private IEnumerator GoToBaseAndMakeSnowball() {
      navMeshAgent.SetDestination(BaseTransform.position);
      debugoutput = "Returning to base";
      var timer = 0.0f;
      yield return new WaitUntil(() => {
         timer += Time.deltaTime;
         return navMeshAgent.remainingDistance < 0.5f || timer > 10.0f;
      });
      debugoutput = "Reached base, making snowballs";
      var createAmount = 6 - playerSmallSnowballCount;
      for (var i = 0; i < createAmount; i++) {
         CreateSmallSnowball();
      }
      isTaskFinished = true;
      debugoutput = "None";
   }

   private IEnumerator DodgeSnowballs() {
      debugoutput = "DodgeSnowballs";
      var firstSnowball = snowballsInSight[0];
      var dodgeDirection = GetDodgeDirection(firstSnowball);
      Dodge(transform.position + dodgeDirection * 2.0f);
      yield return new WaitForSeconds(3.0f);
      isTaskFinished = true;
      debugoutput = "None";
   }

   private IEnumerator RandomWalk(float maxDuration) {
      debugoutput = "RandomWalk";
      var positionWorks = false;
      do {
         var positionX = Random.Range(-40f, 40f);
         var positionZ = Random.Range(-40f, 40f);
         positionWorks = navMeshAgent.SetDestination(new Vector3(positionX, 0, positionZ));
      } while (!positionWorks);

      var timer = 0.0f;
      yield return new WaitUntil(() => {
         timer += Time.deltaTime;
         return timer >= maxDuration || navMeshAgent.remainingDistance < 1.0f || enemiesInSight.Count > 0;
      });
      debugoutput = "None";
      isTaskFinished = true;
   }
}

