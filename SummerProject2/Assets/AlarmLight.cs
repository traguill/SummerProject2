using UnityEngine;
using System.Collections;

public class AlarmLight : MonoBehaviour
{
    public float fade_speed = 2f;            // How fast the light fades between intensities.
    public float high_intensity = 2f;        // The maximum intensity of the light whilst the alarm is on.
    public float low_intensity = 0.5f;       // The minimum intensity of the light whilst the alarm is on.
    public float change_margin = 0.2f;       // The margin within which the target intensity is changed.

    private Light alarm_light;              // The light GameObject that will be modified.
    private AlarmSystem alarm_system;
    private float targetIntensity;          // The intensity that the light is aiming for currently.

    void Awake()
    {
        // Getting light from Alarm component
        alarm_light = GetComponent<Light>();
        alarm_light.enabled = true;
        // When the level starts we want the light to be "off".
        alarm_light.intensity = 0f;
        // When the alarm starts for the first time, the light should aim to have the maximum intensity.
        targetIntensity = high_intensity;

        // EnemyFieldView script to access Alarm boolean.
        alarm_system = GameObject.FindGameObjectWithTag(Tags.game_controller).GetComponent<AlarmSystem>();
    }

    void Update()
    {
        // Using alarm state, we modulate light alarm intensity when the alarm is active...
        if(alarm_system.isAlarmActive())
        {
            // ... Lerp the light's intensity towards the current target.
            alarm_light.intensity = Mathf.Lerp(alarm_light.intensity, targetIntensity, fade_speed * Time.deltaTime);

            // Check whether the target intensity needs changing and change it if so.
            CheckTargetIntensity();   
        }
        else
        {
            // Otherwise fade the light's intensity to zero.
            alarm_light.intensity = Mathf.Lerp(alarm_light.intensity, 0f, fade_speed * Time.deltaTime);          
        }           
    }

    void CheckTargetIntensity()
    {
        // If the difference between the target and current intensities is less than the change margin...
        if (Mathf.Abs(targetIntensity - alarm_light.intensity) < change_margin)
        {
            // ... if the target intensity is high...
            if (targetIntensity == high_intensity)
                // ... then set the target to low.
                targetIntensity = low_intensity;
            else
                // Otherwise set the targer to high.
                targetIntensity = high_intensity;
        }
    }
}
