using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapProgression : MonoBehaviour
{
    [SerializeField] private WorldGrid worldGrid;
    private List<SiteNode> openNodes = new List<SiteNode>();

    private void Start()
    {
        UIManager.Instance.UpdateMapDiceDisplay();

        SoundManager.Instance.PlaySoundNormal(Sounds.MapBGM);
    }
    public void Init()
    {
        openNodes.Clear();
        foreach (var node in worldGrid.GetAllNodes())
        {
            SetNodeStatusOnStart(node);
            node.OnClicked.AddListener(UpdateProgression);
        }
    }

    private void SetNodeStatusOnStart(SiteNode node)
    {
        if (node.gridPos.y == 0)
        {
            node.Unlock();
            openNodes.Add(node);
        }
        else
        {
            node.Lock();
        }
    }

    private void UpdateProgression(SiteNode clickedNode)
    {
        //the node already updated himslef so no need to call his "Picked" method
        openNodes.Remove(clickedNode);

        //lock nodes that weren't picked and clear open nodes list
        foreach (var node in openNodes)
        {
            node.Lock();
        }
        openNodes.Clear();

        foreach (var node in clickedNode.nextNodes)
        {
            node.Unlock();
            openNodes.Add(node);
        }
    }



    //these two functions are temp here
    public void CloseGame()
    {
        // called from button
        Application.Quit();
    }
    public void RestartRun()
    {
        Debug.Log("Restart game");
    }

}
