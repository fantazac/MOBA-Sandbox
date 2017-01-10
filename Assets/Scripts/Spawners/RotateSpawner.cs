using UnityEngine;
using System.Collections;

public class RotateSpawner : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 50);
    }
}
