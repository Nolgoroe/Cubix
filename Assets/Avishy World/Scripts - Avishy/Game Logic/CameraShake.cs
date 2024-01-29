using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = Camera.main.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = orignalPosition.x + (Random.Range(-1f, 1f) * magnitude);
            float y = orignalPosition.y + (Random.Range(-1f, 1f) * magnitude);
            float z = orignalPosition.z + (Random.Range(-1f, 1f) * magnitude);

            Camera.main.transform.position = new Vector3(x, y, z);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        Camera.main.transform.position = orignalPosition;
    }
}
