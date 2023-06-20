using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using TabloidCLI.Models;
using TabloidCLI.Repositories;
using TabloidCLI.UserInterfaceManagers;

namespace TabloidCLI
{
    public class JournalRepository : DatabaseConnector, IRepository<Journal>
    {
        public JournalRepository(string connectionString) : base(connectionString) { }

        public List<Journal> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // SQL query to select all columns from the Journal table
                    cmd.CommandText = @"SELECT id,
                                               Title,
                                               Content,
                                               CreateDateTime
                                          FROM Journal";

                    List<Journal> journals = new List<Journal>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Create a new Journal object and populate its properties from the query results
                        Journal journal = new Journal()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Content = reader.GetString(reader.GetOrdinal("Content")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"))
                        };

                        journals.Add(journal); // Add the journal to the list
                    }

                    reader.Close();

                    return journals; // Return the list of journals
                }
            }
        }

        public Journal Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // SQL query to select a single journal by its id
                    cmd.CommandText = @"SELECT Journal.Id, 
                                            Journal.Title,
                                            Journal.Content,
                                            Journal.CreateDateTime
                                       FROM Journal
                                       WHERE Journal.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Journal journal = null;
                    if (reader.Read())
                    {
                        // Create a new Journal object and populate its properties from the query results
                        journal = new Journal()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Content = reader.GetString(reader.GetOrdinal("Content")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"))
                        };
                    }

                    reader.Close();

                    return journal; // Return the journal
                }
            }
        }

        public void Insert(Journal journal)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // SQL query to insert a new journal into the database
                    cmd.CommandText = @"INSERT INTO Journal (Title, Content, CreateDateTime)
                                         VALUES (@title, @content, @createDateTime)";
                    cmd.Parameters.AddWithValue("@title", journal.Title);
                    cmd.Parameters.AddWithValue("@content", journal.Content);
                    cmd.Parameters.AddWithValue("@createDateTime", journal.CreateDateTime);

                    cmd.ExecuteNonQuery(); // Execute the insert query
                }
            }
        }

        public void Update(Journal journal)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // SQL query to update an existing journal in the database
                    cmd.CommandText = @"UPDATE Journal
                                           SET Title = @title,
                                               Content = @content
                                         WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@title", journal.Title);
                    cmd.Parameters.AddWithValue("@content", journal.Content);
                    cmd.Parameters.AddWithValue("@id", journal.Id);

                    cmd.ExecuteNonQuery(); // Execute the update query
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // SQL query to delete a journal from the database by its id
                    cmd.CommandText = @"DELETE FROM Journal
                                       WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); // Execute the delete query
                }
            }
        }

        // Additional functions

        public void EditJournalEntry()
        {
            Console.WriteLine("Select the entry you want to edit:");
            List<Journal> journals = GetAll();

            // Display the list of journal entries
            foreach (Journal journal in journals)
            {
                Console.WriteLine($"{journal.Id}: {journal.Title}");
            }

            Console.Write("Enter the ID of the entry you want to edit: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int id))
            {
                Journal journal = Get(id);

                if (journal != null)
                {
                    Console.WriteLine($"Editing entry with ID: {journal.Id}");
                    Console.WriteLine($"Current Title: {journal.Title}");
                    Console.WriteLine($"Current Content:\n{journal.Content}");

                    Console.WriteLine("Enter new title (leave empty to keep current value):");
                    string newTitle = Console.ReadLine();

                    Console.WriteLine("Enter new content (leave empty to keep current value):");
                    string newContent = Console.ReadLine();

                    // Update the journal entry with the new values
                    if (!string.IsNullOrWhiteSpace(newTitle))
                    {
                        journal.Title = newTitle;
                    }
                    if (!string.IsNullOrWhiteSpace(newContent))
                    {
                        journal.Content = newContent;
                    }

                    Update(journal);

                    Console.WriteLine("Entry updated successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid entry ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid entry ID.");
            }
        }

        public SearchResults<Post> SearchPosts(string tagName)
        {
            throw new NotImplementedException();
        }
    }
}
