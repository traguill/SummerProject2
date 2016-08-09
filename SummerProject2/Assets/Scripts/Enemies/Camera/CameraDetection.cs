using UnityEngine;
using System.Collections;

public class CameraDetection : MonoBehaviour {

    private AlarmSystem alarm_system;
    new private CameraController camera;

    void Awake()
    {
        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
        camera = GetComponentInParent<CameraController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.player) && !alarm_system.isAlarmActive())
        {
            alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);
            camera.ChangeStateTo(camera.following_state);
        }           
    }
}
