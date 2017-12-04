#if (!UNITY_EDITOR)
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
#endif

/// <summary>
/// Represents Image which gets processed by OCR
/// </summary>
public class Picture
{
#if (!UNITY_EDITOR)
    private SoftwareBitmap pictureAsSoftwareBitmap;
#endif

    public Picture(byte[] image)
    {
        AsByteArray = image;
    }

    public byte[] AsByteArray
    {
        get; set;
    }

#if (!UNITY_EDITOR)
    public async Task<SoftwareBitmap> AsSoftwareBitmap()
    {
        if (this.pictureAsSoftwareBitmap == null)
            await LoadImageFromMem(AsByteArray);
        return this.pictureAsSoftwareBitmap;
    }

    public async Task<Bitmap> AsBitmap()
    {

    }

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
            this.pictureAsSoftwareBitmap = bitmapTemp;
        }
    }
#endif
}
