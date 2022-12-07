using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Web;

using AngleSharp.Dom;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Collections.Specialized;

namespace Oocw.Crawler.Utils;


using IDSS = IDictionary<string, string>;

public static class HtmlUtils
{

    // url

    // naive http request

    public delegate void ClientModifier(HttpClient client);

    public static void SetDefaultAgent(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Accept-Language", "ja-JP,ja;q=0.9,en;q=0.8");
    }

    public static List<(string, string)> GetCookies(this HttpResponseMessage message)
    {
        message.Headers.TryGetValues("Set-Cookie", out var setCookie);
        var setCookieString = setCookie?.Single() ?? "";
        var cookieTokens = setCookieString.Split(';');

        if (cookieTokens.Length == 0 || (
            cookieTokens.Length == 1 && string.IsNullOrWhiteSpace(cookieTokens[0])
            ))
            return new();

        var ret = new List<(string, string)>();
        foreach (var cookieToken in cookieTokens)
        {
            var kvp = cookieToken.Split('=', 2);
            var k = kvp[0];
            var v = kvp.Length > 1 ? kvp[1] : "";
            ret.Add((k, HttpUtility.UrlDecode(v)));
        }
        return ret;
    }

    public static async Task<HttpResponseMessage> GetResponseAsync(string target, ClientModifier? modif = null, bool autoRedirect = true)
    {
        var handler = new HttpClientHandler()
        {
            AllowAutoRedirect = autoRedirect
        };
        var client = new HttpClient(handler);
        SetDefaultAgent(client);
        if (modif != null)
            modif(client);
        return await client.GetAsync(target);
    }

    public static async Task<HttpResponseMessage> GetResponseAsync(string target, HttpContent postBody, ClientModifier? modif = null, bool autoRedirect = true)
    {
        var handler = new HttpClientHandler()
        {
            AllowAutoRedirect = autoRedirect
        };
        var client = new HttpClient(handler);
        SetDefaultAgent(client);
        if (modif != null)
            modif(client);
        return await client.PostAsync(target, postBody);
    }

    public static async Task<string> ParseHtmlAsync(this HttpResponseMessage result, Encoding? encoding = null)
    {
        string resStr;
        if (encoding != null)
        {
            var bytes = await result.Content.ReadAsByteArrayAsync();
            resStr = encoding.GetString(bytes);
        }
        else
        {
            // var result = await client.GetAsync(target);
            resStr = await result.Content.ReadAsStringAsync();
        }
        var htmlStart = resStr.IndexOf("<!DOCTYPE html");
        if (htmlStart > 0)
            resStr = resStr.Substring(htmlStart, resStr.Length - htmlStart);
        return resStr;
    }

    public static IDocument Dom(this string html) => new HtmlParser().ParseDocument(html);
    public static async Task<IDocument> DomAsync(this string html) => await new HtmlParser().ParseDocumentAsync(html);

}
