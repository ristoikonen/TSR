namespace TSR.Web.ApiClients;

public class PostcodeApiClient(HttpClient httpClient)
{
    public async Task<Models.AustralianPostcode?> GetPostcodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;

        try
        {
            return await httpClient.GetFromJsonAsync<Models.AustralianPostcode>($"https://localhost:7452/postcode/{Uri.EscapeDataString(code)}", cancellationToken);
        }
        catch (HttpRequestException)
        {
            // bubble up as null for the UI
            return null;
        }
    }
}
