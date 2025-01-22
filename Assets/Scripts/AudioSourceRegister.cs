using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceRegister : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;
    public bool isMusic = false; // Set this in the Inspector to true for music sources, false for SFX

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        if (source == null) yield break;

        if (isMusic)
        {
            AudioManager.Instance?.RegisterMusicSource(source);
        }
        else
        {
            AudioManager.Instance?.RegisterSFXSource(source);
        }
    }
}
