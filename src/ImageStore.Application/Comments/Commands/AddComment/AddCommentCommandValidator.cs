namespace ImageStore.Application.Comments.Commands.AddComment
{
    public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentCommandValidator()
        {
            RuleFor(x => x.Content)
                .MaximumLength(100);
        }
    }
}
