using UnityEngine;

public class EdgeTeleport : MonoBehaviour
{
    private Spawner spawner;

    private void Start()
    {
        spawner = FindObjectOfType<Spawner>();
    }

    void Update()
    {
        WrapPosition();
    }

    void WrapPosition()
    {
        Vector3 newPosition = transform.position;

        if (transform.position.x < spawner.minX)
            newPosition.x = spawner.maxX;
        else if (transform.position.x > spawner.maxX)
            newPosition.x = spawner.minX;

        if (transform.position.z < spawner.minZ)
            newPosition.z = spawner.maxZ;
        else if (transform.position.z > spawner.maxZ)
            newPosition.z = spawner.minZ;

        transform.position = newPosition;
    }
}
