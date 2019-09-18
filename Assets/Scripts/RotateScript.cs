//Script door Mark Meerssman

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour
{
    [SerializeField, Range(-360,360)] private float speedX;
    [SerializeField, Range(-360, 360)] private float speedY;
    [SerializeField, Range(-360, 360)] private float speedZ;

    [SerializeField] private bool rotateX;
    [SerializeField] private bool rotateY;
    [SerializeField] private bool rotateZ;

    private void Update() {

        float _scaledSpeedX = speedX * Time.deltaTime;
        float _scaledSpeedY = speedY * Time.deltaTime;
        float _scaledSpeedZ = speedZ * Time.deltaTime;

        transform.Rotate(rotateX ? _scaledSpeedX : 0, rotateY ? _scaledSpeedY : 0, rotateZ ? _scaledSpeedZ : 0);
    }
}
