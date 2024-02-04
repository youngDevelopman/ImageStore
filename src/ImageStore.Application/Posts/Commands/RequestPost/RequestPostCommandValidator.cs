using FluentValidation;

namespace ImageStore.Application.Posts.Commands.RequestPost
{
    public class RequestPostCommandValidator : AbstractValidator<RequestPostCommand>
    {
        public RequestPostCommandValidator()
        {
            var allowedFormats = new[] { ".jpg", "png", "bmp" };
            long allowedFileLength = 10; //104857600;

            RuleFor(x => x.fileExtension)
                .Must(fileExtension => allowedFormats.Contains(fileExtension))
                .WithMessage($"File extension must be either {string.Join(", ", allowedFormats)}");

            RuleFor(x => x.File.Length)
                .LessThan(allowedFileLength)
                .WithMessage($"Image length has to be less than {allowedFileLength}");
        }
    }
}
