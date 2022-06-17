using K1_Static_Website.Helper;
using K1_Static_Website.Models;
using K1_Static_Website.Parser;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace K1_Static_Website.Controllers
{
    public class HomeController : Controller
    {
        private const string postEnvironment = "wwwroot/data/posts";
        private const string aboutmd = "wwwroot/data/about/about.md";
        private const string postExtention = "*.md";

        public async Task<IActionResult> Index()
        {
            var result= Redirect("https://silentexception.com");
            result.Permanent = true;
            return result;
        }

        [HttpGet("article/{*fileName}")]
        public async Task<IActionResult> Article(string fileName)
        {
            var result = Redirect($"https://silentexception.com/article/{fileName}");
            result.Permanent = true;
            return result;
        }

        public async Task<IActionResult> About()
        {
            string html = string.Empty;
            using (var fileStream = new FileStream(aboutmd, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                    var markdown = await reader.ReadToEndAsync();
                    html = Markdown.ToHtml(markdown, pipeline);
                }
            }

            return View(model: html);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
