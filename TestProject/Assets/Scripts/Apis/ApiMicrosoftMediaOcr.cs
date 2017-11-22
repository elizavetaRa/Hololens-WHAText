﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.IO;
#if (!UNITY_EDITOR)
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Media.Ocr;
using Windows.Foundation;
using Windows.Graphics.Imaging;
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
#endif

    public void HttpPostImage(string url = null, byte[] jsonBytes = null)
    {
#if (!UNITY_EDITOR)
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
                    Debug.WriteLine("Resulting Text: " + ocrResult.Text);
                    ParseResponseData(ocrResult);
                }
            }
        );
 
        asyncAction.Completed = new AsyncActionCompletedHandler(PostDataAsyncCompleted);
#endif
    }

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
            Debug.WriteLine("BoundingBox\nxMin:" + this.OcrResult.BoundingBox.x, "\nyMin: " + this.OcrResult.BoundingBox.y + "\nWidth: " + this.OcrResult.BoundingBox.width + "\nHeight: " + this.OcrResult.BoundingBox.height);
        }
#endif
    }


#if (!UNITY_EDITOR)
        private void PostDataAsyncCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
#if (!UNITY_EDITOR)
            Debug.WriteLine("OCR has finished.\nStatus: " + asyncStatus + "\nInfo: " + asyncInfo);
#endif
        }

        /// <summary>
        /// Loads image from file and copies it back to UI thread
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task LoadImage(StorageFile file)
        {
            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);

                var bitmapTemp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);


                bitmap = bitmapTemp;
            }
        }

        private async Task LoadSampleImage()
        {
            System.Diagnostics.Debug.WriteLine(Windows.ApplicationModel.Package.Current.InstalledLocation);
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\Schriftarten.PNG");
            await LoadImage(file);
        }

#endif
}