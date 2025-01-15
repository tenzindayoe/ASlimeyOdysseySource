using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    
    public AudioMixer masterMixer;
    [HideInInspector]
    public AudioMixerGroup musicMixerGroup;
    [HideInInspector]
    public AudioMixerGroup sfxMixerGroup;
    [HideInInspector]
    public AudioMixerGroup envMixerGroup; 
    [HideInInspector]
    public AudioMixerGroup UI_SFXMixerGroup;
    [HideInInspector]
    public AudioMixerGroup DialogueMixerGroup;

    [Header("General Audio Clips")]
    public AudioClip walkSound; 
    public AudioClip jumpSound; 

    public AudioClip fallSound; 
    public AudioClip waterGunFireSound; 
    public AudioClip waterGunChargeSound; 
    public AudioClip[] damageSound = new AudioClip[3];



    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private const int PoolSize = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
            
        }
        else
        {
            Destroy(gameObject);
        }
        musicMixerGroup = masterMixer.FindMatchingGroups("Music")[0];
        sfxMixerGroup = masterMixer.FindMatchingGroups("SFX")[0];
        envMixerGroup = masterMixer.FindMatchingGroups("Environment")[0];
        UI_SFXMixerGroup = masterMixer.FindMatchingGroups("UI_SFX")[0];
        DialogueMixerGroup = masterMixer.FindMatchingGroups("Dialogue")[0];
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        //check if all the mixer gro    ups are assigned
        if(musicMixerGroup == null || sfxMixerGroup == null || envMixerGroup == null || UI_SFXMixerGroup == null || DialogueMixerGroup == null){
            Debug.LogError("One or more of the mixer groups are not assigned in the GameAudioManager. Please assign all the mixer groups in the inspector");
        }
    }
    void Start(){
        
    }

    private void InitializePool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            AudioSource source = CreateAudioSource("PooledAudioSource");
            source.gameObject.SetActive(false);
            audioSourcePool.Enqueue(source);
        }
    }

    private AudioSource CreateAudioSource(string name)
    {
        GameObject audioGO = new GameObject(name);
        audioGO.transform.SetParent(transform);
        return audioGO.AddComponent<AudioSource>();
    }

    public AudioSource GetAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }
        return CreateAudioSource("DynamicAudioSource");
    }

    public void ReturnAudioSource(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
    }

    //must not be used for moving audio sources

    public void PlayAudio(AudioClip clip, AudioMixerGroup group, bool spatial = false, Vector3? position = null)
    {
        if (clip == null || group == null) return;

        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.outputAudioMixerGroup = group;
        source.spatialBlend = spatial ? 1f : 0f;

        if (position.HasValue) source.transform.position = position.Value;

        source.Play();
        StartCoroutine(ReturnAfterPlay(source, clip.length));
    }

    private System.Collections.IEnumerator ReturnAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        ReturnAudioSource(source);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
    }
}
