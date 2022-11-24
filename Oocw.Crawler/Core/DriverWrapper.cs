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

    private IWebDriver? driver;

    public readonly string driver_path;
    public readonly int html_timeout;
    public readonly int mem_reset_limit;

    private int mem_reset_count;


    public DriverWrapper(string driver_path = "chromedriver.exe", int mem_reset_limit = 300, int html_timeout = 60)
    {
        this.mem_reset_limit = mem_reset_limit;
        this.driver_path = driver_path;
        this.html_timeout = html_timeout;

        mem_reset_count = 0;
        driver = null;
    }

    public virtual bool is_initialized()
    {
        return driver != null;
    }

    public bool TryCloseDriver()
    {
        if (driver != null)
        {
            driver.Quit();
            driver.Dispose();
            driver = null;
            return true;
        }
        return false;
    }

    public virtual void init_driver()
    {
        TryCloseDriver();
        var options = new ChromeOptions();
        //options.add_argument('user-agent="Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19"')
        options.AddArgument("user_agent=\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36\"");
        options.AddArgument("-Referer=\"http://www.ocw.titech.ac.jp/\"");
        driver = new ChromeDriver(driver_path, options: options);
        mem_reset_count = 0;
    }

    private Exception DriverNotInitialized()
    {
        return new Exception("Driver not initialized!");
    }

    public virtual string get_html(string url)
    {
        if (mem_reset_count >= mem_reset_limit)
            init_driver();
        if (driver == null)
            throw DriverNotInitialized();

        driver.Navigate().GoToUrl(url);
        var html = driver.PageSource;
        mem_reset_count += 1;
        return html;
    }

    public virtual string get_html_after_loaded(string url)
    {
        var timeout = html_timeout;
        var html = this.get_html(url);
        var full_loaded = -1;
        var time_start = DateTime.Now;
        var dtime = TimeSpan.Zero;
        while (full_loaded < 0)
        {
            html = driver!.PageSource;
            full_loaded = Math.Min(html.IndexOf("left-menu"), html.IndexOf("right-contents"));
            dtime = DateTime.Now - time_start;
            if (timeout < dtime.Seconds)
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