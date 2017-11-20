using System;
using System.Collections.Generic;
using System.Diagnostics;
#if (PLATFORM_HOLOLENS)
using Windows.Globalization;
using Windows.Media.Ocr;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
#endif

/// <summary>
/// API Requests with Microsoft.Media.OCR <see href="https://docs.microsoft.com/en-us/uwp/api/windows.media.ocr"/>
/// </summary>
public class ApiMicrosoftMediaOcr : IServiceAdaptor
{

    public delegate void OnGetDataCompleted(string id, string json);

    private static ApiMicrosoftMediaOcr instance = null;

#if (PLATFORM_HOLOLENS)
    private IReadOnlyList<Language> AvailableLanguages;
    private SoftwareBitmap bitmap;
#endif

    private ApiMicrosoftMediaOcr(string preferredLang)
    {
#if (PLATFORM_HOLOLENS)
        CheckForAvailableLanguages();
        this.preferredLang = new Language(preferredLang);

        if (!IsLanguageSupported(this.preferredLang))
        {
            this.preferredLang = AvailableLanguages[0];
        }
#endif

        Debug.WriteLine("Api created.");
    }

    public static ApiMicrosoftMediaOcr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ApiMicrosoftMediaOcr("en");
            }

            return instance;
        }
    }

#if (PLATFORM_HOLOLENS)
    public Language preferredLang
    {
        get
        {
            return this.preferredLang;
        }

        set 
        {
            this.preferredLang = value;
        }
    }
#endif

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

#if (HOLOLENS_PLATFORM)
    /// <summary>
    /// Check for any Ocr languages available on the HoloLens
    /// </summary>
    private void CheckForAvailableLanguages()
    {
        if (OcrEngine.AvailableRecognizerLanguages.Count > 0)
        {
            this.AvailableLanguages = OcrEngine.AvailableRecognizerLanguages;
        }
        else
        {
            Debug.WriteLine("No Ocr Languages on Device available.");
        }
    }

    private bool IsLanguageSupported(Language lang)
    {
        if (!OcrEngine.IsLanguageSupported(lang))
        {
            Debug.WriteLine("Language " + lang.DisplayName + " is not supported");
            return false;
        }
        return true;
    }
#endif

    public void HttpPostImage(string url = null, byte[] jsonBytes = null)
    {
#if (PLATFORM_HOLOLENS)
            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                async (workItem) =>
                {
                    OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(this.preferredLang);

                    if (bitmap.PixelWidth > OcrEngine.MaxImageDimension || bitmap.PixelHeight > OcrEngine.MaxImageDimension)
                    {
                        Debug.WriteLine("Image Resolution not supported.");
                    }
                    else
                    {
                        var ocrResult = await ocrEngine.RecognizeAsync(bitmap);
                        Debug.WriteLine(ocrResult.Text);
                    }
    

                }
           );
 
           asyncAction.Completed = new AsyncActionCompletedHandler(PostDataAsyncCompleted);
#endif
    }

    public void GetDataAsync(string url, string id, OnGetDataCompleted handler)
    {
#if (PLATFORM_HOLOLENS)
            IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                async (workItem) =>
                {
                    try
                    {
                        WebRequest webRequest = WebRequest.Create(url);
                        webRequest.Method = "GET";
                        webRequest.Headers["Content-Type"] = "application/json";
 
                        WebResponse response = await webRequest.GetResponseAsync();
 
                        Stream result = response.GetResponseStream();
                        StreamReader reader = new StreamReader(result);
 
                        string json = reader.ReadToEnd();
 
                        handler(id, json);
                    }
                    catch (Exception)
                    {
                        // handle errors
                    }
                }
            );

            asyncAction.Completed = new AsyncActionCompletedHandler(GetDataAsyncCompleted);
 
#endif
    }

    public void ParseResponseData()
    {
        throw new NotImplementedException();
    }


#if (PLATFORM_HOLOLENS)
        private void PostDataAsyncCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            
        }

        private void GetDataAsyncCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {

        }

        /// <summary>
        /// Loads image from file to bitmap and displays it in UI.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task LoadImage(StorageFile file)
        {
            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);

                bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                var imgSource = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
                bitmap.CopyToBuffer(imgSource.PixelBuffer);
            }
        }

        private async Task LoadSampleImage()
        {
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\Images\\Schriftarten.PNG");
            await LoadImage(file);
        }

#endif
}

/// <summary>
/// holds loaded image event parameters
/// </summary>
public class LoadedImageEventArgs : EventArgs
{
    /// <summary>
    /// constructor for loaded image event parameters
    /// </summary>
    /// <param name="queryId"> the ID of the query the image was loaded for </param>
    /// <param name="keyword"> the keyword the image was loaded for </param>
    /// <param name="imageId"> the ID of the loaded image </param>
    public LoadedImageEventArgs(string queryId, string keyword, string imageId)
    {
        this.keyword = keyword;
        this.queryId = queryId;
        this.imageId = imageId;
    }
    /// <summary>
    /// the ID of the loaded image
    /// </summary>
    private string imageId;
    /// <summary>
    /// the keyword the image was loaded for
    /// </summary>
    private string keyword;
    /// <summary>
    /// the ID of the query the image was loaded for
    /// </summary>
    private string queryId;
    /// <summary>
    /// the ID of the query the image was loaded for
    /// </summary>
    public string QueryId
    {
        get { return queryId; }
    }
    /// <summary>
    /// the keyword the image was loaded for
    /// </summary>
    public string Keyword
    {
        get { return keyword; }
    }
    /// <summary>
    /// the ID of the loaded image
    /// </summary>
    public string ImageId
    {
        get { return imageId; }
    }
}
