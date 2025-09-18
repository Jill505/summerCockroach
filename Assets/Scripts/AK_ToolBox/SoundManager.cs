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
            // ��@�ӨS�b���� AudioSource �Ӽ���
            foreach (AudioSource source in instance.audioSources)
            {
                if (!source.isPlaying)
                {
                    source.PlayOneShot(clip);
                    return;
                }
            }

            // �p�G���b���A�j��βĤ@�Ӽ��]�i�ۭq�欰�^
            instance.audioSources[0].PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"���� '{name}' ���s�b�� Resources/Art/Audio/Sounds�I");
        }
    }
}
