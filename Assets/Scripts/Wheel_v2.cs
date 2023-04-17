using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_v2 : MonoBehaviour
{
    public GameObject ViewingBox;
    public GameObject ReelStripPrefab;
    public GameObject Canvas;

    [Range(0.0f, 5.0f)]
    public float MOVEMENT_INCREMENT;

    private ReelStrip curReelStrip;
    private ReelStrip prevReelStrip;

    // Start is called before the first frame update
    void Start()
    {
        CreateAReelStrip(ReelStripPrefab);        
        SimulateGameRound();
    }

    private void CreateAReelStrip(GameObject reelStrip)
    {
        GameObject newReelStrip = Instantiate(reelStrip.gameObject, transform, false);        
        newReelStrip.transform.SetParent(Canvas.transform,true);
        curReelStrip = newReelStrip.GetComponent<ReelStrip>();
        EventManager.Instance.AddEventListener(this, curReelStrip, CustomEvent.Event, ReelStripMessageHandler);
        curReelStrip.Load(ViewingBox.transform.position.y);
        curReelStrip.name = "stripName: " + Time.realtimeSinceStartup;
    }

    private void SimulateGameRound()
    {
        SpinWheel();

        float timeAccumulate = 0;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.5f));

        timeAccumulate += 0.5f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 1f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 3f));

        timeAccumulate += 2f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 4f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 3f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 1f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.75f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.5f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.3f));

        timeAccumulate += 0.75f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.25f));

        timeAccumulate += 0.5f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.15f));

        timeAccumulate += 0.4f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.1f));

        timeAccumulate += 0.3f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.05f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.1f));

    }
    
    private void ReelStripMessageHandler(object sender, EventManagerEventArgs e)
    {
        MessageObject<string, object> ingressMsg = (MessageObject<string, object>)e.eventObject;
        string command = (string)ingressMsg[Commands.Command];
        switch (command)
        {
            case Commands.TargetReelSymbolReachedDestination:
                UpdateSpeedOfReelStrips(0f); //stop all reel strips
                ReelDoneMovingProcedures();
                break;
            default:
                Debug.LogError("No case found for: " + command);
                break;
        }
    }

    public virtual void SendMessageToParent(MessageObject<string, object> messageObject)
    {
        EventManagerEventArgs args = new EventManagerEventArgs();
        args.eventObject = messageObject;
        EventManager.Instance.DispatchEvent(this, CustomEvent.Event, args);
    }

    private void ReelDoneMovingProcedures()
    {
        prevReelStrip = curReelStrip;
        CreateAReelStrip(ReelStripPrefab);
        Debug.Log("prevReel: " + prevReelStrip + "....curReel: " + curReelStrip);
        PositionNewReelToBeAbovePrevReel(prevReelStrip, curReelStrip);
        Invoke("SimulateGameRound", 3f);
    }

    private void PositionNewReelToBeAbovePrevReel(ReelStrip prevStrip, ReelStrip curStrip)
    {
        Vector3 prevStripTailPos = prevStrip.GetTailPosition();
        Vector3 newPos = new Vector3(prevStripTailPos.x, prevStripTailPos.y + 5f, prevStripTailPos.z);
        curStrip.transform.position = newPos;

    }

    IEnumerator DelayedSpeedChange(float delayTime, float newSpeedVal)
    {
        yield return new WaitForSeconds(delayTime);
        UpdateSpeedOfReelStrips(newSpeedVal);
    }    

    //private void FixedUpdate()
    //{
    //    UpdateSpeedOfSymbols(MOVEMENT_INCREMENT);
    //}

    //private void UpdateSpeedOfSymbols(float newIncrementVal)
    //{
    //    for (int i = 0; i < Symbols.Count; i++)
    //    {
    //        Symbols[i].SetMovementIncrementValue(newIncrementVal);
    //    }
    //}

    private void UpdateSpeedOfReelStrips(float newIncrementVal)
    {
        curReelStrip.SetMovementIncrementValue(newIncrementVal);
        if (prevReelStrip != null)
        {
            prevReelStrip.SetMovementIncrementValue(newIncrementVal);
        }
    }


    private void SpinWheel()
    {
        curReelStrip.StartMoving();
        if (prevReelStrip != null)
        {
            prevReelStrip.StartMoving();
        }
        UpdateSpeedOfReelStrips(0.1f);
    }







}
