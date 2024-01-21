using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SiteType { Shop, Healing, Forge, Combat, Unknown, Junkyard, Boss }


public class BaseSite : MonoBehaviour
{
    public SiteType type;
    public Sprite icon;

    public void LaunchSite()
    {
        //blah blah launch me
    }
}
