
using System.Collections.Generic;
using K1_Static_Website.Models;

namespace K1_Static_Website.Services
{
    public interface IContentInitializer
    {
        List<PostModel> GetLandingPageContent();
        string GetAboutPageContent();
        PostModel FindBlogHeaderInfo(string filePath);

    }
}