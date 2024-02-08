using ImageStore.Domain.Interfaces;
using System.Text;
using System.Web;

namespace ImageStore.Domain.Models
{
    public class PostCursor
    {
        public Guid PostId { get; }
        public DateTime UpdatedAt { get; }
        public int CommentCount { get; }

        public PostCursor(Guid postId, DateTime updatedAt, int commentCount)
        {
            if (commentCount < 0)
            {
                throw new ArgumentOutOfRangeException("Comment count cannot be less than zero");
            }

            PostId = postId;
            UpdatedAt = updatedAt;
            CommentCount = commentCount;
        }

        public static PostCursor CreateFromBase64UrlEncoded(string encoded)
        {
            if(string.IsNullOrEmpty(encoded))
            {
                throw new ArgumentNullException($"Url encoded cannot be empty");
            }

            byte[] bytes = System.Convert.FromBase64String(encoded);
            string[] decodedStrings = System.Text.ASCIIEncoding.ASCII.GetString(bytes).Split('_');

            if (decodedStrings.Length != 3)
                throw new ArgumentException($"Unable to parse encoded cursor string {encoded}");

            if (Guid.TryParse(decodedStrings[0], out var postId) 
                && int.TryParse(decodedStrings[1], out var commentCount)
                && DateTime.TryParse(decodedStrings[2], out var updatedAt))
            {
                return new PostCursor(postId, updatedAt, commentCount);
            }
            else
            {
                throw new ArgumentException($"Unable to parse encoded cursor data. Cursor: {encoded}");
            }
        }

        public static PostCursor CreateFromPost(Post post)
        {
            return new PostCursor(post.Id, post.UpdatedAt, post.CommentsCount);
        }

        public string GetBase64Encoded()
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes($"{PostId}_{CommentCount}_{UpdatedAt}");
            return Convert.ToBase64String(bytes);
        }
    }
}
