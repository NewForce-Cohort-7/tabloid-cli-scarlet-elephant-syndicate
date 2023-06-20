using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.UserInterfaceManagers;

namespace TabloidCLI.Repositories
{
    public class PostRepository : DatabaseConnector, IRepository<Post>
    {
        public PostRepository(string connectionString) : base(connectionString) { }

        public List<Post> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT p.Id, 
                                       p.Title, 
                                       p.URL,    
                                       p.PublishDateTime, 
                                       p.AuthorId, 
                                       p.BlogId 
                                       FROM Post p 
                                       JOIN Author a on p.AuthorId = a.Id 
                                       JOIN Blog b on p.BlogId = b.Id";


                    List<Post> posts = new List<Post>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Post post = new Post()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Url = reader.GetString(reader.GetOrdinal("URL")),
                            PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),
                            Author = new Author()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                            },
                            Blog = new Blog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("BlogId")),
                            },

                        };
                        posts.Add(post);
                    }
                    reader.Close();
                    return posts;
                }
            }
        }


        public Post Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {//add SQL search to find post tags. Joined to tag on ID
                    cmd.CommandText = @"SELECT p.Id AS PostId,
                                               p.Title,
                                               p.URL,
                                               p.PublishDateTime,
                                               pt.TagId,
                                               t.Name
                                          FROM Post p 
                                          LEFT JOIN PostTag pt on p.Id = pt.PostId
                                          LEFT JOIN Tag t on t.Id = pt.TagId
                                          WHERE p.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    Post post = null;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())//this while loop and first if statement gets the posts. If a post is null creates a new post. If the tag is null it allows input of a new tag
                    {
                        if (post == null)
                        {
                            post = new Post()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("PostId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Url = reader.GetString(reader.GetOrdinal("Url")),
                                PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime"))
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("Name"))) //checks to see if name (what we are calling in the post tags) is null
                            {
                                Tag tag = new Tag()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                };
                                post.Tags.Add(tag);
                            }
                        }
                        else //if there is a post with no tag you can make a tag
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Name"))) 
                            {
                                Tag tag = new Tag()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                };
                                post.Tags.Add(tag);
                            }
                        }

                    }

                    reader.Close();

                    return post;
                }
            }
        }

        public List<Post> GetByAuthor(int authorId)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT p.id,
                                               p.Title As PostTitle,
                                               p.URL AS PostUrl,
                                               p.PublishDateTime,
                                               p.AuthorId,
                                               p.BlogId,
                                               a.FirstName,
                                               a.LastName,
                                               a.Bio,
                                               b.Title AS BlogTitle,
                                               b.URL AS BlogUrl
                                          FROM Post p 
                                               LEFT JOIN Author a on p.AuthorId = a.Id
                                               LEFT JOIN Blog b on p.BlogId = b.Id 
                                         WHERE p.AuthorId = @authorId";
                        cmd.Parameters.AddWithValue("@authorId", authorId);
                        SqlDataReader reader = cmd.ExecuteReader();

                        List<Post> posts = new List<Post>();
                        while (reader.Read())
                        {
                            Post post = new Post()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Url = reader.GetString(reader.GetOrdinal("Url")),
                                PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),
                                Author = new Author()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    Bio = reader.GetString(reader.GetOrdinal("Bio")),
                                },
                                Blog = new Blog()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("BlogId")),
                                    Title = reader.GetString(reader.GetOrdinal("BlogTitle")),
                                    Url = reader.GetString(reader.GetOrdinal("BlogUrl")),
                                }
                            };
                            posts.Add(post);
                        }

                        reader.Close();

                        return posts;
                    }
                }
            }

            public void Insert(Post post)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    //Prompts user to select a blog
                    Console.WriteLine("Select a blog:");
                    List<Blog> blogs = GetBlogs(); // Retrieves a list of blogs from the database
                    for (int i = 0; i < blogs.Count; i++) // Iterates through the list of blogs
                    {
                        Console.WriteLine($"{i + 1}) {blogs[i].Title}"); // Displays the current blog's title
                    }

                    int blogIndex;
                    do
                    {
                        Console.Write("Enter the number of the blog: "); // Prompts the user to select a blog
                    } while (!int.TryParse(Console.ReadLine(), out blogIndex) || blogIndex < 1 || blogIndex > blogs.Count); // Validates the user's selection

                    int blogId = blogs[blogIndex - 1].Id; // Gets the Id of the selected blog

                    //Prompts user to select an author
                    Console.WriteLine("Select an author:");
                    List<Author> authors = GetAuthors(); // Retrieves a list of authors from the database
                    for (int i = 0; i < authors.Count; i++) // Iterates through the list of authors
                    {
                        Author author = authors[i]; // Stores the current author in a variable
                        Console.WriteLine($"{i + 1}) {author.FullName}"); // Displays the current author's full name
                    }

                    int authorIndex; // Declares a variable to store the user's selection
                    do
                    {
                        Console.Write("Enter the number of the author: "); // Prompts the user to select an author
                    } while (!int.TryParse(Console.ReadLine(), out authorIndex) || authorIndex < 1 || authorIndex > authors.Count); // Validates the user's selection

                    int authorId = authors[authorIndex - 1].Id; // Gets the Id of the selected author

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Post (Title, URL, PublishDateTime, AuthorId, BlogId)
                        OUTPUT INSERTED.Id
                        VALUES (@title, @url, @publishDateTime, @authorId,
                                @blogId)";
                        cmd.Parameters.AddWithValue("@title", post.Title);
                        cmd.Parameters.AddWithValue("@url", post.Url);
                        cmd.Parameters.AddWithValue("@publishDateTime", post.PublishDateTime);
                        cmd.Parameters.AddWithValue("@authorId",
                                                   authorId);
                        cmd.Parameters.AddWithValue("@blogId", blogId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            public void Update(Post post)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        UPDATE Post
                           SET Title = @title,
                               URL = @url,
                               PublishDateTime = @publishDateTime,
                               AuthorId = @author,
                               BlogId = @blog
                         WHERE Id = @id";

                        cmd.Parameters.AddWithValue("@title", post.Title);
                        cmd.Parameters.AddWithValue("@url", post.Url);
                        cmd.Parameters.AddWithValue("@publishDateTime", post.PublishDateTime);
                        cmd.Parameters.AddWithValue("@author",
                                                                          post.Author.Id);
                        cmd.Parameters.AddWithValue("@blog", post.Blog.Id);
                        cmd.Parameters.AddWithValue("@id", post.Id);

                        cmd.ExecuteNonQuery();
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
                        cmd.CommandText = @"DELETE FROM Post WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            public void InsertTag(Post post, Tag tag)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText =
                            @"INSERT INTO PostTag (PostId, TagId)
                        OUTPUT INSERTED.Id
                        VALUES (@postId, @tagId)";
                        cmd.Parameters.AddWithValue("@postId", post.Id);
                        cmd.Parameters.AddWithValue("@tagId", tag.Id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            public void DeleteTag(int postId, int tagId)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText =
                            @"DELETE FROM PostTag WHERE PostId = @postId AND TagId = @tagId";
                        cmd.Parameters.AddWithValue("@postId", postId);
                        cmd.Parameters.AddWithValue("@tagId", tagId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            public List<Author> GetAuthors()
            {
                List<Author> authors = new List<Author>();

                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, FirstName, LastName
                        FROM Author";

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("Id"));
                                string firstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                string lastName = reader.GetString(reader.GetOrdinal("LastName"));

                                Author author = new Author()
                                {
                                    Id = id,
                                    FirstName = firstName,
                                    LastName = lastName
                                };

                                authors.Add(author);
                            }
                        }
                    }
                }
                return authors;
            }

            public List<Blog> GetBlogs()
            {
                List<Blog> blogs = new List<Blog>();

                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT Id, Title, Url
                        FROM Blog";

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("Id"));
                                string title = reader.GetString(reader.GetOrdinal("Title"));
                                string url = reader.GetString(reader.GetOrdinal("Url"));

                                Blog blog = new Blog
                                {
                                    Id = id,
                                    Title = title,
                                    Url = url
                                };

                                blogs.Add(blog);
                            }
                        }
                    }
                }
                return blogs;
            }

        public SearchResults<Post> SearchPosts(string tagName)
        {
            throw new NotImplementedException();
        }
    }
        
    }
