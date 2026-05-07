using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DetectPlayer : MonoBehaviour
{
    public GameObject playerObject;
    public LayerMask whatIsPlayer;
    public float eyeSight = 5f;
    [SerializeField] private bool playerLost = true;
    [SerializeField] private bool hasPlayerSpotted = false;
    IEnumerator ChaseRoutine()
    {
        playerLost = false;
        yield return new WaitWhile(() => hasPlayerSpotted);
        playerLost = true;

    }
    void Update()
    {
        Vector3 direction = playerObject.transform.position - transform.position;
        hasPlayerSpotted = Physics.Raycast(transform.position, direction.normalized, eyeSight, whatIsPlayer);
        Debug.DrawRay(transform.position, direction.normalized * eyeSight, Color.red);
        
        if (hasPlayerSpotted)
        {
            GetComponent<NavMeshAgent>().SetDestination(playerObject.transform.position);
            if (playerLost)
            {
                StartCoroutine(ChaseRoutine());
            }
        }
        Debug.Log(playerLost);
    }
}
