using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower2D : MonoBehaviour
{
    public BuildSite2D MyBuildSite { get; private set; }

    public void SetBuildSite(BuildSite2D site)
    {
        MyBuildSite = site;
    }

    void OnMouseDown()
    {
        if (BuildManager2D.Instance != null)
        {
            BuildManager2D.Instance.OpenTowerInfo(this);
        }
    }
}
