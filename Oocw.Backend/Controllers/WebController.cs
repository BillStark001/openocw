﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Oocw.Backend.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebController : ControllerBase
    {
        private static Dictionary<string, string> _pageCache;

        static WebController()
        {
            _pageCache = new Dictionary<string, string>();
        }

        private string _indexPath;

        public WebController(
            IWebHostEnvironment env
            )
        {
            _indexPath = Path.Combine(env.WebRootPath, "index.html");
        }

        [HttpGet("/")]
        public ActionResult<string> GetIndex()
        {
            try
            {
                var succ = _pageCache.TryGetValue(_indexPath, out var txt);
                if (!succ)
                {
                    txt = System.IO.File.ReadAllText(_indexPath, Encoding.UTF8);
                    _pageCache[_indexPath] = txt;
                }
                if (txt == null)
                    throw new NotImplementedException();
                Response.ContentType = "text/html; charset=utf-8";
                Response.WriteAsync(txt).Wait();
                return new EmptyResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Redirect("/index.html");
            }

        }
    }
}
