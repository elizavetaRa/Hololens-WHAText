
public class ApiManager
{

    private IServiceAdaptor Api;
    private IServiceAdaptor selectedService;

    public ApiManager(OcrService selectedService)
    {
        switch(selectedService)
        {
            case OcrService.MICROSOFTMEDIAOCR:
            {
                Api = ApiMicrosoftMediaOcr.Instance;
                break;
            }
            default:
            {
                Api = ApiMicrosoftMediaOcr.Instance;
                break;
            }
        }
    }

    public IServiceAdaptor SelectedService
    {
        get
        {
            return selectedService;
        }

        set
        {
            selectedService = value;
        }
    }

    public bool StartRequest()
    {
        Api.HttpPostImage();
        return true;
    }

}

public enum OcrService
{
    MICROSOFTMEDIAOCR,
    GOOGLEVISIONOCR
}
