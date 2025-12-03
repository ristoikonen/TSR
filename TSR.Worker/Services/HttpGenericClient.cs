using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


namespace TSR.Worker.Services
{
    public class HttpGenericClient<T>
    {
        public async Task<JsonDocument?> GetAsync(string requestUri, string HostEndpoint = "", Dictionary<string, string>? contentsParams = null, string token = "")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    client.DefaultRequestHeaders.Add("Host", HostEndpoint);
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    if (token.Length > 0)
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    if (contentsParams is not null)
                    {
                        var content = new FormUrlEncodedContent(contentsParams);
                        string paramAsJSON = System.Text.Json.JsonSerializer.Serialize(content);
                        var contents = new StringContent(paramAsJSON, Encoding.UTF8, "application/json");
                    }

                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode && response.Content is object)
                    {
                        try
                        {
                            return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                            //var contentStream = await response.Content.ReadAsStreamAsync();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine(ex2.Message);
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<T?> PostAsync(string uri, Dictionary<string, string> contentParams, string HostEndpoint = "")
        {
            try
            {
                var content = new FormUrlEncodedContent(contentParams);

                string paramAsJSON = JsonSerializer.Serialize(content); //, new JsonSerializerOptions { WriteIndented = true });
                //string paramAsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(content);

                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Host", HostEndpoint);
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");

                    var contents = new StringContent(paramAsJSON, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(uri, contents);
                    if (response.IsSuccessStatusCode && response.Content is object)
                    {
                        var accountinfo = response.Content.ReadFromJsonAsync<T?>();
                        return accountinfo.Result;
                    }
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
        }


        public async Task<T?> PostAsync(string uri, Dictionary<string, string> contentsParams, string token = "", string HostEndpoint = "")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    client.DefaultRequestHeaders.Add("Host", HostEndpoint);
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    if (token.Length > 0)
                        client.DefaultRequestHeaders.Add("Authorization", $"token {token}");

                    if (contentsParams is not null)
                    {
                        var content = new FormUrlEncodedContent(contentsParams);
                        string paramAsJSON = System.Text.Json.JsonSerializer.Serialize(content);

                        var contents = new StringContent(paramAsJSON, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(uri, contents);

                        if (response.IsSuccessStatusCode && response.Content is object)
                        {
                            var tinfo = response.Content.ReadFromJsonAsync<T?>();
                            return tinfo.Result;
                        }
                    }
                    //TODO: else is error

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
            return default(T);
        }


        //try
        //{
        //    var content = new FormUrlEncodedContent(paramdict);
        //    string paramAsJSON = JsonSerializer.Serialize(paramdict);

        //    var client = new HttpClient();

        //    client.DefaultRequestHeaders.Add("Accept", "*/*");
        //    client.DefaultRequestHeaders.Add("Host", Endpoints.Host);
        //    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        //    if (token.Length > 0)
        //        client.DefaultRequestHeaders.Add("Authorization", $"token {token}");
        //    var contents = new StringContent(paramAsJSON, Encoding.UTF8, "application/json");

        //    HttpResponseMessage response = await client.PostAsync("https://api.mail.tm/token", contents);

        //    if (response.IsSuccessStatusCode && response.Content is object)
        //    {
        //        var jd = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        //        var jd2 = await response.Content.ReadFromJsonAsync<T?>();
        //    }

        //Root data = JsonSerializer.Deserialize<Root>(response.Conten);
        //var res= System.Text.Json.JsonSerializer.Deserialize<CoinData[]>(response.Content.ReadAsStream());
        //if(contentStream is not null)
        //    return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(contentStream);
        //else
        //    return default(T);

        //    if (response.Content is object)
        //    {
        //        var accountinfo = response.Content.ReadFromJsonAsync<T?>();
        //        return accountinfo.Result;
        //    }
        //    return default(T);
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.Message);
        //    return default(T);
        //}

    }
}
