using FluentValidation;
using UserPermission.Application.UserCases.Create.Commands;

namespace UserPermission.Application.UserCases.Create.Validations
{
    public class ModifyPermissionValidation : AbstractValidator<CreatePermissionCommand>
    {
        public ModifyPermissionValidation()
        {
            this.RuleFor(command => command.PermissionTypeId)
                .GreaterThan(x => 0)
                    .WithMessage("The PermissionTypeId not must minor or equal than 0.");

            this.RuleFor(command => command.EmployeeForename)
                .NotEmpty()
                    .WithMessage("The EmployeeForename not must empty.");

            this.RuleFor(command => command.EmployeeSurname)
                .NotEmpty()
                    .WithMessage("The EmployeeSurname not must empty.");
        }
    }
}
