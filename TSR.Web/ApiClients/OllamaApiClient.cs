namespace TSR.Web.ApiClients;

public class OllamaApiClient()
{
    public async Task<float[]?> CreateVector(byte[]? data, string fileName, CancellationToken cancellationToken = default)
    {
        float[]? farr = null;
        if (data is not null)
        {
            ApiService.ImageVectorGenerator imageVectorGenerator = new ApiService.ImageVectorGenerator();

            farr = await imageVectorGenerator.GenerateVectorFromImage(data, fileName);
            Console.WriteLine(farr.Length);
        }
        return farr;
    }
}