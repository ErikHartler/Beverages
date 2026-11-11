using Microsoft.Extensions.Logging;

namespace BeveragesApp.Helpers;

public static class GoogleLensHelper
{
    private static readonly ILogger _logger = PageLogging.CreateLogger(nameof(GoogleLensHelper));
    
    public static async Task<string?> TryDescribePhotoAsync(byte[] imageBytes)
    {
#if ANDROID
        try
        {
            if (imageBytes is not { Length: > 0 })
            {
                _logger.LogWarning("Image description skipped because the photo is empty");
                return null;
            }

            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
            {
                _logger.LogInformation("Image description requires Android 8.0 or newer");
                return "Image description requires Android 8.0 or newer";
            }

            using Android.Graphics.Bitmap? bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(
                imageBytes, 0, imageBytes.Length);
            if (bitmap is null)
            {
                _logger.LogWarning("Image description skipped because the photo could not be decoded");
                return "Image description skipped because the photo could not be decoded";
            }

            using var imageDescriber = Com.Google.Mlkit.Genai.Imagedescription.ImageDescription.GetClient(
                Com.Google.Mlkit.Genai.Imagedescription.ImageDescriberOptions.InvokeBuilder(
                    Android.App.Application.Context!).Build());

            using var featureStatusResult = await Task.Run(() => imageDescriber.CheckFeatureStatus().Get());
            int featureStatus = (featureStatusResult as Java.Lang.Integer)?.IntValue() ?? -1;
            const int unavailable = 0;
            const int downloadable = 1;
            const int downloading = 2;

            if (featureStatus == unavailable)
            {
                _logger.LogInformation("On-device image description is unavailable on this device");
                return "On-device image description is unavailable on this device";
            }

            if (featureStatus is downloadable or downloading)
            {
                _logger.LogInformation("Preparing the on-device image description model");
                await Task.Run(() => imageDescriber.PrepareInferenceEngine().Get());
            }

            using var request = Com.Google.Mlkit.Genai.Imagedescription.ImageDescriptionRequest
                .InvokeBuilder(bitmap)
                .Build();

            var result = await Task.Run(() => imageDescriber.RunInference(request).Get());
            using (result)
            {
                string? description = (result as Com.Google.Mlkit.Genai.Imagedescription.ImageDescriptionResult)?.Description;
                if (string.IsNullOrWhiteSpace(description))
                {
                    _logger.LogWarning("The on-device image description model returned no description");
                    return "The on-device image description model returned no description";
                }

                description = description.Trim();
                _logger.LogInformation("Generated an on-device image description");
                return description;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate an on-device image description");
            return null;
        }
#else
        await Task.CompletedTask;
        return null;
#endif
    }
}
