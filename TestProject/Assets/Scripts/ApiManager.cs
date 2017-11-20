
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
            case OcrService.GOOGLEVISIONOCR:
#if (!UNITY_EDITOR)
                System.Diagnostics.Debug.WriteLine("Service not supported at the moment.");
#endif
                return false;
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
    GOOGLEVISIONOCR
}
