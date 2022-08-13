using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthScript : MonoBehaviour
{
    private EnemyAnimator enemy_Anim;
    private NavMeshAgent navAgent;
    private EnemyController enemy_Controller;

    public float health = 100f;

    public bool is_Player, is_Boar, is_Cannibal;

    private bool is_Dead;

    private EnemyAudio enemyAudio;

    private PlayerStats player_stats;
    
    // Start is called before the first frame update
    void Start()
    {
        
        if(is_Boar || is_Cannibal) {
            enemy_Anim = GetComponent<EnemyAnimator>();
            enemy_Controller = GetComponent<EnemyController>();
            navAgent = GetComponent<NavMeshAgent>();

            enemyAudio = GetComponentInChildren<EnemyAudio>();
        }

        if(is_Player) {
            player_stats = GetComponent<PlayerStats>();
        }

    }

    // Update is called once per frame
    public void ApplyDamage(float damage) {

        if(is_Dead) {
            return;
        }
         
        health -= damage;

        if(is_Player) {
            player_stats.Display_HealthStats(health);
        }

        if(is_Boar || is_Cannibal) {
           
            if(enemy_Controller.Enemy_State == EnemyState.PATROL) {
                enemy_Controller.chase_Distance = 50f;
            }
        }

        if(health <= 0f) {
            PlayerDied();

            is_Dead = true;
        }


    }

    void PlayerDied() {

        if(is_Cannibal) {
   
            navAgent.velocity = Vector3.zero;
            navAgent.isStopped = true;
            enemy_Controller.enabled = false;

            enemy_Anim.Dead();
            Destroy(gameObject, 3f);
            StartCoroutine(DeadSound());

            EnemyManager.instance.EnemyDied(true);

        }

        if(is_Boar) {

            navAgent.velocity = Vector3.zero;
            navAgent.isStopped = true;
            enemy_Controller.enabled = false;

            enemy_Anim.Dead();

            StartCoroutine(DeadSound());
            Destroy(gameObject, 3f);

            EnemyManager.instance.EnemyDied(false);

        }

        if(is_Player) {

            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tags.ENEMY_TAG);

            for (int i = 0; i < enemies.Length; i++) {
                enemies[i].GetComponent<EnemyController>().enabled = false;
            }

            EnemyManager.instance.StopSpawning();
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerAttack>().enabled = false;
            GetComponent<WeaponManager>().GetCurrentSelectedWeapon().gameObject.SetActive(false);

            if(tag == Tags.PLAYER_TAG) {
                Invoke("RestartGame", 3f);
            } else {
                Invoke("TurnOffGameObject", 3f);
            }


        }

        void RestartGame() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

        void TurnOffGameObject() {
            gameObject.SetActive(false);
        }

        IEnumerator DeadSound() {
            yield return new WaitForSeconds(0.3f);
            enemyAudio.Play_DeadSound();
        }

    }
        
    
}
