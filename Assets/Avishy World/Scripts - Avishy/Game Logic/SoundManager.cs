using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum Sounds
{
    BuyItem, 
    CyberTroopSpawn, 
    DiceDisplayResult, 
    DiceEndRoll, 
    DiceRolling, 
    DiceStartRolling,
    ElectricBasicAttack,
    ElectricSpecialAttack,
    EnemyDies,
    EnemyEnterBase,
    EnemyHit,
    EnemySpawn,
    LazerTrapSpawn,
    LockDice,
    PlacingTower,
    RecieveResources,
    TimerTicking,
    TowerRecieveBuff,
    UIClick,
    WaveStart,
    GameplayBGM,
    PlannigPhaseBGM,
    ForgeDice,
    MapBGM

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
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SoundToAudioSourceDict = new Dictionary<Sounds, AudioSourceCombo>();

        for (int i = 0; i < allAudioSources.Count; i++)
        {
            SoundToAudioSourceDict.Add((Sounds)i, allAudioSources[i]);
        }

        SceneManager.activeSceneChanged += StopAllSound;
    }

    private void DisableAudioSourceGameobject(AudioSource source)
    {
        source.gameObject.SetActive(false);
    }

    public void PlaySoundFade(Sounds sound)
    {
        float time = SoundToAudioSourceDict[sound].source.clip.length;

        SoundToAudioSourceDict[sound].source.volume = 0;

        SoundToAudioSourceDict[sound].source.Play();

        LeanTween.value(SoundToAudioSourceDict[sound].source.gameObject, 0, SoundToAudioSourceDict[sound].maxVolume, SoundToAudioSourceDict[sound].timeToFadeVolume).setOnUpdate((float val) =>
        {
            SoundToAudioSourceDict[sound].source.volume = val;
        });
    }



    public void PlaySoundOneShot(Sounds sound)
    {
        SoundToAudioSourceDict[sound].source.PlayOneShot(SoundToAudioSourceDict[sound].source.clip, SoundToAudioSourceDict[sound].maxVolume);
    }
    public void PlaySoundNormal(Sounds sound)
    {
        SoundToAudioSourceDict[sound].source.Play();
    }
    public void PlaySoundIfInactive(Sounds sound)
    {
        if(!SoundToAudioSourceDict[sound].source.isPlaying)
        SoundToAudioSourceDict[sound].source.Play();
    }

    public void StopSoundFade(Sounds sound)
    {
        LeanTween.value(SoundToAudioSourceDict[sound].source.gameObject, SoundToAudioSourceDict[sound].source.volume, 0, SoundToAudioSourceDict[sound].timeToFadeVolume).setOnUpdate((float val) =>
        {
            SoundToAudioSourceDict[sound].source.volume = val;
        });
    }
    public void StopSound(Sounds sound)
    {
        if(SoundToAudioSourceDict[sound].source.isPlaying)
        SoundToAudioSourceDict[sound].source.Stop();
    }


    private void StopAllSound(Scene arg1, Scene arg2)
    {
        foreach (AudioSourceCombo combo in allAudioSources)
        {
            combo.source.Stop();
        }
    }
}
