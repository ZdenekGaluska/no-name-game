using UnityEngine;

public class ArcherScript : MonoBehaviour
{
    public GameObject arrowPrefab;



    void FixedUpdate()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, transform.position, transform.rotation);
        ArrowScript arrowScript = arrowObj.GetComponent<ArrowScript>();
        arrowScript.ShootArrow(Vector2.up);
    }
}

