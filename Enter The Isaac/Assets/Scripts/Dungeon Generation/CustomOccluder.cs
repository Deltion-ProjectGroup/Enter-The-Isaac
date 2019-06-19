using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomOccluder : MonoBehaviour
{


    //First appear occlusion
    //Then Remove occlusion of prev rooms
    public float occludeAppearSpeed, occludeRemoveSpeed;
    public Material occluderMat, occluderRemoveMat, occluderStayMat;
    public IEnumerator OccludeRooms(List<GameObject> roomsToOcclude)
    {
        print("REMOVIN");
        foreach (GameObject room in roomsToOcclude)
        {
            room.GetComponent<BaseRoom>().occluder.SetActive(true);
            room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material = occluderMat;
        }
        Color modifyColor = occluderMat.color;
        modifyColor.a = 0;
        occluderMat.color = modifyColor;
        
        while(modifyColor.a < 1)
        {
            yield return null;
            modifyColor.a = Mathf.Min(modifyColor.a + (occludeAppearSpeed * Time.deltaTime), 1);
            print(modifyColor.a);
            occluderMat.color = modifyColor;
        }
        yield return null;
        foreach (GameObject room in roomsToOcclude)
        {
            if(room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material.name == occluderMat.name + " (Instance)")
            {
                room.GetComponent<BaseRoom>().roomHolder.SetActive(false);
                occluderStayMat.color = occluderMat.color;
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
        modifyColor.a = 1;
        occluderRemoveMat.color = modifyColor;
        while (modifyColor.a > 0)
        {
            yield return null;
            modifyColor.a = Mathf.Max(modifyColor.a - (occludeRemoveSpeed * Time.deltaTime), 0);
            occluderRemoveMat.color = modifyColor;
        }
        yield return null;
        foreach (GameObject room in roomsToRemoveOcclude)
        {
            if(room.GetComponent<BaseRoom>().occluder.GetComponent<MeshRenderer>().material.name == occluderRemoveMat.name + " (Instance)")
            {
                room.GetComponent<BaseRoom>().occluder.SetActive(false);
            }
        }
    }
}
