using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }



    public Transform myPlanetToRotate;
    public float SpeedPerSecond;

    // Update is called once per frame
    void Update()
    {
        myPlanetToRotate.Rotate(Vector3.up, SpeedPerSecond * Time.deltaTime);

        
    }
}
