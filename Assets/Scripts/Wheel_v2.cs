using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_v2 : MonoBehaviour
{
    public GameObject ViewingBox;
    public GameObject DestroyReelPoint;
    public GameObject ReelStripPrefab;
    public GameObject Canvas;

    [Range(0.0f, 5.0f)]
    public float MOVEMENT_INCREMENT;
    private const float VERTICAL_INTERVAL_BETWEEN_SYMBOLS = 5f;

    // how many symbols will show up in the player's view when the reel stops
    // This should be odd in this use case since the reel stops with a symbol right in the middle
    // Make sure to include any symbols that are partially visible in view
    public int SYMBOLS_VISIBLE_IN_VIEW_COUNT = 3;

    private ReelStrip curReelStrip;
    private ReelStrip prevReelStrip;

    // Start is called before the first frame update
    void Start()
    {
        CreateAReelStrip(ReelStripPrefab);        
        //SimulateGameRound();
    }

    private void EnsureCreateReelStripContinuingFromPrevReelStripIfAny(GameObject reelStripPrefab)
    {
        if (prevReelStrip == null)
        {
            CreateAReelStrip(reelStripPrefab);
        }
        else
        {
            // stitch the current reel's symbols in-view to be the head of the new reel
            StitchPrevAndNewReel(prevReelStrip, curReelStrip, reelStripPrefab);
        }
    }

    // this is assuming that there are already a current and prev reel strips
    private void StitchPrevAndNewReel(ReelStrip prevReel, ReelStrip curReel, GameObject reelPrefab)
    {
        Debug.Assert(curReel != null, "HOLD yer horses there! curReel CANNOT be null!");
        Debug.Assert(prevReel != null, "HOLD yer horses there! prevReel CANNOT be null!");

        List<Symbol_v2> symbolsInView = GetSymbolsInView(SYMBOLS_VISIBLE_IN_VIEW_COUNT, curReelStrip);


    }

    // This calculates the bottom most symbol in-view
    // (Assuming that the number of symbols in-view is an odd number; implying
    // that there is 1 symbol right in the center
    private List<Symbol_v2> GetSymbolsInView(int symbolsInViewCount, ReelStrip reelInView)
    {
        List<Symbol_v2> result = new List<Symbol_v2>();
        int targetSymbolIndex = reelInView.GetTargetSymbolIndex();
        int bottomSymbolInViewIndex = targetSymbolIndex - (symbolsInViewCount / 2);
        for (int i = 0; i < symbolsInViewCount; i++ )
        {
            int indexToGet = bottomSymbolInViewIndex + i;
            Symbol_v2 curSymbol = reelInView.GetSymbolScriptByIndex(indexToGet);
            result.Add(curSymbol);
        }

        return result;
    }

    private void CreateAReelStrip(GameObject reelStrip)
    {
        GameObject newReelStrip = Instantiate(reelStrip.gameObject, GetReelSpawnPosition(SYMBOLS_VISIBLE_IN_VIEW_COUNT), transform.rotation, Canvas.transform); ;                
        curReelStrip = newReelStrip.GetComponent<ReelStrip>();
        EventManager.Instance.AddEventListener(this, curReelStrip, CustomEvent.Event, ReelStripMessageHandler);
        curReelStrip.Load(ViewingBox.transform.position.y, DestroyReelPoint.transform.position.y, VERTICAL_INTERVAL_BETWEEN_SYMBOLS);
        curReelStrip.name = "stripName: " + Time.realtimeSinceStartup;
    }

    private Vector3 GetReelSpawnPosition(int symbolsInView)
    {
        Vector3 result = ViewingBox.transform.position;
        int symbolsAboveCenterLine = symbolsInView / 2;
        result.y -= VERTICAL_INTERVAL_BETWEEN_SYMBOLS * symbolsAboveCenterLine; //move the head 
        return result;
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
            case Commands.ReelReachedItsDestroyPoint:
                ReelStrip reelToDestroy = (ReelStrip)ingressMsg[Messages.ReelStripInstance];
                EventManager.Instance.RemoveEventListener(this, reelToDestroy, CustomEvent.Event, ReelStripMessageHandler);
                Destroy(reelToDestroy.gameObject);
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
        // as a safeguard, ONLY create more reels if prevReelStrip is null (has been deleted already)
        if (prevReelStrip == null)
        {
            prevReelStrip = curReelStrip;
            EnsureCreateReelStripContinuingFromPrevReelStripIfAny(ReelStripPrefab);
            //CreateAReelStrip(ReelStripPrefab);
            //Debug.Log("prevReel: " + prevReelStrip + "....curReel: " + curReelStrip);
            //PositionNewReelToBeAbovePrevReel(prevReelStrip, curReelStrip);
        }


        Invoke("SimulateGameRound", 3f); // Simulate player pressing spacebar
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
