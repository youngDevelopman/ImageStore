using ImageStore.Domain.Models;

namespace ImageStore.Infrastructure.Database
{
    /// <summary>
    /// This class is used for counstructing post pagination query 
    /// </summary>
    public class PostPaginationQueryBuilder
    {
        private IQueryable<Post> _queryResult;
        private int _size = 10;
        private bool _includeComments = false;
        private PostCursor _postCursor;
        private PaginationStrategy _paginationStrategy;

        public PostPaginationQueryBuilder(ApplicationDbContext context)
        {
            _queryResult = context.Posts.AsQueryable();
        }

        /// <summary>
        /// Set the pagination strategy. Remember, if set to PreviousCount the query that was built would return the previous set of data in wrong order. Make sure to reverse it back.
        /// </summary>
        /// <param name="paginationStrategy"> </param>
        /// <returns></returns>
        public PostPaginationQueryBuilder SetPaginationStrategy(PaginationStrategy paginationStrategy)
        {
            _paginationStrategy = paginationStrategy;
            return this;
        }

        public PostPaginationQueryBuilder SetCursor(PostCursor postCursor)
        {
            _postCursor = postCursor;
            return this;
        }

        public PostPaginationQueryBuilder WithSize(int size)
        {
            _size = size;
            return this;
        }

        public PostPaginationQueryBuilder IncludeComments()
        {
            _includeComments = true;
            return this;
        }

        public IQueryable<Post> Build()
        {
            /*
                The requirement is to take the list of posts sorted by the number of comments using the cursor based pagination.

                Cursor-based pagination requires sorting by the unique key to generate cursor, otherwise we would not get consistent results
                So, the both id and comment count fields need to be included. 
                Updated at was also added to define the cursor, because we would have an ability to obtain the latest posts
            */

            if(_paginationStrategy == PaginationStrategy.PreviousPage)
            {
                /* In case PreviousPage pagination strategy we want to retrieve the previous 
                  set of data starting from cursor that is provided. 
                
                For that we are going to do a simple trick - reverse the order of the ORDER BY fields
                    and reverse the filteing conditions (i.e. change less than to greater than)

                But there is one thing to remember 
                    - the returned data is sorted in reverse, so we need to apply an additional ordering 
                    to bring the data back to the desired order.
                */
                _queryResult = _queryResult
                    .OrderBy(post => post.CommentsCount)
                    .ThenBy(post => post.UpdatedAt)
                    .ThenBy(post => post.Id)
                    .Where(post =>
                        post.CommentsCount > _postCursor.CommentCount || post.CommentsCount == _postCursor.CommentCount
                        && (post.UpdatedAt > _postCursor.UpdatedAt 
                        || (post.UpdatedAt == _postCursor.UpdatedAt && post.Id > _postCursor.PostId)));
            }
            else
            {
                // Set a base ordering query
                _queryResult = _queryResult
                    .OrderByDescending(post => post.CommentsCount)
                    .ThenByDescending(post => post.UpdatedAt)
                    .ThenByDescending(post => post.Id);

                /* To retrieve next page we need to provide cursor and add the filtering logic 
                    which helps us to determine where the starting post item is located

                    if pagination strategy is FirstPage, there is no need to apply the filtering below, 
                        because we always want to take the x amount of posts right from the start
                 * */
                if(_paginationStrategy == PaginationStrategy.NextPage)
                {
                    _queryResult = _queryResult.Where(post =>
                        post.CommentsCount < _postCursor.CommentCount || post.CommentsCount == _postCursor.CommentCount
                        && (post.UpdatedAt < _postCursor.UpdatedAt
                            || (post.UpdatedAt == _postCursor.UpdatedAt && post.Id < _postCursor.PostId)));
                }
            }

            // Include the comments
            if (_includeComments)
            {
                _queryResult = _queryResult
                    .Include(post => post.Comments);
            }

            // Take the _size amount of records
            _queryResult = _queryResult
                .Take(_size);

            // Take x number of the latest comments of the post
            if (_includeComments)
            {
                _queryResult = _queryResult
                    .Select(post => new Post()
                    {
                        Id = post.Id,
                        Caption = post.Caption,
                        CommentsCount = post.CommentsCount,
                        CreatedAt = post.CreatedAt,
                        Image = post.Image,
                        UserId = post.UserId,
                        User = post.User,
                        UpdatedAt = post.UpdatedAt,
                        Version = post.Version,
                        Comments = post.Comments
                            .OrderByDescending(comment => comment.CreatedAt)
                            .Take(2)
                    });
            }

            return _queryResult;
        }
    }
}
