using FluentValidation;
using UserPermission.Application.UserCases.Update.Commands;

namespace UserPermission.Application.UserCases.Modify.Validations
{
    public class ModifyPermissionValidation : AbstractValidator<ModifyPermissionCommand>
    {
        public ModifyPermissionValidation()
        {
            this.RuleFor(command => command)
                .Custom((obj, context) =>
                {
                    if (string.IsNullOrEmpty(obj.EmployeeForename) &&
                        string.IsNullOrEmpty(obj.EmployeeSurname) &&
                        obj.PermissionTypeId <= 0)
                    {
                        context.AddFailure("There are no values in the request. Complete at least 1 field.");
                    }
                });
        }
    }
}
