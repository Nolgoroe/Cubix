using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SiteType { Shop, Healing, Forge, Combat, Unknown, Junkyard, Boss }


public abstract class BaseSite : MonoBehaviour
{
    public SiteType type;
    public Sprite icon;

    public abstract void LaunchSite();
    
}
