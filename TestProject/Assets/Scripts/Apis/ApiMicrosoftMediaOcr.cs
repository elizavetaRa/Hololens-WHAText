#if (!UNITY_EDITOR)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Media.Ocr;
using Windows.Graphics.Imaging;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

/// <summary>
/// API Requests with Microsoft.Media.OCR <see href="https://docs.microsoft.com/en-us/uwp/api/windows.media.ocr"/>
/// </summary>
public class ApiMicrosoftMediaOcr : IServiceAdaptor
{

    public delegate void OnGetDataCompleted(string id, string json);
    private static ApiMicrosoftMediaOcr instance = null;

#if (!UNITY_EDITOR)
    private Language preferredLang;
    private IReadOnlyList<Language> AvailableLanguages;
    private SoftwareBitmap bitmap;
#endif

    private ApiMicrosoftMediaOcr(string preferredLang)
    {
#if (!UNITY_EDITOR)
        // If preferred language does not exist, use the first one which is available by default
        CheckForAvailableLanguages();
        this.preferredLang = new Language(preferredLang);

        if (!IsLanguageSupported(PreferredLang))
        {
            PreferredLang = AvailableLanguages[0];
        }

        Debug.WriteLine("Api created with Language " + PreferredLang.DisplayName);
#endif
    }

    public static ApiMicrosoftMediaOcr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ApiMicrosoftMediaOcr(LanguageId.DE);
            }

            return instance;
        }
    }

#if (!UNITY_EDITOR)
    public Language PreferredLang
    {
        get
        {
            return preferredLang;
        }

        set
        {
            preferredLang = value;
        }
    }
#endif

    public OcrResult OcrResult
    {
        get;
        private set;
    }

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

#if (!UNITY_EDITOR)
    /// <summary>
    /// Check for any Ocr languages available on the HoloLens
    /// </summary>
    private void CheckForAvailableLanguages()
    {
        if (OcrEngine.AvailableRecognizerLanguages.Count > 0)
        {
            for (var i = 0; i < OcrEngine.AvailableRecognizerLanguages.Count; i++)
            {
                Debug.WriteLine("Supported: " + OcrEngine.AvailableRecognizerLanguages[i].DisplayName);
            }
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

    /*
    public void HttpPostImage(string url = null, byte[] jsonBytes = null)
    {
#if (!UNITY_EDITOR)
        Debug.WriteLine("Ocr started");
        IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
            async (workItem) =>
            {
                OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(PreferredLang);
                await LoadSampleImage();

                if (bitmap.PixelWidth > OcrEngine.MaxImageDimension || bitmap.PixelHeight > OcrEngine.MaxImageDimension)
                {
                    Debug.WriteLine("Image Resolution not supported.");
                }
                else
                {
                    var ocrResult = await ocrEngine.RecognizeAsync(bitmap);
                    ParseResponseData(ocrResult);
                }
            }
        );
 
        asyncAction.Completed = new AsyncActionCompletedHandler(
            (IAsyncAction asyncInfo, AsyncStatus asyncStatus) =>
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.High,
                    new DispatchedHandler(() =>
                    {
                        Debug.WriteLine(this.OcrResult);
                    })
                );
            }
        );
#endif
    }
    */

    public async Task<OcrResult> HttpPostImage(byte[] screenshot = null)
    {
        OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(PreferredLang);

        if (screenshot == null)
            await LoadSampleImageFromFile();
        else
            await LoadImageFromMem(screenshot);

        if (bitmap.PixelWidth > OcrEngine.MaxImageDimension || bitmap.PixelHeight > OcrEngine.MaxImageDimension)
        {
            Debug.WriteLine("Image Resolution not supported.");
        }
        else
        {
            var ocrResult = await ocrEngine.RecognizeAsync(bitmap);
            ParseResponseData(ocrResult);
        }
        Debug.WriteLine("OCR finished");
        return OcrResult;
    }
#endif

    public void ParseResponseData(object response)
    {
#if (!UNITY_EDITOR)
        Windows.Media.Ocr.OcrResult responseTemp = (Windows.Media.Ocr.OcrResult)response;

        if (responseTemp.Text == "" || responseTemp.Text == null)
        {
            Debug.WriteLine("No Text recognized");
            this.OcrResult = new OcrResult("", new UnityEngine.Rect(0, 0, 0, 0));
        }
        else
        {
            // Find bounding box coords which surround the entire recognized text block
            float xMin, yMin, xMax, yMax;
            xMin = yMin = xMax = yMax = 0;
            float xMinTemp, yMinTemp, xMaxTemp, yMaxTemp;


            for (int i = 0; i < responseTemp.Lines.Count; i++)
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


#if (!UNITY_EDITOR)

    /// <summary>
    /// Loads image from memory and copies it back to UI thread
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    private async Task LoadImageFromMem(byte[] image)
    {
        using (InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream())
        {
            // write image to memory stream
            await randomAccessStream.WriteAsync(image.AsBuffer());

            // instantiate decoder for creation of SoftwareBitmap needed by local OCR
            var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

            var bitmapTemp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            bitmap = bitmapTemp;
        }
    }

    /// <summary>
    /// Loads image from file and copies it back to UI thread
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task LoadImageFromFile(StorageFile file)
    {
        using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var bitmapTemp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            bitmap = bitmapTemp;
        }
    }

    private async Task LoadSampleImageFromFile()
    {
        System.Diagnostics.Debug.WriteLine(Windows.ApplicationModel.Package.Current.InstalledLocation);
        var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\Schriftarten.PNG");
        await LoadImageFromFile(file);
    }

#endif
}