#if (!UNITY_EDITOR)
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Globalization;
#else
using UnityEngine;
#endif

/// <summary>
/// API Requests with Microsofts Azure OCR <see href="https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/"/>
/// </summary>
public class ApiMicrosoftAzureOcr : IServiceAdaptor
{
    public delegate void OnGetDataCompleted(string id, string json);
    private static ApiMicrosoftAzureOcr instance = null;
    private string uri = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/ocr";
    // Number of Maximum Requests Until Fallback to Media.OCR 
    private int maxTries = 3;
    // Delay Time Between Request Sendings in ms
    private int delay = 200;

    private ApiMicrosoftAzureOcr(string ApiKey)
    {
        this.ApiKey = ApiKey;
        this.OcrResult = new OcrResult("", new UnityEngine.Rect(0, 0, 0, 0));
    }

    public static ApiMicrosoftAzureOcr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ApiMicrosoftAzureOcr("0aecf5424f47414caabcf34def172d60");
            }

            return instance;
        }
    }

    public string ApiKey
    {
        get;
        set;
    }

    public string Uri
    {
        get
        {
            return uri;
        }
    }

    public OcrResult OcrResult
    {
        get;
        private set;
    }

#if (!UNITY_EDITOR)
    public Language PreferredLang
    {
        get
        {
            throw new NotImplementedException();
        }
    }
#endif

#if (!UNITY_EDITOR)
    public async Task<OcrResult> HttpPostImage(byte[] jsonBytes = null)
    {
        HttpClient client = new HttpClient();
        // Set Api keys
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
        // Set request headers: always detect text rotation
        string requestParameters = "detectOrientation=true";
        // Set request URI
        string requestUri = Uri + "?" + requestParameters;
        HttpResponseMessage response;

        // Request body. Posts a locally stored JPEG image.
        byte[] byteData = await GetImageAsByteArray("Assets\\Schriftarten.PNG");

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
                    System.Diagnostics.Debug.WriteLine("Try Nr " + count);

                    // Execute the REST API call.
                    response = await client.PostAsync(uri, content);

                    // Get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();

                    // Display the JSON response.
                    System.Diagnostics.Debug.WriteLine("\nResponse:\n");
                    System.Diagnostics.Debug.WriteLine(JsonPrettyPrint(contentString));

                    ParseResponseData(contentString);
                    System.Diagnostics.Debug.WriteLine(OcrResult);
                    break;
                }

                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Api Request Error: " + e);
                    await Task.Delay(delay);
                    count++;
                }
            }
        }

        return OcrResult;
    }

    /// <summary>
    /// Returns the contents of the specified file as a byte array.
    /// </summary>
    /// <param name="imageFilePath">The image file to read.</param>
    /// <returns>The byte array of the image data.</returns>
    private async Task<byte[]> GetImageAsByteArray(string imageFilePath)
    {
        using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
        {
            byte[] result = new byte[fileStream.Length];
            await fileStream.ReadAsync(result, 0, (int)fileStream.Length);
            return result;
        }
    }

    /// <summary>
    /// Formats the given JSON string by adding line breaks and indents.
    /// </summary>
    /// <param name="json">The raw JSON string to format.</param>
    /// <returns>The formatted JSON string.</returns>
    private string JsonPrettyPrint(string json)
    {
        if (string.IsNullOrEmpty(json))
            return string.Empty;

        json = json.Replace(Environment.NewLine, "").Replace("\t", "");

        StringBuilder sb = new StringBuilder();
        bool quote = false;
        bool ignore = false;
        int offset = 0;
        int indentLength = 3;

        foreach (char ch in json)
        {
            switch (ch)
            {
                case '"':
                    if (!ignore) quote = !quote;
                    break;
                case '\'':
                    if (quote) ignore = !ignore;
                    break;
            }

            if (quote)
                sb.Append(ch);
            else
            {
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', ++offset * indentLength));
                        break;
                    case '}':
                    case ']':
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', --offset * indentLength));
                        sb.Append(ch);
                        break;
                    case ',':
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', offset * indentLength));
                        break;
                    case ':':
                        sb.Append(ch);
                        sb.Append(' ');
                        break;
                    default:
                        if (ch != ' ') sb.Append(ch);
                        break;
                }
            }
        }
        return sb.ToString().Trim();
    }

#endif

    public void ParseResponseData(object response)
    {
#if (!UNITY_EDITOR)
        if ((string)response == "" || response == null)
        {
            System.Diagnostics.Debug.WriteLine("No Text recognized");
        }
        else
        {
            var responseTemp = JsonConvert.DeserializeObject<MicrosoftAzureResult.RootObject>((string)response);

            // Find bounding box coords which surround the entire recognized text block
            float xMin, yMin, xMax, yMax;
            xMin = yMin = xMax = yMax = 0;
            float xMinTemp, yMinTemp, xMaxTemp, yMaxTemp;
            string text = "";

            for (int i = 0; i < responseTemp.regions.Count; i++)
            {
                // convert string of bounding box numbers to int array
                // pattern: boundingBox = [x, y, width, height]
                string[] tmp = responseTemp.regions[i].boundingBox.Split(',');
                int[] boundingBoxTemp = tmp.Select(s => Int32.Parse(s)).ToArray();

                xMinTemp = boundingBoxTemp[0];
                yMinTemp = boundingBoxTemp[1];
                xMaxTemp = boundingBoxTemp[0] + boundingBoxTemp[2];
                yMaxTemp = boundingBoxTemp[1] + boundingBoxTemp[3];

                if (i == 0)
                {
                    xMin = xMinTemp;
                    xMax = xMaxTemp;
                    yMin = yMinTemp;
                    yMax = yMaxTemp;
                }
                else
                {
                    if (xMinTemp < xMin) xMin = xMinTemp;
                    if (xMaxTemp > xMax) xMax = xMaxTemp;
                    if (yMinTemp < yMin) yMin = yMinTemp;
                    if (yMaxTemp > yMax) yMax = yMaxTemp;
                }

                for (int j = 0; j < responseTemp.regions[i].lines.Count; j++)
                {
                    for (int k = 0; k < responseTemp.regions[i].lines[j].words.Count; k++)
                    {
                        text = text + " " + responseTemp.regions[i].lines[j].words[k].text;
                    }
                   
                }
            }

            this.OcrResult = new OcrResult(text, new UnityEngine.Rect(xMin, yMin, (xMax - xMin), (yMax - yMin)));
        }
           

       
#endif
    }
}