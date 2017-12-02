#if (!UNITY_EDITOR)
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage.Streams;
#endif
using Windows.Graphics.Imaging;

/// <summary>
/// Represents Image which gets processed by OCR
/// </summary>
public class Picture
{

    public Picture(byte[] image)
    {
        Task.Run(() => LoadImageFromMem(image));
        AsByteArray = image;
    }

    public byte[] AsByteArray
    {
        get; set;
    }

    public SoftwareBitmap AsSoftwareBitmap
    {
        get; set;
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
            AsSoftwareBitmap = bitmapTemp;
        }
    }

}
