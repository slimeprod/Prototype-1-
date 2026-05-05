using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    public GameObject particlEsystem;
    public string effectType = "Zooming";
    void Update()
    {
        if (effectType == "Zooming")
        {
            particlEsystem.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, transform.rotation.y, 0f);
        }
    }
}
