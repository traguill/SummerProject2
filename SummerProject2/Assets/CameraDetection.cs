using UnityEngine;
using System.Collections;

public class CameraDetection : MonoBehaviour {

    private AlarmSystem alarm_system;

    void Awake()
    {
        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.player))
            alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);
    }
}
