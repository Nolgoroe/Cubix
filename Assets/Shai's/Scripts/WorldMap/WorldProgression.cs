using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldProgression : MonoBehaviour
{
    [SerializeField] private WorldGrid worldGrid;
    private List<SiteNode> openNodes = new List<SiteNode>();

    private void Start()
    {
        //temp function

        Player.Instance.InitPlayer(); // this also updates the UI health and resource count

        UIManager.Instance.UpdateMapDiceDisplay();
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

}
