using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using NDream.AirConsole;

public class ImageExample : MonoBehaviour
{
   
        // The output of the image
        public RawImage img;

        // The source image
        public string url = "http://portfolio.gmwalek.com/wp-content/uploads/2018/06/AcademicWork.png";
    Texture myTexture; 
    void Start()
    {
        AirConsole.instance.onReady += Ready;
    }

    public void Ready(string code)
    {
        
        url = AirConsole.instance.GetProfilePicture(AirConsole.instance.GetMasterControllerDeviceId());
        Debug.Log("URL of Master Controller: " + url);
        StartCoroutine(GetTexture());

    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Got Texture"); 
            myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            img.texture = myTexture;
            img.rectTransform.sizeDelta = new Vector2(myTexture.width, myTexture.height); 
        }
    }
}
   

