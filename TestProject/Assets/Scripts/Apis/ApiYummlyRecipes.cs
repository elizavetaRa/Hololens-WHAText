#if (!UNITY_EDITOR)
using System;
using System.Threading.Tasks;
using System.Net.Http;
#endif
using UnityEngine;

/// <summary>
/// API Requests with Yummly Recipes API
/// </summary>
public class ApiYummlyRecipes
{

    public delegate void OnGetDataCompleted(string id, string json);
    private static ApiYummlyRecipes instance = null;
    public string apiKey = "";
    private string uri = "https://api.yummly.com/v1";
    // Number of Maximum Requests Until Error Message Gets Shown
    private int maxTries = 3;
    // Delay Time Between Request Sendings in ms
    private int delay = 200;

    private ApiYummlyRecipes(string ApiKey)
    {
        if (ApiKey != "" && ApiKey != null)
            this.ApiKey = ApiKey;

        Debug.LogError("<YummlyRecipes> Api created");
    }

    public static ApiYummlyRecipes Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ApiYummlyRecipes("");
            }

            return instance;
        }
    }

    public RecipeResult RecipeResult
    {
        get;
        private set;
    }

    public string ApiKey
    {
        get
        {
            return null;
        }

        set
        {
            return;
        }
    }

    public string Uri
    {
        get
        {
            return null;
        }

        set
        {
            return;
        }
    }

#if (!UNITY_EDITOR)
    public async Task<OcrResult> HttpPostImage(string[] ingredients)
    {
        RecipeResult = null;
        HttpClient client = new HttpClient();

        // Setting headers
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
        client.DefaultRequestHeaders.Add("X-Yummly-App-ID", "");
        client.DefaultRequestHeaders.Add("X-Yummly-App-Key", "");

        // Set request URI
        string allowedIngredientsParam = "?";
        string requestUri = Uri + "?allowedIngredient[]=" + requestString;
        HttpResponseMessage response;

        // Request body.
        byte[] byteData;
        if (screenshot == null)
            byteData = await GetImageAsByteArray("Assets\\Schriftarten.PNG");
        else
            byteData = screenshot.AsJPEG;

        using (ByteArrayContent content = new ByteArrayContent(byteData))
        {
            // Using octet-stream for local image location
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // execute request maxTries times until fallback to Media.OCR is used
            int count = 0;

            while (count < maxTries)
            {
                try
                {
                    Debug.LogError("<AzureOCR> Try Nr " + count);

                    // Execute the REST API call.
                    response = await client.PostAsync(uri, content);

                    // Get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();

                    // Display the JSON response.
                    //System.Diagnostics.Debug.WriteLine("\nResponse:\n");
                    //System.Diagnostics.Debug.WriteLine(JsonPrettyPrint(contentString));

                    ParseResponseData(contentString);
                    Debug.LogError("<AzureOCR> " + OcrResult.Text);
                    break;
                }

                catch (HttpRequestException httpException)
                {
                    Debug.LogError("<AzureOCR> Network Error.");
                    throw new HttpRequestException("HTTP Request Error");
                }

                catch (Exception e)
                {
                    Debug.LogError("<AzureOCR> Api Request Error: " + e);
                    await Task.Delay(delay);
                    count++;
                }

            }
        }

        //System.Diagnostics.Debug.WriteLine("OCR finished");
        return OcrResult;
    }
#endif


}