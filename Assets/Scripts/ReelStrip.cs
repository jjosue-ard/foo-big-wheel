using Assets.Scripts.Datamodels;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelStrip : MonoBehaviour
{
    public Material[] materials;
    public GameObject SymbolPrefab;    

    private bool isMoving;    
    private float movementIncrementValue;
    private List<Symbol_v2> symbols;    
    private int TARGET_SYMBOL_INDEX;    
    private float stoppingPoint;    

    private bool msgReachDestinationSend; //flag to prevent multiple messages from being sent to parent

    // Start is called before the first frame update
    public void Load(float StoppingPointYPos, float verticalInterval, int targetIndex)
    {
        stoppingPoint = StoppingPointYPos;
        isMoving = false;
        TARGET_SYMBOL_INDEX = targetIndex;
        //GenerateSymbolSequence(verticalInterval);
    }

    public Symbol_v2 GetSymbolScriptByIndex(int i)
    {
        return symbols[i];
    }

    public int GetTargetSymbolIndex()
    {
        return TARGET_SYMBOL_INDEX;
    }

    public void StartMoving()
    {
        Debug.Log("ReelStrip.StartMoving()");
        isMoving = true;
    }

    public Vector3 GetHeadPosition()
    {
        return symbols[0].transform.position;
    }

    public Vector3 GetTailPosition()
    {
        return symbols[symbols.Count - 1].transform.position;
    }

    public void SetMovementIncrementValue(float newVal)
    {
        movementIncrementValue = newVal;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        EnsureMoveReelStrip(movementIncrementValue);
        EnsureNotifyParentIfTargetSymbolReachedDestination(TARGET_SYMBOL_INDEX);
        //EnsureNotifyParentIfReelStripIsReadyToBeDeleted();
    }

    public void EnsureStitchAndGenerateReels(float verticalInterval, int symbolCount, List<Symbol_v2> symbolsToStitchAtHeadOfReelStrip = null)
    {
        symbols = new List<Symbol_v2>();
        int startIndexForInitSymbols = 0;
        if (symbolsToStitchAtHeadOfReelStrip != null)
        {                
            StitchSymbolsInViewToHeadOfReel(ref symbols, symbolsToStitchAtHeadOfReelStrip);
            symbolCount -= symbolsToStitchAtHeadOfReelStrip.Count;
            startIndexForInitSymbols = symbolsToStitchAtHeadOfReelStrip.Count; // skip the symbols that are already in-view 
        }
        Transform startingSpawnPoint = GetStartingTransformBasedOnWhetherOrNotStitchingIsNeeded(symbolsToStitchAtHeadOfReelStrip, verticalInterval);        

        // Create and initialize an entire reelstrip
        // THEN put in the expected win result into the target destination on the reel strip
        GenerateSymbols(symbolCount, SymbolPrefab, verticalInterval, startingSpawnPoint, symbolsToStitchAtHeadOfReelStrip);
        InitSymbols(startIndexForInitSymbols, ReelDataManager.GetReelStripData().SymbolTable);
        SymbolWeightDataModel winSymbolData = PickWinningResult(ReelDataManager.GetReelStripData().SymbolTable);
        InitTargetDestinationSymbol(winSymbolData, TARGET_SYMBOL_INDEX);
    }

    private SymbolWeightDataModel PickWinningResult(List<SymbolWeightDataModel> symbolTable)
    {
        SymbolWeightDataModel result = null;        
        float totalWeight = GetTotalWeight(symbolTable);
        float rndPicked = Random.Range(0, totalWeight);
        float weightSum = 0;
        int i = 0;
        for (i = 0; i < symbolTable.Count; i++)
        {
            weightSum += symbolTable[i].Weight;
            if (rndPicked < weightSum)
            {
                break;
            }
        }
        result = symbolTable[i];
        return result;
    }

    private float GetTotalWeight(List<SymbolWeightDataModel> symbolTable)
    {
        float result = 0;
        for (int i = 0; i < symbolTable.Count; i++)
        {
            result += symbolTable[i].Weight;
        }
        return result;
    }

    private void InitTargetDestinationSymbol(SymbolWeightDataModel winResultData, int targetIndex)
    {
        Debug.Log("Target win result: " + winResultData.DisplayedText);
        symbols[targetIndex].Load(targetIndex, winResultData);
        AddListenerToTargetSymbol(symbols[TARGET_SYMBOL_INDEX]);
    }

    // If no stitching involved, then return this.transform
    // ELSE return the last symbol in-view's transform with offset (so the new reel not right on top of the last symbol)
    private Transform GetStartingTransformBasedOnWhetherOrNotStitchingIsNeeded(List<Symbol_v2> symbolsToStitch, float verticalInterval)
    {
        Transform result = transform;
        if (symbolsToStitch != null)
        {
            int lastdIndex = symbolsToStitch.Count - 1;
            result = symbolsToStitch[lastdIndex].gameObject.transform;
            //Vector3 tmp = symbolsToStitch[lastdIndex].gameObject.transform.position;
            //tmp.y += verticalInterval;
            //result.position = tmp;
        }
        return result;
    }

    private void StitchSymbolsInViewToHeadOfReel(ref List<Symbol_v2> reelSymbols, List<Symbol_v2> symbolsToStitch)
    {
        for (int i = 0; i < symbolsToStitch.Count; i++)
        {
            reelSymbols.Add(symbolsToStitch[i]);
            symbolsToStitch[i].gameObject.transform.SetParent(transform); //transfer from old reel to this reel
        }
    }

    //private void GenerateSymbolSequence(float verticalInterval, int symbolCount)
    //{
    //    symbols = new List<Symbol_v2>();
    //    GenerateSymbols(symbolCount, symbolPrefab, verticalInterval);
    //    InitSymbols();
    //}

    private void InitSymbols(int startIndex, List<SymbolWeightDataModel> symbolTable)
    {
        for (int i = startIndex; i < symbols.Count; i++)
        {
            symbols[i].Load(i, GetRandomSymbolData(symbolTable));
        }        
    }

    private SymbolWeightDataModel GetRandomSymbolData(List<SymbolWeightDataModel> symbolTable)
    {
        int randomIndex = Random.Range(0, symbolTable.Count - 1);
        return symbolTable[randomIndex];
    }

    private void AddListenerToTargetSymbol(Symbol_v2 targetSymbol)
    {
        EventManager.Instance.AddEventListener(this, targetSymbol, CustomEvent.Event, SymbolMessageHandler);
    }

    private void GenerateSymbols(int count, GameObject prefab, float verticalInterval, Transform startingSpawnPoint, List<Symbol_v2> symbolsToStitchAtHeadOfReelStrip = null)
    {        
        for (int i = 0; i < count; i++)
        {
            //Vector3 tmpPos = new Vector3(startingSpawnPoint.position.x, startingSpawnPoint.position.y, startingSpawnPoint.position.z);
            GameObject newGameObj = Instantiate(prefab, startingSpawnPoint.position, startingSpawnPoint.rotation, transform);
            Vector3 spawnPos = startingSpawnPoint.position;
            spawnPos.y = startingSpawnPoint.position.y + (verticalInterval * i) + GetYOffsetForStitchingReels(verticalInterval, symbolsToStitchAtHeadOfReelStrip);
            spawnPos.z = -1; //to be shown in front of the viewing box
            newGameObj.transform.position = spawnPos;
            newGameObj.transform.parent = transform;
            symbols.Add(newGameObj.GetComponent<Symbol_v2>());

            //change the colors of each wall (for testing sake)
            MeshRenderer mr = newGameObj.GetComponent<MeshRenderer>();
            Debug.Assert(mr != null, "Uh OH Spaghettios! MeshRenderer cannot be null for index: " + i);
            mr.material = GetRandomMaterial(i);

            //rotate the quad to face away from the center of the wheel (and instead face the player)            
            newGameObj.name = "Symbol" + i;
        }
    }

    private float GetYOffsetForStitchingReels(float verticalInterval, List<Symbol_v2> symbolsToStitchAtHeadOfReelStrip)
    {
        float yOffset = 0;
        if (symbolsToStitchAtHeadOfReelStrip != null)
        {
            yOffset = verticalInterval;
        }
        return yOffset;
    }

    private Material GetRandomMaterial(int i)
    {
        Material result = materials[0];
        int rndIndex = Random.Range(0, materials.Length - 1);
        result = materials[rndIndex];
        return result;
    }

    private void EnsureMoveReelStrip(float incrementVal)
    {
        if (isMoving)
        {
            Vector3 newPos = transform.position;
            newPos.y -= incrementVal;
            transform.position = newPos;
        }
    }

    private void EnsureNotifyParentIfTargetSymbolReachedDestination(int targetSymbolIndex)
    {
        //safeguard
        if (symbols != null)
        {
            Symbol_v2 targetSymbol = symbols[targetSymbolIndex];
            //Debug.Log("TargetSymbol.y: " + targetSymbol.transform.position.y + "<VS> stopping y: " + stoppingPoint);
            if (targetSymbol.transform.position.y <= stoppingPoint)
            {
                isMoving = false; //STOP REEL from moving
                Invoke("SimulateWaitingForUserSpacebarInput", 3f);
            }
        }
    }

    private void SimulateWaitingForUserSpacebarInput()
    {
        MessageObject<string, object> egress = new MessageObject<string, object>();
        egress.Add(Commands.Command, Commands.TargetReelSymbolReachedDestination);        
        SendMessageToParent(egress);
    }

    //private void EnsureNotifyParentIfReelStripIsReadyToBeDeleted()
    //{
    //    if (symbols != null)
    //    {
    //        Symbol_v2 lastSymbol = symbols[symbols.Count - 1];
    //        //Debug.Log("TargetSymbol.y: " + lastSymbol.transform.position.y + "<VS> destroy y: " + destroyPoint);

    //        if (lastSymbol.transform.position.y <= destroyPoint)
    //        {
    //            MessageObject<string, object> egress = new MessageObject<string, object>();
    //            egress.Add(Commands.Command, Commands.ReelReachedItsDestroyPoint);
    //            egress.Add(Messages.ReelStripInstance, this);
    //            SendMessageToParent(egress);
    //        }
    //    }
    //}

    private void SymbolMessageHandler(object sender, EventManagerEventArgs e)
    {
        MessageObject<string, object> ingressMsg = (MessageObject<string, object>)e.eventObject;
        string command = (string)ingressMsg[Commands.Command];
        switch (command)
        {
            case Commands.TargetReelSymbolReachedDestination:
                //pass through
                MessageObject<string, object> egress = ingressMsg;
                EnsureSendReachedDestinationOnce(ingressMsg);
                break;
            default:
                Debug.LogError("No case found for: " + command);
                break;
        }
    }

    private void EnsureSendReachedDestinationOnce(MessageObject<string, object> egress)
    {
        if (!msgReachDestinationSend)
        {
            msgReachDestinationSend = true;
            SendMessageToParent(egress);
        }
    }

    public virtual void SendMessageToParent(MessageObject<string, object> messageObject)
    {
        EventManagerEventArgs args = new EventManagerEventArgs();
        args.eventObject = messageObject;
        EventManager.Instance.DispatchEvent(this, CustomEvent.Event, args);
    }

}
