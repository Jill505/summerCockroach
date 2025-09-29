using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    private AudioSource bgmSource;

    public static float _volume = 0.7f;
    public static float Volume
    {
        get => _volume;
        set
        {
            _volume = Mathf.Clamp01(value); // 限制在 0~1
            instance.bgmSource.volume = _volume;

        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("BGMManager");
            instance = obj.AddComponent<BGMManager>();
            DontDestroyOnLoad(obj);
        }
    }

    void Awake()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = _volume;

        AudioClip[] clips = Resources.LoadAll<AudioClip>("Art/Audio/BGM");
        foreach (var clip in clips)
        {
            bgmDict[clip.name] = clip;
        }
    }

    public static void Play(string name)
    {
        if (instance == null || instance.bgmDict == null) return;

        if (instance.bgmDict.TryGetValue(name, out AudioClip clip))
        {
            if (instance.bgmSource.clip != clip)
            {
                instance.bgmSource.clip = clip;
                instance.bgmSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"BGM『{name}』不存在於 Resources/Art/Audio/BGM/");
        }
    }

    public static void Pause()
    {
        if (instance != null && instance.bgmSource.isPlaying)
        {
            instance.bgmSource.Pause();
        }
    }

    public static void Resume()
    {
        if (instance != null && !instance.bgmSource.isPlaying)
        {
            instance.bgmSource.Play();
        }
    }

    public static void Stop()
    {
        if (instance != null)
        {
            instance.bgmSource.Stop();
            instance.bgmSource.clip = null;
        }
    }
}
