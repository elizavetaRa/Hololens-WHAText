#if (!UNITY_EDITOR)
using System;
using System.Threading.Tasks;
using System.Net.Http;
#endif
using AssemblyCSharpWSA.Scripts.Utils;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// API Requests with Yummly Recipes API
/// </summary>
public class ApiYummlyRecipes
{

    public delegate void OnGetDataCompleted(string id, string json);
    private static ApiYummlyRecipes instance = null;
    public string apiKey = "";
    private string uri = "https://api.yummly.com/v1/api/recipes";
    // Number of Maximum Requests Until Error Message Gets Shown
    private int maxTries = 3;
    // Delay Time Between Request Sendings in ms
    private int delay = 200;
    private DialogPanel dialogPanel;

    private ApiYummlyRecipes(string ApiKey)
    {
        if (ApiKey != "" && ApiKey != null)
            this.ApiKey = ApiKey;

        Debug.LogError("<YummlyRecipes> Api created");
        dialogPanel = DialogPanel.Instance();
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

    public RecipeResult.RootObject RecipeResult
    {
        get;
        private set;
    }

    public string ApiKey
    {
        get
        {
            return apiKey;
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
            return uri;
        }

        set
        {
            return;
        }
    }

#if (!UNITY_EDITOR)
    public async Task<RecipeResult.RootObject> HttpGetRecipesByIngredients(string[] ingredients)
    {
        RecipeResult = null;
        HttpClient client = new HttpClient();

        List<string> queryParameters = new List<string>();

        // Setting headers
        client.DefaultRequestHeaders.Add("X-Yummly-App-ID", "4ac0164f");
        client.DefaultRequestHeaders.Add("X-Yummly-App-Key", "ced2ceb7ccf7410227ba6dcb72020c5e");

        // Set request URI
        string requirePicturesParam = "requirePictures=false"; 
        string allowedIngredientsParamName = "allowedIngredient[]=";

        for (int i = 0; i < ingredients.Length; i++)
        {
            queryParameters.Add(allowedIngredientsParamName + ingredients[i]);
        }

        queryParameters.Add(requirePicturesParam);

        string requestUri = Uri + "?" + String.Join("&", queryParameters);
        Debug.LogError(requestUri);
        HttpResponseMessage response;

        int count = 0;

        while (count < maxTries)
        {
            try
            {
                Debug.LogError("<YummlyRecipe> Try Nr " + count);

                // Execute the REST API call.
                response = await client.GetAsync(requestUri);

                Debug.LogError(response);

                // Get response string
                string contentString = await response.Content.ReadAsStringAsync();

                if (contentString == "" || contentString == null)
                {
                    Debug.LogError("<YummlyRecipe> No recipes found");
                    dialogPanel.enqueueNotification("No Recipes found. Try another search input.");
                }
                else
                {
                    // Debug.LogError(contentString);
                    RecipeResult = JsonConvert.DeserializeObject<RecipeResult.RootObject>(contentString);
                    return RecipeResult;
                    Debug.LogError("<YummlyRecipe> " + RecipeResult.totalMatchCount + " Recipes found.");
                }

                break;
            }

            catch (HttpRequestException httpException)
            {
                Debug.LogError("<YummlyRecipe> Network Error.");
                throw new HttpRequestException("HTTP Request Error");
            }

            catch (Exception e)
            {
                Debug.LogError("<YummlyRecipe> Api Request Error: " + e);
                await Task.Delay(delay);
                count++;
            }

        }

        dialogPanel.enqueueNotification("Network Error. Please try again.");
        return null;
    }
#endif



}