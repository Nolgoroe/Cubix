using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//temporarily used on every site that loads a scene 
public class CombatSite : BaseSite
{
    [SerializeField] private int levelSceneNum;

    [ContextMenu("sweoop")]
    public override void LaunchSite()
    {
        SceneManager.LoadScene(levelSceneNum);
    }

}
