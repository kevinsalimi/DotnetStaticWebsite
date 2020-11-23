using K1_Static_Website.Helper;
using K1_Static_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Westwind.AspNetCore.Markdown;

namespace K1_Static_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var files = Directory.GetFiles("wwwroot/Markdowns", "*.md", SearchOption.TopDirectoryOnly);
            List<PostModel> postListSummary = new List<PostModel>(10);
            foreach (var item in files)
            {
                var fileName = item.Substring(item.IndexOf("\\") + 1);
                var html = await Markdown.ParseFromFileAsync($"~/Markdowns/{fileName}");
                var postTitle = FileDigger.GetPostTitle(fileName);
                postListSummary.Add(
                    new PostModel
                    {
                        Title = postTitle,
                        Link = postTitle.Replace(' ', '-'),
                        Summary = FileDigger.GetSummary(html),
                        CreationDate = System.IO.File.GetCreationTime(item).ToLongDateString()
                    }); 
            }
            return View(model: postListSummary);
        }

        [HttpGet("Article/{*fileName}")]
        public async Task<IActionResult> Article(string fileName)
        {
            fileName = fileName.Replace('-',' ');
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Wrong parameter.");
            }

            try
            {
                var htmlData = await Markdown.ParseFromFileAsync($"~/Markdowns/{fileName}.md");
                return View(model: htmlData);
            }
            catch (Exception)
            {
                return View(model: "Some bad things happend.");
            }
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
