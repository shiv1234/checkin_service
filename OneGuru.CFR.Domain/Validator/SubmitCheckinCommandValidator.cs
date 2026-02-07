using FluentValidation;
using OneGuru.CFR.Domain.Commands;

namespace OneGuru.CFR.Domain.Validator
{
    public class SubmitCheckinCommandValidator : AbstractValidator<SubmitCheckinCommand>
    {
        public SubmitCheckinCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("Employee ID is required.");

            RuleFor(x => x.PulseScore)
                .NotNull()
                .WithMessage("Pulse score is required before submission.")
                .InclusiveBetween(1, 5)
                .When(x => x.PulseScore.HasValue)
                .WithMessage("Pulse score must be between 1 and 5.");

            RuleFor(x => x.WeekNumber)
                .InclusiveBetween(1, 53)
                .WithMessage("Week number must be between 1 and 53.");

            RuleFor(x => x.Year)
                .GreaterThan(2020)
                .WithMessage("Year must be greater than 2020.");

            RuleFor(x => x.TaskCompletionRate)
                .InclusiveBetween(0, 100)
                .When(x => x.TaskCompletionRate.HasValue)
                .WithMessage("Task completion rate must be between 0 and 100.");

            RuleFor(x => x.Wins)
                .MaximumLength(4000)
                .When(x => x.Wins != null);

            RuleFor(x => x.Blockers)
                .MaximumLength(4000)
                .When(x => x.Blockers != null);

            RuleFor(x => x.ManagerNote)
                .MaximumLength(2000)
                .When(x => x.ManagerNote != null);
        }
    }
}
