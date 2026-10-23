using UnityEngine;

public class MovingObjects : MonoBehaviour
{
    public GameObject ObjectToMove;
    public GameObject TargetPosition;
    public float SpeedMultiplier = 1;

    private bool isMoving = false;
    private void OnTriggerEnter(Collider other)
    {   
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, TargetPosition.transform.position, Time.deltaTime * SpeedMultiplier);
        }

        if(ObjectToMove.transform.position == TargetPosition.transform.position)
        {
            isMoving = false;
        }
    }
}
