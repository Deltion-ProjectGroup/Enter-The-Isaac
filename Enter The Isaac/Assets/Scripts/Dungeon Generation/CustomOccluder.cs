using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomOccluder : MonoBehaviour
{


    //First appear occlusion
    //Then Remove occlusion of prev rooms
    public float occludeSpeed;
    public Material occluderMat, occluderRemoveMat, occluderStayMat;
    public IEnumerator OccludeRooms(List<GameObject> roomsToOcclude)
    {
        print("REMOVIN");
        foreach (GameObject room in roomsToOcclude)
        {
            room.GetComponent<BaseRoom>().occluder.SetActive(true);
            room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material = occluderMat;
            print(occluderMat.name);
            print(room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material.name);
        }
        Color modifyColor = occluderMat.color;
        modifyColor.a = occluderRemoveMat.color.a;
        occluderMat.color = modifyColor;
        while(modifyColor.a < 1)
        {
            yield return null;
            modifyColor.a = Mathf.Min(modifyColor.a + (occludeSpeed * Time.deltaTime), 1);
            occluderMat.color = modifyColor;
        }
        foreach (GameObject room in roomsToOcclude)
        {
            if(room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material.name == occluderMat.name + " (Instance)")
            {
                room.GetComponent<BaseRoom>().roomHolder.SetActive(false);
                room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material = occluderStayMat;
            }
        }
    }

    public IEnumerator ClearOccludeRooms(List<GameObject> roomsToRemoveOcclude)
    {
        foreach (GameObject room in roomsToRemoveOcclude)
        {
            room.GetComponent<BaseRoom>().roomHolder.SetActive(true);
            room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material = occluderRemoveMat;
        }
        Color modifyColor = occluderRemoveMat.color;
        modifyColor.a = occluderMat.color.a;
        occluderRemoveMat.color = modifyColor;
        while (modifyColor.a > 0)
        {
            yield return null;
            modifyColor.a = Mathf.Max(modifyColor.a - occludeSpeed, 0);
            occluderRemoveMat.color = modifyColor;
        }
        foreach (GameObject room in roomsToRemoveOcclude)
        {
            if(room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material.name == occluderRemoveMat.name + " (Instance)")
            {
                room.GetComponent<BaseRoom>().occluder.SetActive(false);
            }
        }
    }
}
