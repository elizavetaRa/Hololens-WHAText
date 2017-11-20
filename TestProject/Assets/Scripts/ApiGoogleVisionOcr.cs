using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
#if (!UNITY_EDITOR)
using Windows.Foundation;
#endif

/// <summary>
/// API Requests with Microsoft.Media.OCR <see href="https://cloud.google.com/vision/?hl=de"/>
/// </summary>
public class ApiGoogleVisionOcr
{

    public delegate void OnGetDataCompleted(string id, string json);

    private static ApiGoogleVisionOcr instance = null;

    private ApiGoogleVisionOcr()
    {
    }

    public static ApiGoogleVisionOcr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ApiGoogleVisionOcr();
            }

            return instance;
        }
    }

    public string ApiKey
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    public string Uri { get; set; }

    public void HttpPostImage(string url, byte[] jsonBytes)
    {
#if (!UNITY_EDITOR)
            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                async (workItem) =>
                {
                    WebRequest webRequest = WebRequest.Create(url);
                    webRequest.Method = "POST";
                    webRequest.Headers["Content-Type"] = "application/json";
 
                    Stream stream = await webRequest.GetRequestStreamAsync();
                    stream.Write(jsonBytes, 0, jsonBytes.Length);
 
                    WebResponse response = await webRequest.GetResponseAsync();
                }
           );
 
           asyncAction.Completed = new AsyncActionCompletedHandler(PostDataAsyncCompleted);
#endif
    }

    public void GetDataAsync(string url, string id, OnGetDataCompleted handler)
    {
#if (!UNITY_EDITOR)
            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                async (workItem) =>
                {
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(url);
                        webRequest.Method = "GET";
                        webRequest.Headers["Content-Type"] = "application/json";
 
                        WebResponse response = await webRequest.GetResponseAsync();
 
                        Stream result = response.GetResponseStream();
                        StreamReader reader = new StreamReader(result);
 
                        string json = reader.ReadToEnd();
 
                        handler(id, json);
                    }
                    catch (Exception)
                    {
                        // handle errors
                    }
                }
            );

            asyncAction.Completed = new AsyncActionCompletedHandler(GetDataAsyncCompleted);
 
#endif
    }

    public void ParseResponseData()
    {
        throw new NotImplementedException();
    }

#if (!UNITY_EDITOR)
        private void PostDataAsyncCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            
        }

        private void GetDataAsyncCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {

        }
#endif
}