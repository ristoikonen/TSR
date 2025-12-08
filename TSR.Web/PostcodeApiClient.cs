namespace TSR.Web;

public class PostcodeApiClient(HttpClient httpClient)
{
    public async Task<TSR.Web.Models.AustralianPostcode?> GetPostcodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;

        try
        {
            return await httpClient.GetFromJsonAsync<TSR.Web.Models.AustralianPostcode>($"https://localhost:7452/postcode/{Uri.EscapeDataString(code)}", cancellationToken);
        }
        catch (HttpRequestException)
        {
            // bubble up as null for the UI
            return null;
        }
    }
}
