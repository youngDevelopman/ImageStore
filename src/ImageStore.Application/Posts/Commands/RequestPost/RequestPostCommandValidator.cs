using FluentValidation;

namespace ImageStore.Application.Posts.Commands.RequestPost
{
    public class RequestPostCommandValidator : AbstractValidator<RequestPostCommand>
    {
        public RequestPostCommandValidator()
        {
            var allowedFormats = new[] { ".jpg", "png", "bmp" };
            long allowedFileLength = 104857600; // 100 MB

            RuleFor(x => x.fileExtension)
                .Must(fileExtension => allowedFormats.Contains(fileExtension))
                .WithName("File extension")
                .WithMessage($"File extension must be either {string.Join(", ", allowedFormats)}");

            RuleFor(x => x.File.Length)
                .LessThan(allowedFileLength)
                .WithName("File format")
                .WithMessage($"Image length has to be less than {allowedFileLength}");
        }
    }
}
