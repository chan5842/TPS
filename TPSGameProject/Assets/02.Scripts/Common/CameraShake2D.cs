using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake2D : MonoBehaviour
{
    Vector3 PosCam;
    Quaternion RotCam;
    Transform camTr;

    public bool isShake;
    float timePrev;
    float duration = 0.5f;      // 지속 시간

    void Start()
    {
        camTr = GetComponent<Transform>();
        
    }

    public void TurnOn()
    {
        if(!isShake)
            isShake = true;
        timePrev = Time.time;
        PosCam = camTr.localPosition;
        RotCam = camTr.localRotation;

    }

    void Update()
    {
        if(isShake)
        {
            float x = Random.Range(-0.5f, 0.5f);
            float y = Random.Range(-0.05f, 0.5f);

            Camera.main.transform.position += new Vector3(x, y, 0);
            Camera.main.transform.localEulerAngles += new Vector3(x, y, 0f);

            if(Time.time - timePrev > duration)
            {
                isShake = false;
                Camera.main.transform.position = PosCam;
                Camera.main.transform.localRotation = RotCam;
            }
        }
    }
}
