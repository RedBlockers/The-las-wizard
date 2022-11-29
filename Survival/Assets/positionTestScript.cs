using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class positionTestScript : MonoBehaviour
{

    [SerializeField]
    private Transform objectTransform;


    // Update is called once per frame
    void Update()
    {
        Vector3 wantedPos = Camera.main.WorldToScreenPoint(objectTransform.position);
        transform.position = wantedPos;
    }
}
