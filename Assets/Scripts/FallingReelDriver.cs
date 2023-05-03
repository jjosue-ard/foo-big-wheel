using Assets.Scripts.Datamodels;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingReelDriver : MonoBehaviour
{

    public GameObject ViewingBox;    
    public GameObject ReelStripPrefab;
    public GameObject Canvas;
    
    public float VERTICAL_INTERVAL_BETWEEN_SYMBOLS = 5f;

    // how many symbols will show up in the player's view when the reel stops
    // This should be odd in this use case since the reel stops with a symbol right in the middle
    // Make sure to include any symbols that are partially visible in view
    public int SYMBOLS_VISIBLE_IN_VIEW_COUNT = 3;
    public int TargetSymbolIndex = 168;

    private Vector3 STOPPING_Y_POS; //where the falling reel strip will stop
    private int SYMBOL_COUNT;    
    private ReelStrip curReelStrip;
    private ReelStrip prevReelStrip;

    // Start is called before the first frame update
    void Start()
    {
        ReelDataManager.Load();
        STOPPING_Y_POS = ViewingBox.transform.position;
        STOPPING_Y_POS.y += 2f;
        SYMBOL_COUNT = ReelDataManager.GetReelStripData().SymbolCountPerReelStrip;
        EnsureCreateReelStripContinuingFromPrevReelStripIfAny(ReelStripPrefab);
        SimulateGameRound();
    }

    private void EnsureCreateReelStripContinuingFromPrevReelStripIfAny(GameObject reelStripPrefab)
    {
        if (prevReelStrip == null)
        {
            curReelStrip = CreateAReelStrip(reelStripPrefab);
            curReelStrip.EnsureStitchAndGenerateReels(VERTICAL_INTERVAL_BETWEEN_SYMBOLS, SYMBOL_COUNT);
        }
        else
        {
            // stitch the current reel's symbols in-view to be the head of the new reel
            List<Symbol_v2> symbolsInView = GetSymbolsInView(SYMBOLS_VISIBLE_IN_VIEW_COUNT, curReelStrip);
            curReelStrip = CreateAReelStrip(reelStripPrefab);
            StitchPrevAndNewReel(symbolsInView, curReelStrip, reelStripPrefab);
            DestroyReelProcedures(prevReelStrip);
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
        Debug.Assert(TargetSymbolIndex < ReelDataManager.GetReelStripData().SymbolCountPerReelStrip, "Target symbol index is out of bounds; Target index: " + TargetSymbolIndex + " total symbol count: " + ReelDataManager.GetReelStripData().SymbolCountPerReelStrip);
        newReel.Load(STOPPING_Y_POS, VERTICAL_INTERVAL_BETWEEN_SYMBOLS, TargetSymbolIndex);
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

        

    }
    
    private void ReelStripMessageHandler(object sender, EventManagerEventArgs e)
    {
        MessageObject<string, object> ingressMsg = (MessageObject<string, object>)e.eventObject;
        string command = (string)ingressMsg[Commands.Command];
        switch (command)
        {
            case Commands.TargetReelSymbolReachedDestination:                
                ReelDoneMovingProcedures();
                break;
            //case Commands.ReelReachedItsDestroyPoint:
            //    ReelStrip reelToDestroy = (ReelStrip)ingressMsg[Messages.ReelStripInstance];
            //    DestroyReelProcedures(reelToDestroy);
            //    break;
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

    private void DestroyReelProcedures(ReelStrip reelToDestroy)
    {        
        EventManager.Instance.RemoveEventListener(this, reelToDestroy, CustomEvent.Event, ReelStripMessageHandler);
        Destroy(reelToDestroy.gameObject);
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

  

    private void SpinWheel()
    {
        curReelStrip.StartMoving();
        if (prevReelStrip != null)
        {
            prevReelStrip.StartMoving();
        }        
    }







}
