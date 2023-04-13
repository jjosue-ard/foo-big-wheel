using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_v2 : MonoBehaviour
{
    public GameObject ReelStripPrefab;
    public GameObject Canvas;

    [Range(0.0f, 5.0f)]
    public float MOVEMENT_INCREMENT;

    private ReelStrip curReelStrip;

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
        curReelStrip.Load();
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
        StartCoroutine(DelayedSpeedChange(timeAccumulate, 0f));
    }

    IEnumerator DelayedSpeedChange(float delayTime, float newSpeedVal)
    {
        yield return new WaitForSeconds(delayTime);
        UpdateSpeedOfReelStrip(newSpeedVal);
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

    private void UpdateSpeedOfReelStrip(float newIncrementVal)
    {
        curReelStrip.SetMovementIncrementValue(newIncrementVal);
    }


    private void SpinWheel()
    {
        curReelStrip.StartMoving();
    }







}
