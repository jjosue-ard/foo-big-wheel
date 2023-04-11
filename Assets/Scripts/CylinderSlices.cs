using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this class is for the CENTER of the circle
public class CylinderSlices : MonoBehaviour
{    
    public float totalWallCount;
    public GameObject WallPrefab;
    private List<GameObject> walls;

    // Start is called before the first frame update
    void Start()
    {
        walls = new List<GameObject> ();
        GenerateWalls();
        RotateEntireWheelToFacePlayers();
    }

    private void RotateEntireWheelToFacePlayers()
    {
        Vector3 newRotate = new Vector3(0f, -90f, 0f);
        transform.Rotate(0f, -90f, 0f, Space.Self);
    }

    /*
     * Created in clockwise arrangement
     */

    private void GenerateWalls()
    {
        float RADIUS = 5f;
        for (int i = 0; i < totalWallCount; i++)
        {

            Vector3 tmpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            GameObject newWall = Instantiate(WallPrefab, tmpPos, transform.rotation, transform);           
            Vector3 spawnPos = GetDotTransform(transform.position, RADIUS, i);
            newWall.transform.position = spawnPos;

            //float rotationOffset = (360f / totalWallCount) * (float)i;            
            //newWall.transform.eulerAngles = new Vector3(
            //    newWall.transform.eulerAngles.x,
            //    newWall.transform.eulerAngles.y,
            //    newWall.transform.eulerAngles.z + rotationOffset
            //);
            newWall.transform.rotation = GetRotationAwayCenter(newWall); // make the gameobjects look away from the center of the circle
            newWall.name = "Slice_" + i;           
        }
    }

    private Quaternion GetRotationAwayCenter(GameObject gameObj)
    {
        Vector3 directionAwayFromCenter = transform.position - gameObj.transform.position;
        Quaternion awayRotation = Quaternion.LookRotation(directionAwayFromCenter);
        return awayRotation;    
    }


    private Vector3 GetDotTransform(Vector3 origPos, float radius, float angle)
    {
        float xVal = radius * Mathf.Cos((angle * Mathf.PI) / 180f);
        float yVal = radius * Mathf.Sin((angle * Mathf.PI) / 180f);
        Vector3 transformWithOffset = new Vector3();
        transformWithOffset = origPos;
        transformWithOffset.x += xVal;
        transformWithOffset.y += yVal;
        return transformWithOffset;
    }







}
