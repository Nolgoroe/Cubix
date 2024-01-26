using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sounds
{
    GameStart, WaveStart, PlaceTower
}

[System.Serializable]
public class AudioSourceCombo
{
    public AudioSource source;
    public float maxVolume;
    public float timeToFadeVolume;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] List<AudioSourceCombo> allAudioSources; 
    private Dictionary<Sounds, AudioSourceCombo> SoundToAudioSourceDict;

    private void Awake()
    {
        Instance = this;

        SoundToAudioSourceDict = new Dictionary<Sounds, AudioSourceCombo>();

        for (int i = 0; i < allAudioSources.Count; i++)
        {
            SoundToAudioSourceDict.Add((Sounds)i, allAudioSources[i]);
        }
    }

    private IEnumerator ActivateSoundTimed(Sounds sound)
    {
        float time = SoundToAudioSourceDict[sound].source.clip.length;

        SoundToAudioSourceDict[sound].source.volume = 0;

        SoundToAudioSourceDict[sound].source.gameObject.SetActive(true);

        LeanTween.value(SoundToAudioSourceDict[sound].source.gameObject, 0, SoundToAudioSourceDict[sound].maxVolume, SoundToAudioSourceDict[sound].timeToFadeVolume).setOnUpdate((float val) =>
        {
            SoundToAudioSourceDict[sound].source.volume = val;
        });

        SoundToAudioSourceDict[sound].source.gameObject.SetActive(true);

        yield return new WaitForSeconds(time);

        SoundToAudioSourceDict[sound].source.gameObject.SetActive(false);
    }


    private void DisableAudioSourceGameobject(AudioSource source)
    {
        source.gameObject.SetActive(false);
    }


    public void CallActivateSoundTimed(Sounds sound)
    {
        StartCoroutine(ActivateSoundTimed(sound));
    }

    public void ActivateSoundImmediate(Sounds sound)
    {
        SoundToAudioSourceDict[sound].source.Play();

    }

    public void StopSound(Sounds sound)
    {
        SoundToAudioSourceDict[sound].source.volume = 1;

        SoundToAudioSourceDict[sound].source.gameObject.SetActive(true);

        LeanTween.value(SoundToAudioSourceDict[sound].source.gameObject, SoundToAudioSourceDict[sound].source.volume, 0, SoundToAudioSourceDict[sound].timeToFadeVolume)
            .setOnComplete(() => DisableAudioSourceGameobject(SoundToAudioSourceDict[sound].source))
            .setOnUpdate((float val) =>
            {
                SoundToAudioSourceDict[sound].source.volume = val;
            });
    }

}
