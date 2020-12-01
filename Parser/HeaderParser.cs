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

        public static void CreateNewModel()
        {
            postModels = new List<PostModel>();
            postAddress = new Dictionary<string, string>();
        }

        public static void CreateNewPost(string address)
        {
            post = new PostModel() { Address = address };
        }

        public static void ParsLine(string line)
        {
            var splitedLine = line.Split(':');
            switch (splitedLine[0])
            {
                case nameof(PostModel.Title):
                    post.Title = splitedLine[1];
                    post.Link = splitedLine[1].Trim().Replace(' ', '-');
                    postAddress.Add(post.Link, post.Address);
                    break;
                case nameof(PostModel.Summary):
                    post.Summary = splitedLine[1];
                    break;
                case nameof(PostModel.Keywords):
                    post.Keywords.AddRange(splitedLine[1].Split(','));
                    break;
                case nameof(PostModel.CreationDate):
                    post.CreationDate = splitedLine[1];
                    break;
                case nameof(PostModel.Author):
                    post.Author = splitedLine[1];
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
            return postModels;
        }

        public static string FindAddress(string link)
        {
            return postAddress[link];
        }
    }

}
