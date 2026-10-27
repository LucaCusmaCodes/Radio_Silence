using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public GameObject ObjectToMove;
    public GameObject TargetPosition;
    public float SpeedMultiplier = 5f;


    private bool isMoving = false;
    private bool hasCompletedSE2 = false;

    private void Start()
    {
        
    }
    

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger if the Player walks in and it hasn't moved yet
        if (other.CompareTag("Player") && !isMoving && !hasCompletedSE2)
        {
            isMoving = true;
            print("hi");
        }
    }

    private void Update()
    {
        if (isMoving && ObjectToMove != null && TargetPosition != null)
        {
            Vector3 previousBusPos = ObjectToMove.transform.position;
            ObjectToMove.transform.position = Vector3.MoveTowards(
                ObjectToMove.transform.position,
                TargetPosition.transform.position,
                Time.deltaTime * SpeedMultiplier
            );

            // 5. Check if the bus has reached its final destination
            if (Vector3.Distance(ObjectToMove.transform.position, TargetPosition.transform.position) < 0.001f)
            {
                ObjectToMove.transform.position = TargetPosition.transform.position;
                isMoving = false;
                hasCompletedSE2 = true;
                StoreEvent2.SetMovementCompleted();
            }
        }
    }   
}
