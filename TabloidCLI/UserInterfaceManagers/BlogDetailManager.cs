using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    public class BlogDetailManager : IUserInterfaceManager
    {
        private IUserInterfaceManager _parentUI;
        private BlogRepository _blogRepository;
        private int _blogId;

        public BlogDetailManager(IUserInterfaceManager parentUI, string connectionString, int blogId)
        {
            _parentUI = parentUI;
            _blogRepository = new BlogRepository(connectionString);
            _blogId = blogId;
        }

        public IUserInterfaceManager Execute()
        {
            Blog blog = _blogRepository.Get(_blogId);
            Console.WriteLine("Blog Details");
            Console.WriteLine($"Title: {blog.Title}");
            Console.WriteLine($"Url: {blog.Url}");
            Console.WriteLine();

            Console.WriteLine("1) Edit");
            Console.WriteLine("2) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Edit();
                    return this;
                case "2":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void Edit()
        {
            Blog blog = _blogRepository.Get(_blogId);
            Console.WriteLine("Editing Blog");
            Console.Write("New Title (blank to leave unchanged): ");
            string title = Console.ReadLine();
            Console.Write("New Content (blank to leave unchanged): ");
            string url = Console.ReadLine();

            // Update the journal only if the user entered a non-empty value
            if (!string.IsNullOrWhiteSpace(title))
            {
                blog.Title = title;
            }

            if (!string.IsNullOrWhiteSpace(url)) 
            {
                blog.Url = url;
            }

            _blogRepository.Update(blog);
        }
    }
}
