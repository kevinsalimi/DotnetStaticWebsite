using K1_Static_Website.Models;
using K1_Static_Website.Parser;
using K1_Static_Website.Services;
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
        private readonly IContentInitializer _contentInitializer;

        public HomeController(IContentInitializer landingPageCreator)
        {
            _contentInitializer = landingPageCreator;
        }
        public IActionResult Index()
        {
            return View(model: _contentInitializer.GetLandingPageContent());
        }

        [HttpGet("article/{*fileName}")]
        public async Task<IActionResult> Article(string fileName)
        {
            string filePath = fileName.Replace("%2F", "/");
            var blogHeaderInfo = _contentInitializer.FindBlogHeaderInfo(filePath);

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new StreamReader(fileStream);

            for (var i = 0; i < 9; i++)
            { reader.ReadLine(); }

            var markdown = await reader.ReadToEndAsync();
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            ViewBag.Summary = blogHeaderInfo.Summary;
            ViewBag.Keywords = blogHeaderInfo.Keywords;
            ViewBag.Author = blogHeaderInfo.Author;

            return View(model:
                        new ArticleModel
                        {
                            Html = Markdown.ToHtml(markdown, pipeline),
                            ArticleId = blogHeaderInfo.ArticleId
                        });
        }

        public async Task<IActionResult> About()
        {
            return View(model: _contentInitializer.GetAboutPageContent());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult HealthCheck()
        {
            return Ok();
        }
    }
}
