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
        private int _blogId;
        private string _connectionString;

        public BlogDetailMenu(IUserInterfaceManager parentUI, string connectionString, int blogId)
        {
            _parentUI = parentUI;
            _blogRepository = new BlogRepository(connectionString);
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
                    return null;

                case "3":
                    return null;
                case "4":
                    return null;
                case "5":
                    return null;
                case "0":
                    Console.WriteLine("Good bye");
                    return null;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;

            }
        }

    


    }
}
