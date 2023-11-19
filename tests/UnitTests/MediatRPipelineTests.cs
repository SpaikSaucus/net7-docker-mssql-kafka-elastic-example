using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using UserPermission.Application.Behaviors;
using Xunit;

namespace UserPermission.UnitTests
{
    public class MediatRPipelineTests
    {
        [Fact]
        public async Task ValidationPipelineBehaviorShouldDelegateInFluentAssertionsFramework()
        {
            //Arrange
            var validationWithNoErrors = new ValidationResult();
            var nextDelegate = A.Fake<RequestHandlerDelegate<object>>();
            var validator = A.Fake<IValidator<object>>();
            A.CallTo(() => validator.Validate(A<object>.Ignored)).Returns(validationWithNoErrors);

            var validatorBehavior = new ValidatorBehavior<IRequest<object>, object>(new[] {validator});

            //Act
            var _ = await validatorBehavior.Handle(null, nextDelegate, CancellationToken.None);

            //Assert
            A.CallTo(() => validator.Validate(A<object>.Ignored)).MustHaveHappened();
            A.CallTo(() => nextDelegate.Invoke()).MustHaveHappened();
        }

        [Fact]
        public async Task ValidationPipelineBehaviorShouldThrowExceptionOnValidationError()
        {
            //Arrange
            var validationWithErrors = new ValidationResult(new []{ new ValidationFailure("someProperty", "An error message")});
            var nextDelegate = A.Fake<RequestHandlerDelegate<object>>();
            var validator = A.Fake<IValidator<object>>();
            A.CallTo(() => validator.Validate(A<object>.Ignored)).Returns(validationWithErrors);

            var validatorBehavior = new ValidatorBehavior<IRequest<object>, object>(new[] { validator });

            //Act
            Func<Task<object>> act = async () => await validatorBehavior.Handle(null, nextDelegate, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<ValidationException>();
            A.CallTo(() => validator.Validate(A<object>.Ignored)).MustHaveHappened();
            A.CallTo(() => nextDelegate.Invoke()).MustNotHaveHappened();
        }

        [Fact]
        public async Task LoggingPipelineBehaviorShouldDelegateToMsExtensionsLoggingFramework()
        {
            //Arrange
            var sampleRequest = A.Fake<IRequest<object>>();
            var nextDelegate = A.Fake<RequestHandlerDelegate<object>>();
            var logger = A.Fake<ILogger<LoggingBehavior<IRequest<object>, object>>>();
            var loggingBehavior = new LoggingBehavior<IRequest<object>, object>(logger);

            //Act
            var _ = await loggingBehavior.Handle(sampleRequest, nextDelegate, CancellationToken.None);

            //Assert
            A.CallTo(logger).Where(call => call.Method.Name == "Log").MustHaveHappenedTwiceExactly();
            A.CallTo(() => nextDelegate.Invoke()).MustHaveHappened();
        }
    }
}
