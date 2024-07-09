using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class API_Manager : MonoBehaviour
{
    private string JsonString;
    private string APILink;
    public MonitorManager monitorManager;


    //public ImagePrompt imagePrompt;
    // Start is called before the first frame update
    void Start()
    {
        APILink = "https://backend-server-tqhm.onrender.com";

        GetAllImages();
    }

    public void GetAllImages()
    {
        string uri = APILink + "/readall"; //building url
        StartCoroutine(GetAllImagesRequest(uri));
    }
   
    public void RequestImage(string prompt)
    {
      
        string uri = APILink + "/robin/" + prompt; //building url
        StartCoroutine(GetRobinAPIRequest(uri));
        Debug.Log("your code is down");
    }
    // Update is called once per frame
    
    void Update()
    {
        
    }
    IEnumerator GetRobinAPIRequest(string uri)
    {
        //imagePrompt.StartLoadingText();
        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();
      
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                break;
            case UnityWebRequest.Result.DataProcessingError:
               Debug.Log("Error1");
                break;
            case UnityWebRequest.Result.ProtocolError:
               Debug.Log("Error2");
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                JsonString = webRequest.downloadHandler.text; //json string
                Imagejson json = Imagejson.CreateFromJSON(JsonString); //json object

                json.printInfo();
                monitorManager.current.setImage(json.imageURL);
                //StartCoroutine(GetAllImagesRequest(APILink + "/readall"));
                Debug.Log(json.imageURL);
                break;
        }

    }

    IEnumerator GetAllImagesRequest(string uri)
    {
        //imagePrompt.StartLoadingText();
        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                break;
            case UnityWebRequest.Result.ProtocolError:
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                JsonString = webRequest.downloadHandler.text; //json string

                var results = JsonConvert.DeserializeObject<Root>(JsonString);
                //var result = JsonConvert.DeserializeObject<Artifact[]>(JsonString);

                monitorManager.artifacts = results.artifacts;

                int lastIndex = monitorManager.artifacts.Count - 1;
                var urlCurrent = monitorManager.artifacts[lastIndex-1].imageURL;
                var urlPrevious = monitorManager.artifacts[lastIndex - 2].imageURL;
                var urlNext = monitorManager.artifacts[lastIndex].imageURL;

                monitorManager.next.setImage(urlNext, lastIndex);
                monitorManager.current.setImage(urlCurrent, lastIndex-1);
                monitorManager.previous.setImage(urlPrevious, lastIndex - 2);
                break;
        }

    }

}
