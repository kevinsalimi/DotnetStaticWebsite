
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K1_Static_Website.Models;
using Markdig;

namespace K1_Static_Website.Services
{
    public class ContentInitializer : IContentInitializer
    {
        private const string aboutBlogPath = "wwwroot/data/about/about.md";
        private const string folderPath = "wwwroot/data/posts";
        private const string postExtension = "*.md";
        private string aboutPageContent;
        private List<PostModel> blogPosts;
        public ContentInitializer()
        {
            BuildAllBlocks();
            BuildTheAboutPage();
        }

        public List<PostModel> GetLandingPageContent()
        {
            return blogPosts.OrderByDescending(x => x.DisplayPriority).ToList();
        }

        public string GetAboutPageContent()
        {
            return aboutPageContent;
        }

        public PostModel FindBlogHeaderInfo(string filePath)
        {
            return blogPosts.FirstOrDefault(x => x.Address.Contains(filePath));
        }

        private void BuildTheAboutPage()
        {
            using var fileStream = new FileStream(aboutBlogPath, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new StreamReader(fileStream);
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var markdown = reader.ReadToEnd();
            aboutPageContent = Markdown.ToHtml(markdown, pipeline);
        }

        private void BuildAllBlocks()
        {
            var blogPaths = Directory.GetFiles(folderPath, postExtension, SearchOption.TopDirectoryOnly).ToList();
            blogPosts = new List<PostModel>(blogPaths.Count);

            foreach (var blogPath in blogPaths)
            {
                using var fileStream = new FileStream(blogPath, FileMode.Open, FileAccess.Read);
                using StreamReader reader = new StreamReader(fileStream);

                var blogPost = new PostModel { Address = blogPath.Trim().Replace(' ', '-') };

                for (int i = 0; i < 9; i++)
                {
                    ParsLine(blogPost, reader.ReadLine());
                }

                blogPosts.Add(blogPost);
            }
        }
        private void ParsLine(PostModel blogPost, string LineContent)
        {
            var splittedLine = LineContent.Split(':');
            switch (splittedLine[0])
            {
                case nameof(PostModel.Title):
                    blogPost.Title = splittedLine[1];
                    break;
                case nameof(PostModel.Link):
                    blogPost.Link = splittedLine[1].Trim().Replace(' ', '-');
                    break;
                case nameof(PostModel.Summary):
                    blogPost.Summary = splittedLine[1];
                    break;
                case nameof(PostModel.Keywords):
                    blogPost.Keywords = splittedLine[1];
                    break;
                case nameof(PostModel.CreationDate):
                    blogPost.CreationDate = splittedLine[1];
                    break;
                case nameof(PostModel.Author):
                    blogPost.Author = splittedLine[1];
                    break;
                case nameof(PostModel.ArticleId):
                    blogPost.ArticleId = splittedLine[1];
                    break;
                case nameof(PostModel.DisplayPriority):
                    blogPost.DisplayPriority = int.Parse(splittedLine[1]);
                    break;
                default:
                    break;
            }
        }

    }
}