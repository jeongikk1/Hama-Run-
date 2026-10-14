using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float minX = 0f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10f);
            desiredPosition.x = Mathf.Max(desiredPosition.x, minX);
            transform.position = desiredPosition;
        }
    }
}