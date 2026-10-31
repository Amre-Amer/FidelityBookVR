using System.Collections.Generic;
using UnityEngine;

public class PagesMgr : MonoBehaviour
{
    public Globals g;
    float distMove;
    float dirMove;
    List<GameObject>markers = new();

    void Awake()
    {
        LoadPages();
    }

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
        g.nPageSelected = FindMarker();
        HighlightMarker();
        AdjustPages();
    }

    void LoadPages()
    {
        g.pages.Clear();
        foreach(Transform t in transform)
        {
            g.pages.Add(t.GetComponent<PageMgr>());
        }    
    }

    void AdjustPages()
    {
        for(int n = 0; n < g.pages.Count; n++)
        {
            PageMgr page = g.pages[n];            
            if (n <= g.nPageSelected){
                page.TurnLeft();
            } else
            {
                page.TurnRight();
            }
        }
    }

    void HighlightMarker()
    {
        for(int n = 0; n < markers.Count; n++)
        {
            GameObject marker = markers[n];
            Color color = Color.black;
            if (n == g.nPageSelected) color = Color.green;
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
            float dist = Vector3.Distance(g.goTarget.transform.position, marker.transform.position);
            if (dist < distMin || n == 0)
            {
                distMin = dist;
                nMin = n;
            }
        }    
        if (nMin < 0) Debug.Log("<color=red>Marker not found !</color>\n");
        return nMin;
    }

    void UpdateTarget()
    {
        MoveTarget();
        float z = g.goTarget.transform.position.z;
        if (dirMove == 1 && z > g.goRight.transform.position.z)
        {
            dirMove  = -1;
        }
        if (dirMove == -1 && z < g.goLeft.transform.position.z)
        {
            dirMove  = 1;
        }
    }

    void MoveTarget()
    {
        // Debug.Log("Application.isEditor: " + Application.isEditor + "\n");
        if (Application.isEditor == false) return;
        Vector3 pos = g.goTarget.transform.position;
        pos.z += distMove * dirMove;
        g.goTarget.transform.position = pos;
    }

    void AddMarkers()
    {
        for(int n = 0; n < g.pages.Count - 1; n++)
        {
            PageMgr pageLeft = g.pages[n];
            PageMgr pageRight = g.pages[n + 1];
            Vector3 posMid = (pageLeft.transform.position + pageRight.transform.position) / 2;
            GameObject marker = CreateMarker(posMid);
            marker.name = "marker_" + n;
            markers.Add(marker);
        }
    }

    GameObject CreateMarker(Vector3 pos)
    {
        GameObject marker = Instantiate(g.markerPrefab, transform);        
        marker.transform.position = pos;
        return marker;
    }
}
