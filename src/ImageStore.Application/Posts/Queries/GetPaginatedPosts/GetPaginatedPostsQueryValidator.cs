namespace ImageStore.Application.Posts.Queries.GetPaginatedPosts
{
    public class GetPaginatedPostsQueryValidator : AbstractValidator<GetPaginatedPostsQuery>
    {
        public GetPaginatedPostsQueryValidator()
        {
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(50)
                .WithName("Page size");


            RuleFor(cursor => cursor)
                 .Must(cursor => !(!string.IsNullOrEmpty(cursor.Next) && !string.IsNullOrEmpty(cursor.Previous)))
                 .WithName("Cursors")
                 .WithMessage("Both 'Next' and 'Previous' cursors cannot be set at the same time.");
        }
    }
}
