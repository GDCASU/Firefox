using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Underwater : MonoBehaviour
{
    float aboveWater = 0f;
    float underwater = 1f;

    //enters the water
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FMODUnity.FMODStartMusic.music.setParameterByName("IsUnderwater", underwater);
        }
    }

    //exits the water
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FMODUnity.FMODStartMusic.music.setParameterByName("IsUnderwater", aboveWater);
        }
    }
}
