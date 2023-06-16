using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    internal class JournalDetailManager : IUserInterfaceManager
    {
        private IUserInterfaceManager _parentUI;
        private JournalRepository _journalRepository;
        private int _journalId;

        public JournalDetailManager(IUserInterfaceManager parentUI, string connectionString, int journalId)
        {
            _parentUI = parentUI;
            _journalRepository = new JournalRepository(connectionString);
            _journalId = journalId;
        }

        public IUserInterfaceManager Execute()
        {
            Journal journal = _journalRepository.Get(_journalId);
            Console.WriteLine("Journal Details");
            Console.WriteLine($"Title: {journal.Title}");
            Console.WriteLine($"Content: {journal.Content}");
            Console.WriteLine($"Created At: {journal.CreateDateTime}");
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
            Journal journal = _journalRepository.Get(_journalId);
            Console.WriteLine("Editing Journal");
            Console.Write("New Title (blank to leave unchanged): ");
            string title = Console.ReadLine();
            Console.Write("New Content (blank to leave unchanged): ");
            string content = Console.ReadLine();

            // Update the journal only if the user entered a non-empty value
            if (!string.IsNullOrWhiteSpace(title))
            {
                journal.Title = title;
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                journal.Content = content;
            }

            _journalRepository.Update(journal);
        }
    }
}
