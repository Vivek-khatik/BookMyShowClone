using BookMyShow.Application.DTOs.Movies.Actors;
using FluentValidation;
using System;

namespace BookMyShow.Application.Validators.Movies.Actors;

public class CreateActorRequestValidator : AbstractValidator<CreateActorRequest>
{
    public CreateActorRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ProfileImageUrl)
            .Must(uri => string.IsNullOrEmpty(uri) || (Uri.TryCreate(uri, UriKind.Absolute, out var outUri) && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)))
            .WithMessage("ProfileImageUrl must be a valid URL.")
            .MaximumLength(1000);
    }
}
