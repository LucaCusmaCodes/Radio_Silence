using UnityEngine;

public class DeclineCustomer : MonoBehaviour
{
    public GameObject Can;
    public GameObject CustomerRequest;
    public GameObject DeclineRequest;
    public GameObject CustomerResponse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CustomerResponse.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(Can);
            CustomerRequest.SetActive(false);
            Destroy(DeclineRequest);
            CustomerResponse.SetActive(true);
            GameManager.Instance.ProcessChoice("Declined Customer Food",1);
            VignetteController.Instance.IncreaseDarkness();
            MusicManager.Instance.ApplyPitchDropStep();
        }
    }
}
