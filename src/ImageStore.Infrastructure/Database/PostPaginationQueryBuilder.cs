using ImageStore.Domain.Entities;
using ImageStore.Domain.Interfaces;
using ImageStore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageStore.Infrastructure.Database
{
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
            if(_paginationStrategy == PaginationStrategy.PreviousPage)
            {
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
                _queryResult = _queryResult
                    .OrderByDescending(post => post.CommentsCount)
                    .ThenByDescending(post => post.UpdatedAt)
                    .ThenByDescending(post => post.Id);

                if(_paginationStrategy == PaginationStrategy.NextPage)
                {
                    _queryResult = _queryResult.Where(post =>
                        post.CommentsCount < _postCursor.CommentCount || post.CommentsCount == _postCursor.CommentCount
                        && (post.UpdatedAt < _postCursor.UpdatedAt
                            || (post.UpdatedAt == _postCursor.UpdatedAt && post.Id < _postCursor.PostId)));
                }
            }

            if (_includeComments)
            {
                _queryResult = _queryResult
                    .Include(post => post.Comments);
            }

            _queryResult = _queryResult
                .Take(_size);

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
