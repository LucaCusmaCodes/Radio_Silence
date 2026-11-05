using UnityEngine;

public class OnTriggerLeave : MonoBehaviour
{
    public GameObject objectToActivate;
    public string targetTag = "Customer";

    private bool alreadyActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyActivated) return;

        if (other.CompareTag(targetTag))
        {
            alreadyActivated = true;
            objectToActivate.SetActive(true);
        }
    }
}