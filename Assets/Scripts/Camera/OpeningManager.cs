using UnityEngine;
using UnityEngine.Playables;

public class OpeningManager : MonoBehaviour
{
    public PlayableDirector timelineDirector;
    public CockroachMove cockroachMove;
    public AllGameManager allGameManager;
    public CameraLogic3D cameraLogic3D;
    public CameraViewToggle cameraViewToggle;

    void Start()
    {
        // 鎖定玩家與計時
        cockroachMove.SetCanMove(false);
        allGameManager.isTimerRunning = false;
        AnimationEventReceiver.prepared = false;

        // 讓 CameraLogic3D 交出控制權給 Timeline
        cameraLogic3D.overriddenByTimeline = true;

        // 停止 CameraViewToggle 原本的開場 Transition
        // （改由 Timeline 結束後手動觸發）
        cameraViewToggle.StopAllCoroutines();

        timelineDirector.stopped += OnTimelineFinished;
        timelineDirector.Play();
    }

    void OnTimelineFinished(PlayableDirector director)
    {
        cameraLogic3D.overriddenByTimeline = false;

        cockroachMove.SetCanMove(true);
        allGameManager.isTimerRunning = true;
        AnimationEventReceiver.prepared = true;

        // Timeline 結束後再執行原本的入場 Transition
        //StartCoroutine(cameraViewToggle.StartCoroutineFromOutside());
    }
}