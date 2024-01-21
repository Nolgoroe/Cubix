using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SiteNode : MonoBehaviour
{
    [Header("BaseSettings")]
    public Vector2 gridPos;
    public RectTransform rect;
    public Button button;
    public UnityEvent<SiteNode> OnClicked;
    [SerializeField] private GameObject mapLinePrefab;
    [SerializeField] private GameObject completedIcon;
    [SerializeField] private Color lockedColor;
    [SerializeField] private Color unlockedColor;
    [SerializeField] private Image image;
    [SerializeField] private BaseSite site;

    [Header("Progression")]
    public bool isLocked;
    public int entryLinks;
    public int exitLinks;
    public int singleLinkStreak;
    public List<SiteNode> nextNodes = new List<SiteNode>();
    [Range(0, 1)] public float chanceToLink;
    [SerializeField] private int maxLinks;
    private List<MapLine> lines = new List<MapLine>();

    #region Properties

    public int LinksSum { get { return exitLinks + entryLinks; } }
    public int MaxLinks { get { return maxLinks; } }

    #endregion


    private void OnDestroy()
    {
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }
    }


    public void CreateLink(SiteNode connectNode)
    {
        nextNodes.Add(connectNode);

        MapLine newLine = null;
        GameObject newLineGO = Instantiate(mapLinePrefab, transform.position, Quaternion.identity, transform);
        newLineGO.TryGetComponent<MapLine>(out newLine);
        newLine.Init(this, connectNode);
        newLine.RenderLine();
        connectNode.entryLinks++;
        exitLinks++;
        lines.Add(newLine);
    }

    public void ChangeLinesParent(Transform parent)
    {
        foreach (var line in lines)
        {
            line.transform.SetParent(parent);
        }
    }

    public void Lock()
    {
        isLocked = true;
        image.color = lockedColor;
    }
    public void Unlock()
    {
        isLocked = false;
        image.color = unlockedColor;
    }

    public void Pick()
    {
        isLocked = true;
        completedIcon.SetActive(true);
        site.LaunchSite();
    }

    public void InvokeClicked()
    {
        if (!isLocked)
        {
            Pick();
            OnClicked.Invoke(this);
        }
    }
}
