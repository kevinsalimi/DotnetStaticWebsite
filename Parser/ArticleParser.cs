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
            var splitedLine = line.Split(':');
            switch (splitedLine[0])
            {
                case nameof(PostModel.Title):
                    article.Title = splitedLine[1];
                    break;
                case nameof(PostModel.Summary):
                    article.Summary = splitedLine[1];
                    break;
                case nameof(PostModel.Keywords):
                    article.Keywords = splitedLine[1];
                    break;
                case nameof(PostModel.Author):
                    article.Author = splitedLine[1];
                    break;
                case nameof(PostModel.ArticleId):
                    article.ArticleId = splitedLine[1];
                    break;
                default:
                    break;
            }
        }

        public static ArticleModel GetPostDetaile()
        {
            return article;
        }


    }

}
