using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
    public Slider healthBar;
    public float health = 0L;
    public MoveHandler moveHandler;
    public string playerType;
    public long minHealth = 0L;
    public long maxHealth = 100L;
    void Start()
    {
        health = (playerType == "Youtuber" ? 100L : 60L);
        healthBar.maxValue = health;
    }
    void Update()
    {       
        healthBar.value = Mathf.Lerp(healthBar.value, health, 10f * Time.unscaledDeltaTime);
        health = Mathf.Clamp(health, 0f, 100f);
        if (health == 0L)
        {
            KillPlayer();
        }
        if (moveHandler.isSafe())
        {
            health += 1f;
        }
    }
    void KillPlayer()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void DamagePlayer(float amount)
    {
        if (!moveHandler.isSafe())
        {
            health -= amount; //when calling this method, use the f prefix
        }
    }
}
