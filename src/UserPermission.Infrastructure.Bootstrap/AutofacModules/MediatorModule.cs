using Autofac;
using FluentValidation;
using MediatR;
using UserPermission.Application.Behaviors;

namespace UserPermission.Infrastructure.Bootstrap.AutofacModules
{
    public class MediatorModule : Module
    {
        private readonly bool enableCommandLogging;

        public MediatorModule(bool enableCommandLogging)
        {
            this.enableCommandLogging = enableCommandLogging;
        }

        protected override void Load(ContainerBuilder builder)
        {
            //Discover "Application" service layer validations and auto-register with MediatR pipeline
            builder.RegisterAssemblyTypes(typeof(ValidatorBehavior<,>).Assembly)
                .AsClosedTypesOf(typeof(IValidator<>))
                .AsImplementedInterfaces();
            
            if (this.enableCommandLogging)
            {
                builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            }

            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            
        }
    }
}
