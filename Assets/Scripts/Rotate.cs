using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// for Item pickups to look cool
/// </summary>
public class Rotate : MonoBehaviour
{
    [SerializeField] float _rotationSpeed = 1;

    private void Start()
    {
        transform.Rotate(Vector3.forward * Random.Range(0,360));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * 360 * _rotationSpeed * Time.deltaTime);
    }
}
