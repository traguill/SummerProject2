using UnityEngine;
using System.Collections;

public class CameraDetection : MonoBehaviour {

    new private CameraController camera;

    void Awake()
    {
        camera = GetComponentInParent<CameraController>();
        camera.alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
        camera.last_spotted_position = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<LastSpottedPosition>();        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.player) && !camera.alarm_system.isAlarmActive())
        {
            camera.alarm_system.SetAlarm(ALARM_STATE.ALARM_ON);
            camera.last_spotted_position.LastPosition = other.transform.position;
            camera.ChangeStateTo(camera.following_state);            
        }           
    }
}
