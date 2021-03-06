using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ShakeScreenController : MonoBehaviour
{
    public static ShakeScreenController Instance;
    Transform cameraTransform;
    bool shakeStart = false;
    [SerializeField] float shakeDuration = 0;
    [SerializeField] float shakeMagnitude = 0.7f;

    // A posição inicial do 
    Vector3 initialPosition;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        if (cameraTransform == null)
        {
            cameraTransform = GetComponent<Transform>();
        }
    }

    void Update()
    {
        if (shakeStart)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            StartCoroutine(CountShakeDuration());
        }
        else
        {
            transform.localPosition = initialPosition;
        }
    }

    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    IEnumerator CountShakeDuration()
    {
        yield return new WaitForSeconds(shakeDuration);
        shakeStart = false;
    }

    public void ShakeNow()
    {
        shakeStart = true;
    }
}
