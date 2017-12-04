
using HoloToolkit.Unity;
using UnityEngine;
using System;
using System.Net.Http;
#if (!UNITY_EDITOR)
using Windows.Media.Ocr;
using System.Threading.Tasks;
#endif

public class ApiManager : Singleton<ApiManager>
{

    private IServiceAdaptor Api;
    private OcrResult result;
    public OcrService SelectedService = OcrService.MICROSOFTMEDIAOCR;

    /// <summary> handles the event when an image was analysed
    public event EventHandler<AnalyseImageEventArgs> ImageAnalysed;

    protected void Start()
    {
        //InitSelectedService();
    }

    /// <summary>
    /// Init Class Instance of Selected Service
    /// </summary>
    private bool InitSelectedService()
    {
        switch (SelectedService)
        {
            case OcrService.MICROSOFTAZUREOCR:
                Api = ApiMicrosoftAzureOcr.Instance;
                return true;
            case OcrService.MICROSOFTMEDIAOCR:
                Api = ApiMicrosoftMediaOcr.Instance;
                return true;
            default:
                Api = ApiMicrosoftMediaOcr.Instance;
                return true;
        }
    }

    public Picture Screenshot {

        get; set;

    }

    /// <summary>
    /// called whenever an image was analysed
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnImageAnalysed(AnalyseImageEventArgs e)
    {
        // send event if there are subscribers
        EventHandler<AnalyseImageEventArgs> handler = ImageAnalysed;
        if (handler != null) handler(this, e);
    }

    public async Task AnalyzeImageAsync(RequestType requestType, Picture screenshot)
    {
        // get correct ocr api instance depending on request type
        switch (requestType)
        {
            case RequestType.REMOTE:
                Api = ApiMicrosoftAzureOcr.Instance;
                break;
            case RequestType.LOCAL:
            default:
                Api = ApiMicrosoftMediaOcr.Instance;
                break;
        }

        Screenshot = screenshot;

#if (!UNITY_EDITOR)
        try
        {
            result = await Api.HttpPostImage(Screenshot);
        }
        catch (HttpRequestException e)
        {
            Api = ApiMicrosoftMediaOcr.Instance;
            result = await Api.HttpPostImage(Screenshot);
        }
#endif

        OnImageAnalysed(new AnalyseImageEventArgs(result));

    }
}

public class AnalyseImageEventArgs : EventArgs
{

    public AnalyseImageEventArgs(OcrResult result)
    {
        Result = result;
    }

    public OcrResult Result
    {
        get; 
    }

}

public enum OcrService
{
    MICROSOFTMEDIAOCR,
    MICROSOFTAZUREOCR
}

/// <summary>
/// Language Identifiers needed for Microsoft.Media.OCR, returning BCP47 normed values
/// </summary>
public static class LanguageId
{
    public static string DE { get { return "de"; } }
    public static string EN { get { return "en"; } }
    public static string ES { get { return "es"; } }
    public static string ZH { get { return "zh"; } }
}

/// <summary>
/// Defines whether request was destined for local or remote OCR
/// </summary>
public enum RequestType
{
    LOCAL,
    REMOTE
}