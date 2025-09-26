using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayNightRotator : MonoBehaviour
{
    private EraManager eraManager;
    public RectTransform PE; // ���W�b��
    public RectTransform DE; // ���s�b��
    public RectTransform ME; // �j�����b��

    private float PEtoDE_Duration;
    private float DEtoME_Duration;

    void Start()
    {
        // ��l����
        eraManager = GameObject.Find("AllGameManager").GetComponent<EraManager>();
        PE.localRotation = Quaternion.Euler(0, 0, 0);     // �U
        DE.localRotation = Quaternion.Euler(0, 0, -180f); // �W
        ME.localRotation = Quaternion.Euler(0, 0, -180f); // �W
        PEtoDE_Duration = eraManager.eraValue.intervalPEToDE;
        DEtoME_Duration = eraManager.eraValue.intervalDEToME;
    }

    void Update()
    {
        if (AnimationEventReceiver.prepared)
        {
            AnimationEventReceiver.prepared = false; // ����ƱҰ�
            SetAlphaFull();
            StartCoroutine(RotateSequence());
        }
    }

    IEnumerator RotateSequence()
    {
        yield return RotateTwoOverTime(PE, 0f, -180f, DE, -180f, 0f, PEtoDE_Duration);

        yield return RotateTwoOverTime(DE, 0f, -180f, ME, -180f, 0f, DEtoME_Duration);
    }

    public IEnumerator RotateTwoOverTime(RectTransform first, float startZ1, float endZ1,
                                 RectTransform second, float startZ2, float endZ2,
                                 float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            first.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(startZ1, endZ1, t));
            second.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(startZ2, endZ2, t));

            yield return null;
        }

        // �T�O�̲ר��ץ��T
        first.rotation = Quaternion.Euler(0, 0, endZ1);
        second.rotation = Quaternion.Euler(0, 0, endZ2);
    }

    public void SetAlphaFull()
    {
        SetAlpha(1f);
    }

    private void SetAlpha(float alpha)
    {
        if (PE.TryGetComponent<Image>(out var peImg))
            peImg.color = new Color(peImg.color.r, peImg.color.g, peImg.color.b, alpha);
        if (DE.TryGetComponent<Image>(out var deImg))
            deImg.color = new Color(deImg.color.r, deImg.color.g, deImg.color.b, alpha);
        if (ME.TryGetComponent<Image>(out var meImg))
            meImg.color = new Color(meImg.color.r, meImg.color.g, meImg.color.b, alpha);
    }


}
