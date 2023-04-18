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
    public int SYMBOL_COUNT = 200;

    private ReelStrip curReelStrip;
    private ReelStrip prevReelStrip;

    // Start is called before the first frame update
    void Start()
    {
        EnsureCreateReelStripContinuingFromPrevReelStripIfAny(ReelStripPrefab, prevReelStrip, curReelStrip);
        //SimulateGameRound();
    }

    private void EnsureCreateReelStripContinuingFromPrevReelStripIfAny(GameObject reelStripPrefab, ReelStrip prevReel, ReelStrip curReel)
    {
        if (prevReel == null)
        {
            curReelStrip = CreateAReelStrip(reelStripPrefab);
            curReelStrip.EnsureStitchAndGenerateReels(VERTICAL_INTERVAL_BETWEEN_SYMBOLS, SYMBOL_COUNT);
        }
        else
        {
            // stitch the current reel's symbols in-view to be the head of the new reel
            List<Symbol_v2> symbolsInView = GetSymbolsInView(SYMBOLS_VISIBLE_IN_VIEW_COUNT, curReelStrip);
            curReelStrip = CreateAReelStrip(reelStripPrefab);
            StitchPrevAndNewReel(symbolsInView, curReel, reelStripPrefab);
        }
    }

    // this is assuming that there are already a current and prev reel strips
    private void StitchPrevAndNewReel(List<Symbol_v2> symbolsInView, ReelStrip curReel, GameObject reelPrefab)
    {
        Debug.Assert(curReel != null, "HOLD yer horses there! curReel CANNOT be null!");        
        curReel.EnsureStitchAndGenerateReels(VERTICAL_INTERVAL_BETWEEN_SYMBOLS, SYMBOL_COUNT, symbolsInView);
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

    private ReelStrip CreateAReelStrip(GameObject reelStrip)
    {
        GameObject newReelStrip = Instantiate(reelStrip.gameObject, GetReelSpawnPosition(SYMBOLS_VISIBLE_IN_VIEW_COUNT), transform.rotation, Canvas.transform);
        ReelStrip newReel = newReelStrip.GetComponent<ReelStrip>();
        EventManager.Instance.AddEventListener(this, newReel, CustomEvent.Event, ReelStripMessageHandler);
        newReel.Load(ViewingBox.transform.position.y, DestroyReelPoint.transform.position.y, VERTICAL_INTERVAL_BETWEEN_SYMBOLS);
        newReel.name = "stripName: " + Time.realtimeSinceStartup;
        return newReel;
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
            EnsureCreateReelStripContinuingFromPrevReelStripIfAny(ReelStripPrefab, prevReelStrip, curReelStrip);
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
