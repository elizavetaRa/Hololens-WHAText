
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
    // Maximum Request Executions When Errors Occur
    private int maxRequests = 3;

    protected void Start()
    {
        Debug.Log("Selected Service:" + SelectedService);
#if (!UNITY_EDITOR)
        System.Diagnostics.Debug.WriteLine("Selected Service:" + SelectedService);
        if (InitSelectedService())
        {
            var result = Task.Run(() => Api.HttpPostImage());
        }
#endif
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