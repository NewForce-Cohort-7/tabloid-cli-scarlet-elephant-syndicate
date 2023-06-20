using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using TabloidCLI.Models;
using TabloidCLI.UserInterfaceManagers;

namespace TabloidCLI.Repositories
{
    public class BlogRepository : DatabaseConnector, IRepository<Blog>
    {
        public BlogRepository(string connectionString) : base(connectionString) { }

        public List<Blog> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id,
                                               Title,
                                               Url
                                          FROM Blog";

                    List<Blog> blogs = new List<Blog>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Blog blog = new Blog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Url = reader.GetString(reader.GetOrdinal("Url")),
                        };
                        blogs.Add(blog);
                    }

                    reader.Close();

                    return blogs;
                }
            }
        }//End of GetAll (THIS ONE IS COMPLETED)
        public Blog Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                    // Adding a join to the query to get the tags for the blog
                {
                    cmd.CommandText = @"SELECT
                                               b.Id AS BlogId,                 
                                               b.Title,
                                               b.URL,
                                               bt.TagId,
                                               t.Name
                                          FROM Blog b
                                          LEFT JOIN BlogTag bt ON b.Id = bt.BlogId
                                          LEFT JOIN Tag t ON t.Id = bt.TagId
                                         WHERE b.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    Blog blog = null;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) //this is a while loop because we are getting multiple rows back
                    {
                        if (blog == null)
                        {
                            blog = new Blog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("BlogId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Url = reader.GetString(reader.GetOrdinal("Url")),
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                            {
                                Tag tag = new Tag()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                };
                                blog.Tags.Add(tag);
                            }
                        }
                        else
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                            {
                                Tag tag = new Tag()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                };
                                blog.Tags.Add(tag);
                            }
                        }
                    }

                        reader.Close();
                    

                    return blog;
                }
            }
        } //End of Get
        public void Insert(Blog blog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Blog (Title, Url )
                                                     VALUES (@title, @url)";
                    cmd.Parameters.AddWithValue("@title", blog.Title);
                    cmd.Parameters.AddWithValue("@url", blog.Url);

                    cmd.ExecuteNonQuery();
                }
            }
        }//End of Insert
        public void Update(Blog blog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Blog 
                                           SET Title = @title,
                                               Url = @url
                                         WHERE id = @id";

                    cmd.Parameters.AddWithValue("@title", blog.Title);
                    cmd.Parameters.AddWithValue("@url", blog.Url);
                    cmd.Parameters.AddWithValue("@id", blog.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }//End of Update
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Blog WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }//End of Delete

        // Adding method to add a tag to a blog
        public void InsertTag(Blog blog, Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =
                        @"INSERT INTO BlogTag (BlogId, TagId)
                            OUTPUT INSERTED.Id
                            VALUES (@blogId, @tagId)";
                    cmd.Parameters.AddWithValue("@blogId", blog.Id);
                    cmd.Parameters.AddWithValue("@tagId", tag.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public SearchResults<Post> SearchPosts(string tagName)
        {
            throw new NotImplementedException();
        }

        public List<Post> GetPostByBlog(int blogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT p.id as PostId,
                                        p.Title as PostTitle,
                                        p.URL as PostURL,
                                        p.PublishDateTime,
                                        p.AuthorId, 
                                        p.BlogId,
                                        b.Title AS BlogTitle,
                                        b.URL AS BlogUrl
                                        FROM Post p 
                                        LEFT JOIN Blog b on p.BlogId = b.Id 
                                           WHERE p.Blogid = @blogId";

                    cmd.Parameters.AddWithValue("@blogId", blogId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Post> posts = new List<Post>();
                    while (reader.Read())
                    {
                        Post post = new Post()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("PostId")),
                            Title = reader.GetString(reader.GetOrdinal("PostTitle")),
                            Url = reader.GetString(reader.GetOrdinal("PostURL")),
                            PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),

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


    }//End of class
}//End of namespace
