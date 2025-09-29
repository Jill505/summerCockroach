using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();
    private List<AudioSource> audioSources = new List<AudioSource>();
    private int maxSources = 10;

    

    [RuntimeInitializeOnLoadMethod]
    static void InitOnStart()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("SoundManager");
            instance = obj.AddComponent<SoundManager>();
            DontDestroyOnLoad(obj);
        }
    }

    void Awake()
    {
        for (int i = 0; i < maxSources; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audioSources.Add(source);
        }

        caveHeatWarningSource = gameObject.AddComponent<AudioSource>(); 
        caveHeatWarningSource.loop = true; 
        caveHeatWarningSource.playOnAwake = false; 
        caveHeatWarningSource.volume = 0f;

        hungerWarningSource = gameObject.AddComponent<AudioSource>(); 
        hungerWarningSource.loop = true; 
        hungerWarningSource.playOnAwake = false; 
        hungerWarningSource.volume = 0f;

        walkSource = gameObject.AddComponent<AudioSource>(); 
        walkSource.loop = true; 
        walkSource.playOnAwake = false; 
        walkSource.volume = 1f;

        AudioClip[] clips = Resources.LoadAll<AudioClip>("Art/Audio/Sounds");
        foreach (AudioClip clip in clips)
        {
            soundDict[clip.name] = clip;
        }
    }

    public static void Play(string name)
    {
        if (instance == null || instance.soundDict == null) return;

        if (instance.soundDict.TryGetValue(name, out AudioClip clip))
        {
            // 找一個沒在播的 AudioSource 來播放
            foreach (AudioSource source in instance.audioSources)
            {
                if (!source.isPlaying)
                {
                    source.PlayOneShot(clip);
                    return;
                }
            }

            // 如果都在播，強制用第一個播（可自訂行為）
            instance.audioSources[0].PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"音效 '{name}' 不存在於 Resources/Art/Audio/Sounds！");
        }
    }

    public static void StopAllSounds()
    {
        if (instance == null) return;

        // 停掉所有池子裡的 AudioSource
        foreach (AudioSource source in instance.audioSources)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }

        // 停掉警告音效
        StopCaveHeatWarning();
        StopHungerWarning();

        // 停掉行走音效
        StopWalkSound();
    }

    private AudioSource caveHeatWarningSource;
    private Coroutine caveHeatCoroutine;
    public static void StartCaveHeatWarning(string name, float fadeDuration = 2f)
    {
        if (instance == null) return; if (instance.soundDict.TryGetValue(name, out AudioClip clip))
        {
            if (instance.caveHeatWarningSource.isPlaying) return; 
            instance.caveHeatWarningSource.clip = clip; 
            instance.caveHeatWarningSource.volume = 0f; 
            instance.caveHeatWarningSource.Play(); 
            if (instance.caveHeatCoroutine != null) instance.StopCoroutine(instance.caveHeatCoroutine); 
            instance.caveHeatCoroutine = instance.StartCoroutine(instance.FadeInLoop(fadeDuration)); 
        } 
        else { Debug.LogWarning($"警告音效 '{name}' 不存在於 Resources/Art/Audio/Sounds！"); } 
    } 
    public static void StopCaveHeatWarning() 
    { 
        if (instance == null) return; 
        if (instance.caveHeatCoroutine != null) 
        { 
            instance.StopCoroutine(instance.caveHeatCoroutine); 
            instance.caveHeatCoroutine = null; 
        } 
        if (instance.caveHeatWarningSource.isPlaying) 
        { 
            instance.caveHeatWarningSource.Stop(); 
        } 
    } 
    private IEnumerator FadeInLoop(float duration) 
    {
        float timer = 0f; 
        while (timer < duration) 
        { 
            timer += Time.deltaTime; caveHeatWarningSource.volume = Mathf.Lerp(0f, 1f, timer / duration); 
            yield return null; 
        } 
        caveHeatWarningSource.volume = 1f; 
    }

    private AudioSource hungerWarningSource;
    private Coroutine hungerCoroutine;
    public static void StartHungerWarning(string name, float fadeDuration = 2f) 
    { 
        if (instance == null) return; 
        if (instance.soundDict.TryGetValue(name, out AudioClip clip)) 
        { 
            if (instance.hungerWarningSource.isPlaying) return; 
            instance.hungerWarningSource.clip = clip; 
            instance.hungerWarningSource.volume = 0f; 
            instance.hungerWarningSource.Play(); 
            if (instance.hungerCoroutine != null) instance.StopCoroutine(instance.hungerCoroutine); 
            instance.hungerCoroutine = instance.StartCoroutine(instance.FadeIn(instance.hungerWarningSource, fadeDuration)); 
        } 
    }

    
    public static void StopHungerWarning() 
    { 
        if (instance == null) return; 
        if (instance.hungerCoroutine != null) 
        { 
            instance.StopCoroutine(instance.hungerCoroutine); 
            instance.hungerCoroutine = null; 
        } 
        if (instance.hungerWarningSource.isPlaying) instance.hungerWarningSource.Stop(); 
    } 

    private AudioSource walkSource; 
    private float normalPitch = 2.0f; 
    private float dashPitch = 3.5f;

    public static void PlayWalkSound(string name, bool isDashing) 
    { 
        
        if (instance == null) return;
        if (instance.soundDict.TryGetValue(name, out AudioClip clip)) 
        { 
            if (!instance.walkSource.isPlaying) 
            { 
                instance.walkSource.clip = clip; 
                instance.walkSource.Play(); 
            } 
            instance.walkSource.pitch = isDashing ? instance.dashPitch : instance.normalPitch; 
        } 
    }
    public static void StopWalkSound() 
    { 
        if (instance == null) return; 
        if (instance.walkSource.isPlaying) instance.walkSource.Stop(); 
    } 
    private IEnumerator FadeIn(AudioSource source, float duration) 
    { 
        float timer = 0f; 
        while (timer < duration) 
        { timer += Time.deltaTime; 
            source.volume = Mathf.Lerp(0f, 1f, timer / duration); 
            yield return null; 
        } 
        source.volume = 1f; 
    }
}

