using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.UserInterfaceManagers;

namespace TabloidCLI
{
    public interface IRepository<TEntity>
    {
        List<TEntity> GetAll();
        TEntity Get(int id);
        void Insert(TEntity entry);
        void Update(TEntity entry);
        void Delete(int id);
        SearchResults<Post> SearchPosts(string tagName);
    }
}