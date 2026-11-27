using UnityEngine;

public class OnTriggerActivate : MonoBehaviour
{

    public GameObject ObjectToActivate;

    public void Start()
    {
        ObjectToActivate.SetActive(false);
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectToActivate.SetActive(true);
        }
    }
}
