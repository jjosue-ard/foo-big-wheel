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
    //private const int SYMBOL_COUNT = 200;
    private const int TARGET_SYMBOL_INDEX = 168;    
    private float stoppingPoint;
    private float destroyPoint;

    // Start is called before the first frame update
    public void Load(float StoppingPointYPos, float DestroyPointYPos, float verticalInterval)
    {
        stoppingPoint = StoppingPointYPos;
        destroyPoint = DestroyPointYPos;
        isMoving = false;
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
        EnsureNotifyParentIfReelStripIsReadyToBeDeleted();
    }

    public void EnsureStitchAndGenerateReels(float verticalInterval, int symbolCount, List<Symbol_v2> symbolsToStitchAtHeadOfReelStrip = null)
    {
        symbols = new List<Symbol_v2>();
        if (symbolsToStitchAtHeadOfReelStrip != null)
        {                
            StitchSymbolsInViewToHeadOfReel(symbols, symbolsToStitchAtHeadOfReelStrip);
            symbolCount -= symbolsToStitchAtHeadOfReelStrip.Count;
        }
        GenerateSymbols(symbolCount, SymbolPrefab, verticalInterval, GetStartingTransformBasedOnWhetherOrNotStitchingIsNeeded(symbolsToStitchAtHeadOfReelStrip));
        InitSymbols();
    }

    // If no stitching involved, then return this.transform
    // ELSE return the last symbol in-view's transform
    private Transform GetStartingTransformBasedOnWhetherOrNotStitchingIsNeeded(List<Symbol_v2> symbolsToStitch)
    {
        Transform result = transform;
        if (symbolsToStitch != null)
        {
            int lastdIndex = symbolsToStitch.Count - 1;
            result = symbolsToStitch[lastdIndex].gameObject.transform;
        }
        return result;
    }

    private void StitchSymbolsInViewToHeadOfReel(List<Symbol_v2> reelSymbols, List<Symbol_v2> symbolsToStitch)
    {
        for (int i = 0; i < symbolsToStitch.Count; i++)
        {
            reelSymbols.Add(symbolsToStitch[i]);
        }
    }

    //private void GenerateSymbolSequence(float verticalInterval, int symbolCount)
    //{
    //    symbols = new List<Symbol_v2>();
    //    GenerateSymbols(symbolCount, symbolPrefab, verticalInterval);
    //    InitSymbols();
    //}

    private void InitSymbols()
    {
        for (int i = 0; i < symbols.Count; i++)
        {
            symbols[i].Load(i);
        }

        AddListenerToTargetSymbol(symbols[TARGET_SYMBOL_INDEX]);
    }

    private void AddListenerToTargetSymbol(Symbol_v2 targetSymbol)
    {
        EventManager.Instance.AddEventListener(this, targetSymbol, CustomEvent.Event, SymbolMessageHandler);
    }

    private void GenerateSymbols(int count, GameObject prefab, float verticalInterval, Transform startingSpawnPoint)
    {
        for (int i = 0; i < count; i++)
        {
            //Vector3 tmpPos = new Vector3(startingSpawnPoint.position.x, startingSpawnPoint.position.y, startingSpawnPoint.position.z);
            GameObject newGameObj = Instantiate(prefab, startingSpawnPoint.position, startingSpawnPoint.rotation, transform);
            Vector3 spawnPos = startingSpawnPoint.position;
            spawnPos.y = startingSpawnPoint.position.y + (verticalInterval * i);
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
        Symbol_v2 targetSymbol = symbols[targetSymbolIndex];
        Debug.Log("TargetSymbol.y: " + targetSymbol.transform.position.y + "<VS> stopping y: " + stoppingPoint);
        if (targetSymbol.transform.position.y <= stoppingPoint)
        {
            isMoving = false; //STOP REEL from moving
            Invoke("SimulateWaitingForUserSpacebarInput", 3f);
        }
    }

    private void SimulateWaitingForUserSpacebarInput()
    {
        MessageObject<string, object> egress = new MessageObject<string, object>();
        egress.Add(Commands.Command, Commands.TargetReelSymbolReachedDestination);
        SendMessageToParent(egress);
    }

    private void EnsureNotifyParentIfReelStripIsReadyToBeDeleted()
    {
        Symbol_v2 lastSymbol = symbols[symbols.Count - 1];
        //Debug.Log("TargetSymbol.y: " + lastSymbol.transform.position.y + "<VS> destroy y: " + destroyPoint);
        
        if (lastSymbol.transform.position.y <= destroyPoint)
        {
            MessageObject<string, object> egress = new MessageObject<string, object>();
            egress.Add(Commands.Command, Commands.ReelReachedItsDestroyPoint);
            egress.Add(Messages.ReelStripInstance, this);
            SendMessageToParent(egress);
        }
    }

    private void SymbolMessageHandler(object sender, EventManagerEventArgs e)
    {
        MessageObject<string, object> ingressMsg = (MessageObject<string, object>)e.eventObject;
        string command = (string)ingressMsg[Commands.Command];
        switch (command)
        {
            case Commands.TargetReelSymbolReachedDestination:
                //pass through
                MessageObject<string, object> egress = ingressMsg;
                SendMessageToParent(egress);
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

}
