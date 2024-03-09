using K1_Static_Website.Models;

namespace K1_Static_Website.Parser
{
    public static class ArticleParser
    {
        private static ArticleModel article;

        public static void CreateNew()
        {
            article = new ArticleModel() { };
        }

        public static void ParsLine(string line)
        {
            var splittedLine = line.Split(':');
            switch (splittedLine[0])
            {
                case nameof(PostModel.Title):
                    article.Title = splittedLine[1];
                    break;
                case nameof(PostModel.Summary):
                    article.Summary = splittedLine[1];
                    break;
                case nameof(PostModel.Keywords):
                    article.Keywords = splittedLine[1];
                    break;
                case nameof(PostModel.Author):
                    article.Author = splittedLine[1];
                    break;
                case nameof(PostModel.ArticleId):
                    article.ArticleId = splittedLine[1];
                    break;
                default:
                    break;
            }
        }

        public static ArticleModel GetPostDetails()
        {
            return article;
        }


    }

}
