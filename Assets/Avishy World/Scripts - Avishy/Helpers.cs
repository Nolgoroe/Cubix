using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Helpers : MonoBehaviour
{
    public static Helpers Instance;

    [Header("Dictionary data")]
    [SerializeField] private Sprite[] allResourceIcons;
    [SerializeField] private Sprite[] allBuffIcons;

    private static Dictionary<ResourceType, Sprite> resourceTypeToIcon;
    private static Dictionary<BuffType, Sprite> buffTypeToIcon;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        resourceTypeToIcon = new Dictionary<ResourceType, Sprite>();
        buffTypeToIcon = new Dictionary<BuffType, Sprite>();

        for (int i = 0; i < allResourceIcons.Length; i++)
        {
            resourceTypeToIcon.Add((ResourceType)i, allResourceIcons[i]);
        }

        for (int i = 0; i < allBuffIcons.Length; i++)
        {
            buffTypeToIcon.Add((BuffType)i, allBuffIcons[i]);
        }
    }

    //Adding the "This" before Transform just means that it becomes an extension method of the Transform type.
    //Without the "This" before it, it can't be used anywhere in the script.
    //Using this we can now call this function on any intance of a transform.
    public static void CenterOnChildred(Transform transformParent)
    {
        List<Transform> children = transformParent.Cast<Transform>().ToList();

        Vector3 pos = Vector3.zero;

        foreach (Transform C in children)
        {
            pos += C.position;
            C.parent = null;
        }

        pos /= children.Count;

        transformParent.position = pos;

        foreach (var C in children)
        {
            C.parent = transformParent;
        }
    }


    public  static IEnumerator SetMat(Renderer renderer, Material Mat, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        renderer.material = Mat;
    }
    public static void SetMatImmediate(Renderer renderer, Material Mat)
    {
        renderer.material = Mat;
    }

    public static void GeneralFloatValueTo(GameObject gameObject, Material mat, float from, float to, float time, LeanTweenType easeType, string keyName, System.Action action = null)
    {
        LeanTween.value(gameObject, from, to, time).setEase(easeType).setOnComplete(action).setOnUpdate((float val) =>
        {
            mat.SetFloat(keyName, val);
        });
    }

    public static Sprite ReturnIconByType(ResourceType resourceType)
    {
        return resourceTypeToIcon[resourceType];
    }

    public static Sprite ReturnIconByType(BuffType resourceType)
    {
        return buffTypeToIcon[resourceType];
    }
}
