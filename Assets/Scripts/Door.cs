using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public bool isLocked = false;
    public bool autoClose = false;
    public float closeDelay = 3f;
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip lockedSound;
    
    private bool isOpen = false;
    private float currentAngle = 0f;
    private Vector3 initialRotation;
    private AudioSource audioSource;
    private Coroutine closeCoroutine;
    
    void Start()
    {
        initialRotation = transform.eulerAngles;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        // Smooth door animation
        if (isOpen && currentAngle < openAngle)
        {
            currentAngle += openSpeed * Time.deltaTime * 60f;
            currentAngle = Mathf.Min(currentAngle, openAngle);
            transform.eulerAngles = new Vector3(initialRotation.x, initialRotation.y + currentAngle, initialRotation.z);
        }
        else if (!isOpen && currentAngle > 0)
        {
            currentAngle -= openSpeed * Time.deltaTime * 60f;
            currentAngle = Mathf.Max(currentAngle, 0);
            transform.eulerAngles = new Vector3(initialRotation.x, initialRotation.y + currentAngle, initialRotation.z);
        }
    }
    
    public void ToggleDoor()
    {
        if (isLocked)
        {
            if (audioSource != null && lockedSound != null)
            {
                audioSource.PlayOneShot(lockedSound);
            }
            return;
        }
        
        isOpen = !isOpen;
        
        if (isOpen)
        {
            if (closeCoroutine != null)
            {
                StopCoroutine(closeCoroutine);
                closeCoroutine = null;
            }
            
            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
            
            if (autoClose)
            {
                closeCoroutine = StartCoroutine(AutoClose());
            }
        }
        else
        {
            if (audioSource != null && closeSound != null)
            {
                audioSource.PlayOneShot(closeSound);
            }
        }
    }
    
    IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(closeDelay);
        isOpen = false;
        
        if (audioSource != null && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }
    }
    
    public void Unlock()
    {
        isLocked = false;
    }
    
    public void Lock()
    {
        isLocked = true;
    }
}