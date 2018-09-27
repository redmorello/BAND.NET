using System.Collections.Generic;
using System.Threading.Tasks;
using BAND.Models;

namespace BAND
{
    public interface IBandClient
    {
        Task<List<Band>> GetBands();
        Task<Posts> GetPosts(string bandKey, string locale);
        Task<Post> GetPost(string bandKey, string postKey);
        //Task<Post> WritePost();
        //Task<Post> DeletePost();
        Task<List<Comment>> GetComments(string bandKey, string postKey, string sort = default(string));
        //Task<Comment> WriteComment();
        //Task<Comment> DeleteComment();
        Task<List<string>> CheckPermissions(string bandKey, string permissions);
        Task<Albums> GetAlbums(string bandKey);
        Task<Photos> GetPhotos(string bandKey, string photoAlbumKey);

    }
}
