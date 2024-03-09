using K1_Static_Website.Models;
using K1_Static_Website.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
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

        [HttpGet("article/{*link}")]
        public async Task<IActionResult> Article(string link)
        {
            var blogHeaderInfo = _contentInitializer.FindBlogHeaderInfo(link);

            using var fileStream = new FileStream(blogHeaderInfo.Address, FileMode.Open, FileAccess.Read);
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
