using UnityEngine;
using UnityEngine.UI;

public class camera : MonoBehaviour
{
    public Transform player; 
    public Vector3 offset; 

    void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}