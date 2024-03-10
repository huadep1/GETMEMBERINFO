using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GETMEMBERINFO
{
    internal class Program
    {
        static HttpClientHandler handler = new HttpClientHandler();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            handler.AutomaticDecompression = DecompressionMethods.All;
            Console.WriteLine("Please enter your cookies");
            string cookies = Console.ReadLine();
            if (cookies.Contains("c_user"))
            {
                string userid = cookies.Split("c_user=")[1].Split(";")[0];
                Console.WriteLine("Facebook Account ID: " + userid);
            }
            else
            {
                Console.WriteLine("No Cookies");
                return;
            }
            foreach (string item in cookies.Split(';'))
            {
                var temp1 = item.Trim();
                var temp2 = temp1.Split('=');
                if (temp2.Length > 1)
                {
                    handler.CookieContainer.Add(
                        new Uri("https://www.facebook.com"),
                        new Cookie(temp2[0], temp2[1])
                    );
                }
            }
            string dtsg = await GET_DTSG_Async();
            if (dtsg == null)
            {
                Console.WriteLine("Error DTSG");
                return;
            }
            Console.WriteLine("Please enter your group id");
            string groupid = Console.ReadLine();
            await GET_MEMBER_INFO_Async(dtsg, groupid);
        }

        static HttpClient client => new HttpClient(handler);

        static async Task<string> GET_DTSG_Async()
        {
            try
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    "https://www.facebook.com/help/contact/571927962827151/"
                );

                request.Headers.Add("authority", "www.facebook.com");
                request.Headers.Add(
                    "accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
                );
                request.Headers.Add(
                    "accept-language",
                    "en-GB,en;q=0.9,vi-VN;q=0.8,vi;q=0.7,en-US;q=0.6"
                );
                request.Headers.Add("cache-control", "max-age=0");
                request.Headers.Add("dnt", "1");
                request.Headers.Add("dpr", "1");
                request.Headers.Add("sec-ch-prefers-color-scheme", "dark");
                request.Headers.Add(
                    "sec-ch-ua",
                    "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Google Chrome\";v=\"122\""
                );
                request.Headers.Add(
                    "sec-ch-ua-full-version-list",
                    "\"Chromium\";v=\"122.0.6261.112\", \"Not(A:Brand\";v=\"24.0.0.0\", \"Google Chrome\";v=\"122.0.6261.112\""
                );
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-model", "\"\"");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                request.Headers.Add("sec-fetch-dest", "document");
                request.Headers.Add("sec-fetch-mode", "navigate");
                request.Headers.Add("sec-fetch-site", "same-origin");
                request.Headers.Add("sec-fetch-user", "?1");
                request.Headers.Add("upgrade-insecure-requests", "1");
                request.Headers.Add(
                    "user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36"
                );
                request.Headers.Add("viewport-width", "1069");
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var dtsg = new Regex(
                    "<input type=\"hidden\" name=\"fb_dtsg\" value=\"([^\\\"]+)\""
                ).Match(responseBody);
                if (!dtsg.Success)
                {
                    return "";
                }
                return dtsg.Groups[1].Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }

        static async Task GET_MEMBER_INFO_Async(string dtsg, string groupid)
        {
            string cursor = "";
            while (true)
            {
                try
                {
                    using var request = new HttpRequestMessage(
                        HttpMethod.Post,
                        "https://www.facebook.com/api/graphql/"
                    );
                    request.Headers.Add("authority", "www.facebook.com");
                    request.Headers.Add("accept", "*/*");
                    request.Headers.Add("accept-language", "en-US,en;q=0.9,vi;q=0.8");
                    request.Headers.Add("dpr", "1");
                    request.Headers.Add("origin", "https://www.facebook.com");
                    request.Headers.Add("sec-ch-prefers-color-scheme", "dark");
                    request.Headers.Add(
                        "sec-ch-ua",
                        "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Google Chrome\";v=\"122\""
                    );
                    request.Headers.Add(
                        "sec-ch-ua-full-version-list",
                        "\"Chromium\";v=\"122.0.6261.112\", \"Not(A:Brand\";v=\"24.0.0.0\", \"Google Chrome\";v=\"122.0.6261.112\""
                    );
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("sec-ch-ua-model", "\"\"");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("sec-ch-ua-platform-version", "\"10.0.0\"");
                    request.Headers.Add("sec-fetch-dest", "empty");
                    request.Headers.Add("sec-fetch-mode", "cors");
                    request.Headers.Add("sec-fetch-site", "same-origin");
                    request.Headers.Add(
                        "user-agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36"
                    );
                    request.Headers.Add("viewport-width", "1537");
                    request.Headers.Add("x-asbd-id", "129477");
                    request.Headers.Add(
                        "x-fb-friendly-name",
                        "GroupsCometPeopleProfilesPaginatedListPaginationQuery"
                    );
                    request.Headers.Add("x-fb-lsd", "NJ-fSktSea_7hCham0PKhi");
                    Dictionary<string, string> formData = new Dictionary<string, string>
                    {
                        { "__a", "1" },
                        { "fb_dtsg", dtsg },
                        {
                            "variables",
                            "{\"count\":10,\"cursor\":\""
                                + cursor
                                + "\",\"groupID\":\""
                                + groupid
                                + "\",\"membershipType\":\"MEMBER\",\"scale\":1,\"search\":null,\"statusStaticFilter\":null,\"id\":\""
                                + groupid
                                + "\"}"
                        },
                        { "doc_id", "24770628242581257" }
                    };
                    request.Content = new FormUrlEncodedContent(formData);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(
                        "application/x-www-form-urlencoded"
                    );
                    HttpResponseMessage response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonDocument.Parse(responseBody).RootElement;
                    bool flag = responseJson
                        .GetProperty("data")
                        .GetProperty("node")
                        .GetProperty("people_profiles")
                        .GetProperty("page_info")
                        .GetProperty("has_next_page")
                        .GetBoolean();
                    foreach (
                        var info in responseJson
                            .GetProperty("data")
                            .GetProperty("node")
                            .GetProperty("people_profiles")
                            .GetProperty("edges")
                            .EnumerateArray()
                    )
                    {
                        Console.WriteLine(
                            string.Format(
                                "{0} | {1} | {2}",
                                info.GetProperty("node").GetProperty("name").GetString(),
                                info.GetProperty("node").GetProperty("id").GetString(),
                                info.GetProperty("node").GetProperty("profile_url").GetString()
                            )
                        );
                    }
                    if (flag)
                    {
                        cursor = responseJson
                            .GetProperty("data")
                            .GetProperty("node")
                            .GetProperty("people_profiles")
                            .GetProperty("page_info")
                            .GetProperty("end_cursor")
                            .GetString();
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
            }
        }
    }
}
