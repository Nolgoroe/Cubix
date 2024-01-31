using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CombatSite : BaseSite
{
    [SerializeField] private int levelSceneNum;

    public override void LaunchSite()
    {
        SceneManager.LoadScene(levelSceneNum);
    }

}
