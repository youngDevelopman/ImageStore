﻿using ImageStore.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ImageStore.Application.Comments.Commands.AddComment
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, AddedCommentResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;

        private const int MAX_ADD_COMMENT_ATTEMPS = 5;

        public AddCommentCommandHandler(IUnitOfWork unitOfWork, ICommentRepository commentRepository, IPostRepository postRepository)
        {
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }

        public async Task<AddedCommentResultDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            Guid commentId = default;
            var saved = false;
            int attemps = 0;
            while (!saved && MAX_ADD_COMMENT_ATTEMPS > attemps)
            {
                try
                {
                    var post = await _postRepository.GetPostByIdAsync(request.PostId, cancellationToken);
                    if (post == null)
                    {
                        throw new PostNotFoundException();
                    }

                    var comment = new Comment()
                    {
                        Content = request.Content,
                        PostId = request.PostId,
                        UserId = request.UserId,
                    };

                    // TODO: Consider creating AddComment method for Post model
                    await _commentRepository.AddCommentAsync(comment, cancellationToken);
                    post.CommentsCount++;

                    await _unitOfWork.SaveChangesAsync();
                    commentId = comment.Id;
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    /*
                     The idea here is while comment is being added another comment might be added concurrently updating the comments count,
                        thus setting comments count to the inconsistent state. 
                    
                    In that case, DbUpdateConcurrencyException would be raised because row version id has been configured on the server side.

                    And here is the logic for setting in memory comment count to its actual db counterpart 
                        and keep retrying x amount of times until the comment is added.
                    */
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is Post)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            var commentCountProperty = proposedValues.Properties.First(x => x.Name == nameof(Post.CommentsCount));
                            var proposedCommentCount = proposedValues[commentCountProperty];
                            var databaseCommentCount = databaseValues[commentCountProperty];

                            proposedValues[commentCountProperty] = databaseCommentCount;

                            entry.OriginalValues.SetValues(databaseValues);
                        }
                    }
                }

                attemps++;
            }

            if(!saved)
            {
                throw new InvalidOperationException($"Error while adding the comment. Try again");
            }

            return new AddedCommentResultDto(commentId, request.Content);
        }
    }
}
