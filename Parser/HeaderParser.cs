using K1_Static_Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace K1_Static_Website.Parser
{
    public static class HeaderParser
    {
        private static List<PostModel> postModels;
        private static PostModel post;
        private static Dictionary<string, string> postAddress;

        public static void CreateNewModel(int length)
        {
            postModels = new List<PostModel>(length);
            postAddress = new Dictionary<string, string>(length);
        }

        public static void CreateNewPost(string address)
        {
            post = new PostModel() { Address = address };
        }

        public static void ParsLine(string line)
        {
            var splittedLine = line.Split(':');
            switch (splittedLine[0])
            {
                case nameof(PostModel.Title):
                    post.Title = splittedLine[1];
                    break;
                case nameof(PostModel.Link):
                    post.Link = splittedLine[1].Trim().Replace(' ', '-');
                    postAddress.Add(post.Link, post.Address);
                    break;
                case nameof(PostModel.Summary):
                    post.Summary = splittedLine[1];
                    break;
                case nameof(PostModel.Keywords):
                    post.Keywords = splittedLine[1];
                    break;
                case nameof(PostModel.CreationDate):
                    post.CreationDate = splittedLine[1];
                    break;
                case nameof(PostModel.Author):
                    post.Author = splittedLine[1];
                    break;
                case nameof(PostModel.ArticleId):
                    post.ArticleId = splittedLine[1];
                    break;
                case nameof(PostModel.DisplayPriority):
                    post.DisplayPriority = int.Parse(splittedLine[1]);
                    break;
                default:
                    break;
            }
        }

        public static void MakeHeader()
        {
            postModels.Add(post);
        }

        public static List<PostModel> GetPostList()
        {
            return postModels.OrderByDescending(x=>x.DisplayPriority).ToList();
        }

        public static string FindAddress(string link)
        {
            return postAddress[link];
        }

        public static int GetPostListLength()
        {
            return postModels != null ? postModels.Count : 0;
        }

    }

}
