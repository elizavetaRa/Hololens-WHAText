
using HoloToolkit.Unity;
using UnityEngine;
#if (!UNITY_EDITOR)
using Windows.Media.Ocr;
#endif

public class ApiManager : Singleton<ApiManager>
{

    private IServiceAdaptor Api;
    public OcrService SelectedService = OcrService.MICROSOFTMEDIAOCR;

    protected void Start()
    {
        Debug.Log("Selected Service:" + SelectedService);
#if (!UNITY_EDITOR)
        System.Diagnostics.Debug.WriteLine("Selected Service:" + SelectedService);
#endif
        if (InitSelectedService())
        {
            Api.HttpPostImage();
        }
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
