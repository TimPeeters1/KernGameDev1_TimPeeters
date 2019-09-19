using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Player player;
    Camera mainCam;

    [SerializeField] Transform horizontalTurret;
    [SerializeField] Transform verticalTurret;

    private void Start()
    {
        player = Player.Instance;
        mainCam = player.cameraObject.GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        Ray _camRay = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 _targetPosition = _camRay.GetPoint(2000f);

        Debug.DrawLine(horizontalTurret.transform.position, _targetPosition, Color.red);

        Vector3 _hLookPosition = new Vector3(_targetPosition.x, 0, _targetPosition.z);
        Vector3 _vLookPosition = new Vector3(0, _targetPosition.y, 0);

        Debug.DrawLine(horizontalTurret.transform.position, _hLookPosition, Color.blue);
        //Debug.DrawLine(verticalTurret.transform.position, _vLookPosition, Color.green);

        Quaternion _newHRotation = Quaternion.LookRotation(_hLookPosition);
        horizontalTurret.rotation = Quaternion.Slerp(horizontalTurret.rotation, _newHRotation, Time.deltaTime * 100f);

        Quaternion _newVRotation = Quaternion.LookRotation(_targetPosition);
        verticalTurret.rotation = Quaternion.Slerp(verticalTurret.rotation, _newVRotation, Time.deltaTime * 10f);

    }
}
