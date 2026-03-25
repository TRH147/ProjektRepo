using UnityEngine;

public class TargetSetter : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        target.position = transform.position;    
    }
}
