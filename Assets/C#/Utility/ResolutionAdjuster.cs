using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionAdjuster : MonoBehaviour
{
    public SpriteRenderer rink;
    public Transform launcher,backwall;
    private void Awake()
    {

        float y = transform.position.y - 11;
        verticalFit();
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
           launcher. transform.position=new Vector3(launcher.position.x,launcher.position.y-2,launcher.position.z);       
            transform.position=new Vector3(transform.position.x,-11,transform.position.z);       
            backwall.transform.position=new Vector3(launcher.position.x,backwall.position.y-2,launcher.position.z); 
#endif

    }
    // Start is called before the first frame update
    void Start()
    {

        //Camera.main.orthographicSize = rink.bounds.size.x * Screen.height / Screen.width * 0.5f;

    
    }

    void horizontalFit()
    {
        Camera.main.orthographicSize = rink.bounds.size.y / 2;
    }
    void verticalFit()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = rink.bounds.size.x / rink.bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = rink.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = rink.bounds.size.y / 2 * differenceInSize;
        }
    }
}
