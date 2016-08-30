using UnityEngine;
using System.Collections;

public enum ALARM_STATE : byte
{
    ALARM_OFF,
    ALARM_ON
};

public class AlarmSystem : MonoBehaviour
{
    public float alarm_max_duration;      // Time (in seconds) that alarm is on
    public float alarm_timer;        
    private ALARM_STATE alarm_state;      // Alarm state: ON or OFF
    
    // Called before first start function
    void Awake()
    {
        alarm_state = ALARM_STATE.ALARM_OFF;        
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(alarm_state == ALARM_STATE.ALARM_ON)
        {
            if (alarm_timer < alarm_max_duration)
            {
                alarm_timer += Time.deltaTime;
            }
            else
            {
                alarm_state = ALARM_STATE.ALARM_OFF;
                alarm_timer = 0.0f;
            }
        }

        // Debug
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SetAlarm(ALARM_STATE.ALARM_ON);     

    }

    public void SetAlarm(ALARM_STATE _alarm_state)
    {
        alarm_state = _alarm_state;
        if (_alarm_state == ALARM_STATE.ALARM_ON)
            alarm_timer = 0.0f;
    }

    public bool isAlarmActive()
    {
        return (alarm_state == ALARM_STATE.ALARM_ON) ? true : false;
    }
}
