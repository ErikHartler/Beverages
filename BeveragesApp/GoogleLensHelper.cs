namespace BeveragesApp
{
    public class GoogleLensHelper
    {
        public static async Task<string?> TryDescribePhotoAsync(byte[] imageBytes)
        {
#if ANDROID
            try
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
                {
                    return null;
                }

                using Android.Graphics.Bitmap? bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(
                    imageBytes, 0, imageBytes.Length);
                if (bitmap is null)
                {
                    return null;
                }

                using var imageDescriber = Com.Google.Mlkit.Genai.Imagedescription.ImageDescription.GetClient(
                    Com.Google.Mlkit.Genai.Imagedescription.ImageDescriberOptions.InvokeBuilder(
                        Android.App.Application.Context!).Build());

                using var request = Com.Google.Mlkit.Genai.Imagedescription.ImageDescriptionRequest
                    .InvokeBuilder(bitmap)
                    .Build();

                var result = await Task.Run(() => imageDescriber.RunInference(request).Get());
                using (result)
                {
                    return (result as Com.Google.Mlkit.Genai.Imagedescription.ImageDescriptionResult)?.Description;
                }
            }
            catch
            {
                // The on-device model is available only on supported devices and can fail
                // while AICore is preparing or downloading the model.
                return null;
            }
#else
        await Task.CompletedTask;
        return null;
#endif
        }
    }
}
