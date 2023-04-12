using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_v2 : MonoBehaviour
{
    public Material[] materials;
    public GameObject symbolPrefab;

    [Range(0.0f, 5.0f)]
    public float MOVEMENT_INCREMENT;

    private List<Symbol_v2> Symbols;

    // Start is called before the first frame update
    void Start()
    {
        Symbols = new List<Symbol_v2>();
        GenerateSymbols(1000, symbolPrefab);
        InitSymbols();
        SimulateGameRound();
    }

    private void SimulateGameRound()
    {
        SpinWheel();
        float timeAccumulate = 0;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.5f));
        
        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 1f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 3f));

        timeAccumulate += 2f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 4f));

        timeAccumulate += 3f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 3f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 1f));

        timeAccumulate += 1f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0.5f));

        timeAccumulate += 0.8f;
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0f));


    }

    IEnumerator DelayedSpeedChange(float delayTime, float newSpeedVal)
    {
        yield return new WaitForSeconds(delayTime);
        UpdateSpeedOfSymbols(newSpeedVal);
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

    //private void FixedUpdate()
    //{
    //    UpdateSpeedOfSymbols(MOVEMENT_INCREMENT);
    //}

    private void UpdateSpeedOfSymbols(float newIncrementVal)
    {
        for (int i = 0; i < Symbols.Count; i++)
        {
            Symbols[i].SetMovementIncrementValue(newIncrementVal);
        }
    }

    private void InitSymbols()
    {        
        for (int i = 0; i < Symbols.Count; i++)
        {
            Symbols[i].Load(i);
        }
    }

    private void SpinWheel()
    {
        for (int i = 0; i < Symbols.Count; i++)
        {
            Symbols[i].StartMoving();
        }
    }







}
