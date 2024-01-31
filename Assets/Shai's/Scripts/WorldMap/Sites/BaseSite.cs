using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SiteType { Shop, Healing, Forge, Combat, Mystery, Junkyard, Boss }


public abstract class BaseSite : MonoBehaviour
{
    [Header("BaseSiteSettings")]
    public SiteType type;
    public Sprite icon;

    public abstract void LaunchSite();
    
}
