using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;

/// <summary>
/// This scraper USES Selenium.
/// In principle, you can use requests, urllib, etc.
/// Some OCW pages use cookies and js to identify users and update pages.
/// So you have to get the page dynamically. ""Selenium"".
/// Selenium requires a browser driver.
/// Specifically https://www.selenium.dev/selenium/docs/api/py/api.html referring to.
/// </summary>
/// 

namespace Oocw.Crawler.Core;

public class DriverWrapper
{

    private IWebDriver? _driver;


    public virtual bool IsInitialized => _driver != null;

    public string DriverPath { get; }
    public int HtmlTimeout { get; }
    public int MemoryResetLimit { get; }

    private int _memoryResetCount;


    public DriverWrapper(string driverPath = "chromedriver.exe", int memoryResetLimit = 300, int htmlTimeout = 60)
    {
        MemoryResetLimit = memoryResetLimit;
        DriverPath = driverPath;
        HtmlTimeout = htmlTimeout;

        _memoryResetCount = 0;
        _driver = null;
    }


    public bool TryCloseDriver()
    {
        if (_driver != null)
        {
            _driver.Quit();
            _driver.Dispose();
            _driver = null;
            return true;
        }
        return false;
    }

    public virtual void Initialize()
    {
        TryCloseDriver();
        var options = new ChromeOptions();
        //options.add_argument('user-agent="Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19"')
        options.AddArgument("user_agent=\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36\"");
        options.AddArgument("-Referer=\"http://www.ocw.titech.ac.jp/\"");
        _driver = new ChromeDriver(DriverPath, options: options);
        _memoryResetCount = 0;
    }

    private Exception DriverNotInitialized()
    {
        return new Exception("Driver not initialized!");
    }

    public virtual string GetHtml (string url)
    {
        if (_memoryResetCount >= MemoryResetLimit)
            Initialize();
        if (_driver == null)
            throw DriverNotInitialized();

        _driver.Navigate().GoToUrl(url);
        var html = _driver.PageSource;
        _memoryResetCount += 1;
        return html;
    }

    public virtual string GetHtmlAfterLoaded(string url)
    {
        var timeout = HtmlTimeout;
        var html = this.GetHtml(url);
        var fullLoaded = -1;
        var time_start = DateTime.Now;
        var dTime = TimeSpan.Zero;
        while (fullLoaded < 0)
        {
            html = _driver!.PageSource;
            fullLoaded = Math.Min(html.IndexOf("left-menu"), html.IndexOf("right-contents"));
            dTime = DateTime.Now - time_start;
            if (timeout < dTime.Seconds)
            {
                return "timeout";
            }
            if (html.IndexOf("HTTP 404") >= 0 ||
                html.IndexOf("404 NOT FOUND") >= 0 ||
                html.IndexOf("お探しのコンテンツが見つかりませんでした") >= 0)
            {
                return "404";
            }
            else if (html.IndexOf("top-mein-navi") >= 0)
            {
                return "toppage";
            }
        }
        return html;
    }
}