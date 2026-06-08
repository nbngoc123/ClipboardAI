using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ClipboardAI.Services.OCR;

public interface IOcrService
{
    Task<string> ExtractTextFromImageAsync(BitmapSource image);
}
