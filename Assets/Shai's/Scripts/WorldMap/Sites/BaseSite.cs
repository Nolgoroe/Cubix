using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SiteType { Shop, Healing, Forge, Combat, Unknown, Junkyard, Boss }


public class BaseSite : MonoBehaviour
{
    public SiteType type;
    public Sprite icon;
    [SerializeField] private int lvlSceneNum;

    public void LaunchSite()
    {
        SceneManager.LoadScene(lvlSceneNum);
    }
}
