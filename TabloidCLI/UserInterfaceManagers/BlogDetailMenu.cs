using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    public class BlogDetailMenu : IUserInterfaceManager
    {
        private IUserInterfaceManager _parentUI;
        private BlogRepository _blogRepository;
        private TagRepository _tagRepository;
        private int _blogId;
        private string _connectionString;

        public BlogDetailMenu(IUserInterfaceManager parentUI, string connectionString, int blogId)
        {
            _parentUI = parentUI;
            _blogRepository = new BlogRepository(connectionString);
            _tagRepository = new TagRepository(connectionString);
            _blogId = blogId;
            _connectionString = connectionString;
        }

        public IUserInterfaceManager Execute()
        {
            Blog blog = _blogRepository.Get(_blogId);
            Console.WriteLine("1) View");
            Console.WriteLine($"2) Add Tag");
            Console.WriteLine($"3) Remove Tag");
            Console.WriteLine($"4) View Posts");
            Console.WriteLine("5) Return");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":

                    return new BlogDetailManager(_parentUI, _connectionString, _blogId);
                    
                case "2":
                    AddTag();
                    return this;

                case "3":
                    return null;
                case "4":
                    ViewBlogPosts2(_blogRepository);
                    return this;
                case "5":
                    return _parentUI;
                case "0":
                    Console.WriteLine("Good bye");
                    return null;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;

            }
        }
        // Gives the user the ability to add a Tag to a Blog
        private void AddTag()
        {
            Blog blog = _blogRepository.Get(_blogId);

            Console.WriteLine($"Which tag would you like to add to {blog.Title}?");

            List<Tag> tags = _tagRepository.GetAll();

            for (int i = 0; i < tags.Count; i++)
            {
                Tag tag = tags[i];
                Console.WriteLine($"{i + 1}) {tag.Name}");
            }
            Console.Write("> ");
            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                Tag tag = tags[choice - 1];
                _blogRepository.InsertTag(blog, tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection. Won't add tag.");
            }
        }

        private BlogRepository Get_blogRepository()
        {
            return _blogRepository;
        }

        private void ViewBlogPosts2(BlogRepository _blogRepository)
        {
            List<Post> posts = _blogRepository.GetPostByBlog(_blogId);
            foreach (Post post in posts)
            {
                Console.WriteLine(post);
            }
            Console.WriteLine();
        }


    }
}
