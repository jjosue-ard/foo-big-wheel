using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelStrip : MonoBehaviour
{
    public Material[] materials;
    public GameObject symbolPrefab;

    private bool isMoving;    
    private float movementIncrementValue;
    private List<Symbol_v2> Symbols;
    private const int SYMBOL_COUNT = 10;

    // Start is called before the first frame update
    public void Load()
    {
        isMoving = false;
        GenerateSymbolSequence();
    }

    public void StartMoving()
    {
        Debug.Log("ReelStrip.StartMoving()");
        isMoving = true;
    }

    public Vector3 GetTailPosition()
    {
        return Symbols[Symbols.Count - 1].transform.position;
    }

    public void SetMovementIncrementValue(float newVal)
    {
        movementIncrementValue = newVal;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        EnsureMoveSymbol(movementIncrementValue);
    }

    private void GenerateSymbolSequence()
    {
        Symbols = new List<Symbol_v2>();
        GenerateSymbols(SYMBOL_COUNT, symbolPrefab);
        InitSymbols();
    }

    private void InitSymbols()
    {
        for (int i = 0; i < Symbols.Count; i++)
        {
            Symbols[i].Load(i);
        }
    }

    private void GenerateSymbols(int count, GameObject prefab)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 tmpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            GameObject newGameObj = Instantiate(prefab, tmpPos, transform.rotation, transform);
            Vector3 spawnPos = transform.position;
            spawnPos.y = transform.position.y + (5f * i);
            newGameObj.transform.position = spawnPos;
            newGameObj.transform.parent = transform;
            Symbols.Add(newGameObj.GetComponent<Symbol_v2>());

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

    private void EnsureMoveSymbol(float incrementVal)
    {
        if (isMoving)
        {
            Vector3 newPos = transform.position;
            newPos.y -= incrementVal;
            transform.position = newPos;
        }
    }

  
}
