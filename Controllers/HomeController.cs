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
        private const string postEnvironment = "wwwroot/posts";
        private const string aboutmd = "wwwroot/about/about.md";
        private const string postExtention = "*.md";

        public async Task<IActionResult> Index()
        {
            var posts = Directory.GetFiles(postEnvironment, postExtention, SearchOption.TopDirectoryOnly);
            HeaderParser.CreateNewModel();

            foreach (var post in posts)
            {
                using (var fileStream = new FileStream(post, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        HeaderParser.CreateNewPost(post);
                        for (int i = 0; i < 6; i++)
                        {
                            HeaderParser.ParsLine(await reader.ReadLineAsync());
                        }
                        HeaderParser.MakeHeader();
                    }
                }


            }
            return View(model: HeaderParser.GetPostList());
        }

        [HttpGet("article/{*fileName}")]
        public async Task<IActionResult> Article(string fileName)
        {
            using (var fileStream = new FileStream(HeaderParser.FindAddress(fileName),
                FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    for (var i = 0; i < 7; i++)
                    {
                        await reader.ReadLineAsync();
                    }

                    var markdown = await reader.ReadToEndAsync();
                    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                    return View(model: new ArticleModel
                    { Html = Markdown.ToHtml(markdown, pipeline), Title = string.Concat("/", fileName) });
                }
            }
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
