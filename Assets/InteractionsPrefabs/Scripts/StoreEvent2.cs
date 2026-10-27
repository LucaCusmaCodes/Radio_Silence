using UnityEngine;

public class StoreEvent2 : MonoBehaviour
{
    public GameObject Player;
    public GameObject Can;
    public GameObject CustomerRequest;
    public GameObject DeclineRequest;
    public GameObject CustomerThanks;
    public GameObject Manager;
    private static bool movementCompleted = false;
    private bool done = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Can.SetActive(false);
        CustomerRequest.SetActive(false);
        DeclineRequest.SetActive(false);
        CustomerThanks.SetActive(false);
        Manager.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        {
            if(movementCompleted && !done && GameManager.Instance.internalScore < 6)
            {
                Can.SetActive(true);
                CustomerRequest.SetActive(true);
                DeclineRequest.SetActive(true);
                float distance = Vector3.Distance(Player.transform.position, Can.transform.position);
                if(distance<=5f && Input.GetKeyDown(KeyCode.E))
                {
                    Can.SetActive(false);
                    CustomerRequest.SetActive(false);
                    DeclineRequest.SetActive(false);
                    CustomerThanks.SetActive(true);
                    Manager.SetActive(true);
                    done = true;
                }
            }
        }
    }

    public static void SetMovementCompleted()
    {
        movementCompleted = true;
    }
}
