
using HoloToolkit.Unity;
using UnityEngine;
using System;
#if (!UNITY_EDITOR)
using Windows.Media.Ocr;
using System.Threading.Tasks;
#endif

public class ApiManager : Singleton<ApiManager>
{

    private IServiceAdaptor Api;
    public OcrService SelectedService = OcrService.MICROSOFTMEDIAOCR;

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

    internal void AnalyzeImage(RequestType requestType, Picture screenshot)
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
        var result = Task.Run(() => Api.HttpPostImage(Screenshot));
#endif

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