using UnityEngine;

public class MessSpawn : MonoBehaviour
{
 
    public GameObject EndroitOuSpawner;
    public GameObject ObjetACreer;
    public string TagDeObjetACreer;

    private void Start()
    {
        ObjetACreer.SetActive(false); // Ensure the object is inactive at the start
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == TagDeObjetACreer)
        {
           ObjetACreer.SetActive(true); 
        }
    }
}
