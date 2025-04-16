using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float bobSpeed = 1f;
    public float bobHeight = 0.5f;
    public Light itemLight;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
        
        if (itemLight != null)
        {
            itemLight.intensity = 1.5f;
        }
    }
    
    void Update()
    {
        // Rotate the collectible
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Pulsate light if available
        if (itemLight != null)
        {
            itemLight.intensity = 1.5f + Mathf.Sin(Time.time * 2f) * 0.5f;
        }
    }
}