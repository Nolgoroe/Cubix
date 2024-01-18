using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadersControl : MonoBehaviour {

    float timer = 0;
    float timerSciFi = 0;

    public static bool doNow = false;
	void Update () {

        if (!doNow) return;
        timer += Time.deltaTime * GameManager.gameSpeed;

        
        Shader.SetGlobalFloat("_ShaderDisplacement", timer);
        Shader.SetGlobalFloat("_ShaderHologramDisplacement", 1 - timer);
        Shader.SetGlobalFloat("_ShaderDissolve", 1 - timer);

        if (timerSciFi > 0.9f)
        {
            timerSciFi = 0;
            doNow = false;
        }

        timerSciFi += Time.deltaTime * GameManager.gameSpeed;

        Shader.SetGlobalFloat("_ShaderSciFi", timerSciFi);
        
    }
}
