using UnityEngine;
using System.Collections;

public class FinishingConditions : LevelConditions {

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.player))
            ++players_at_end_level;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.player))
            --players_at_end_level;
    }
}
