using ImageStore.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ImageStore.Application.Comments.Commands.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepository;

        private const int MAX_REMOVE_COMMENT_ATTEMPS = 5;
        public DeleteCommentCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, ICommentRepository commentRepository)
        {
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
        }

        public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(request.CommentId, cancellationToken);
            if (comment == null)
            {
                throw new CommentNotFoundException($"Comment with id of {request.CommentId} has not been found.");
            }

            if(request.PostId !=  comment.PostId)
            {
                throw new InvalidOperationException($"Provided prost id {request.PostId} does not match the post id of the comment {comment.PostId}");
            }

            if (comment.UserId != request.UserId)
            {
                throw new InvalidOperationException($"Comment with id of {request.CommentId} does not belong to the user with id of {request.UserId}");
            }

            var saved = false;
            int attemps = 0;
            while (!saved && MAX_REMOVE_COMMENT_ATTEMPS > attemps)
            {
                try
                {
                    var post = comment.Post;
                    _commentRepository.RemoveComment(comment, cancellationToken);
                    post.CommentsCount--;

                    await _unitOfWork.SaveChangesAsync();
                    saved = true;
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    /*
                     The idea here is while comment is being removed another comment might be added concurrently updating the comments count,
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

            if (!saved)
            {
                throw new InvalidOperationException($"Error while deleting the comment {comment.Id}. Try again");
            }
        }
    }
}
