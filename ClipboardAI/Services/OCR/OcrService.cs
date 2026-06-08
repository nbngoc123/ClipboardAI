using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace ClipboardAI.Services.OCR;

public class OcrService : IOcrService
{
    public async Task<string> ExtractTextFromImageAsync(BitmapSource image)
    {
        if (image == null) throw new ArgumentNullException(nameof(image));

        using var memoryStream = new MemoryStream();
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(image));
        encoder.Save(memoryStream);
        memoryStream.Position = 0;

        using var winrtStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
        var outStream = winrtStream.AsStreamForWrite();
        await memoryStream.CopyToAsync(outStream);
        await outStream.FlushAsync();
        winrtStream.Seek(0);

        var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(winrtStream);
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        // 4. Initialize OcrEngine
        var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
        if (ocrEngine == null)
        {
            throw new NotSupportedException("OCR is not supported on this device or for the user profile languages.");
        }

        // 5. Recognize Text
        var ocrResult = await ocrEngine.RecognizeAsync(softwareBitmap);

        return ocrResult.Text;
    }
}
