using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Damager : MonoBehaviour
{
    public ParticleSystem bloodSpatter;
    public float knockBack = 4f;
    private GameObject player;
    [SerializeField] float amountOfDamage = 10f;
    public IEnumerator DamageRoutine()
    {
        bloodSpatter.Play();
        yield return new WaitWhile(() => bloodSpatter.isPlaying);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            collision.gameObject.GetComponent<PlayerSettings>().DamagePlayer(amountOfDamage); //damage player
            StartCoroutine(DamageRoutine()); //runs a damage routine, a sequence of visual and audio effects    
        }
        else
        {
            player = null;
        }
    }
}
