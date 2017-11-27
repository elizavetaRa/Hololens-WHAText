#if (!UNITY_EDITOR)
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
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

    private ApiMicrosoftAzureOcr(string ApiKey)
    {
        this.ApiKey = ApiKey;
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

    public async Task<OcrResult> HttpPostImage(string url = null, byte[] jsonBytes = null)
    {
#if (!UNITY_EDITOR)
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

            try
            {
                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                System.Diagnostics.Debug.WriteLine("\nResponse:\n");
                System.Diagnostics.Debug.WriteLine(JsonPrettyPrint(contentString));
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Api Request Error: " + e);
            }
        }

        return OcrResult;
#endif
    }

#if (!UNITY_EDITOR)

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
            this.OcrResult = new OcrResult("", new UnityEngine.Rect(0, 0, 0, 0));
        }
        else
        {
            var responseTemp = JObject.Parse((string)response);
            // Find bounding box coords which surround the entire recognized text block
            float xMin, yMin, xMax, yMax;
            xMin = yMin = xMax = yMax = 0;
            float xMinTemp, yMinTemp, xMaxTemp, yMaxTemp;

            for (int i = 0; i < responseTemp.SelectToken("regions").; i++)
            {
                for (int j = 0; j < responseTemp.Lines[i].Words.Count; j++)
                {
                    xMinTemp = (float)responseTemp.Lines[i].Words[j].BoundingRect.X;
                    yMinTemp = (float)responseTemp.Lines[i].Words[j].BoundingRect.Y;
                    xMaxTemp = (float)(responseTemp.Lines[i].Words[j].BoundingRect.X + responseTemp.Lines[i].Words[j].BoundingRect.Width);
                    yMaxTemp = (float)(responseTemp.Lines[i].Words[j].BoundingRect.Y + responseTemp.Lines[i].Words[j].BoundingRect.Height);

                    if (i == 0 && j == 0)
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
                }
            }

            this.OcrResult = new OcrResult(responseTemp.Text, new UnityEngine.Rect(xMin, yMin, (xMax - xMin), (yMax - yMin)));
        }
           

       
#endif
    }

}