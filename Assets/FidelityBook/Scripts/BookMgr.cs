using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BookMgr : MonoBehaviour
{
    public List<PageMgr> pages;
    public GameObject markerPrefab;
    public GameObject goLeft;
    public GameObject goRight;
    public GameObject goTarget;
    float distMove;
    float dirMove;
    List<GameObject>markers = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        distMove = .01f;
        dirMove = 1;
        AddMarkers();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTarget();
        int nMarker = FindMarker();
        HighlightMarker(nMarker);
        AdjustPages(nMarker);
    }

    void AdjustPages(int nMarker)
    {
        for(int n = 0; n < pages.Count; n++)
        {
            PageMgr page = pages[n];            
            if (n <= nMarker){
                page.TurnLeft();
            } else
            {
                page.TurnRight();
            }
        }
    }

    void HighlightMarker(int nMarker)
    {
        for(int n = 0; n < markers.Count; n++)
        {
            GameObject marker = markers[n];
            Color color = Color.black;
            if (n == nMarker) color = Color.green;
            SetColor(marker, color);
        }
    }

    void SetColor(GameObject go, Color color)
    {
        go.GetComponent<Renderer>().material.color = color;
    }

    int FindMarker()
    {
        float distMin = -1;
        int nMin = -1;    
        for(int n = 0; n < markers.Count; n++)
        {
            GameObject marker = markers[n];
            float dist = Vector3.Distance(goTarget.transform.position, marker.transform.position);
            if (dist < distMin || n == 0)
            {
                distMin = dist;
                nMin = n;
            }
        }    
        return nMin;
    }

    void UpdateTarget()
    {
        MoveTarget();
        float z = goTarget.transform.position.z;
        if (dirMove == 1 && z > goRight.transform.position.z)
        {
            dirMove  = -1;
        }
        if (dirMove == -1 && z < goLeft.transform.position.z)
        {
            dirMove  = 1;
        }
    }

    void MoveTarget()
    {
        // if (Application.isEditor == false) return;
        Vector3 pos = goTarget.transform.position;
        pos.z += distMove * dirMove;
        goTarget.transform.position = pos;
    }

    void AddMarkers()
    {
        for(int n = 0; n < pages.Count - 1; n++)
        {
            PageMgr pageLeft = pages[n];
            PageMgr pageRight = pages[n + 1];
            Vector3 posMid = (pageLeft.transform.position + pageRight.transform.position) / 2;
            GameObject marker = CreateMarker(posMid);
            markers.Add(marker);
        }
    }

    GameObject CreateMarker(Vector3 pos)
    {
        GameObject marker = Instantiate(markerPrefab, transform);
        marker.transform.position = pos;
        return marker;
    }
}
