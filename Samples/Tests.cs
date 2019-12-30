using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using Xunit;

namespace Samples
{
	public class Tests
	{
		[Fact]
		public void PassConfiguration()
		{
			// Arrange
			using var container = new Container();

			container.RegisterInstance(new SomeServiceSettings("test url"));
			container.RegisterSingleton<ISomeService, SomeService>();

			// Act
			var result = container.GetInstance<ISomeService>();

			// Assert
			Assert.IsType<SomeService>(result);
		}

		[Fact]
		public void Verification_MissedRegistration()
		{
			// Arrange
			using var container = new Container();

			container.Register<ISomeService, SomeService>();
			container.Register<CircuitService1>();
			container.Register<CircuitService2>();

			// Act
			var result = Assert.Throws<InvalidOperationException>(() => container.Verify(VerificationOption.VerifyAndDiagnose));

			// Assert
			var activationException = Assert.IsType<ActivationException>(result.InnerException);
			Assert.Contains(nameof(SomeService), activationException.Message);
		}

		[Fact]
		public void Verification_CircuitDependency()
		{
			// Arrange
			using var container = new Container();

			container.Register<CircuitService1>();
			container.Register<CircuitService2>();

			// Act
			var result = Assert.Throws<InvalidOperationException>(() => container.Verify(VerificationOption.VerifyAndDiagnose));

			// Assert
			var activationException = Assert.IsType<ActivationException>(result.InnerException);
			Assert.Contains("CircuitService", activationException.Message);
		}

		[Fact]
		public void Decorate()
		{
			// Arrange
			using var container = new Container();

			container.RegisterInstance(new SomeServiceSettings("test url"));
			container.RegisterSingleton<ISomeService, SomeService>();
			container.RegisterDecorator<ISomeService, TracingSomeServiceDecorator>();

			// Act
			var result = container.GetInstance<ISomeService>();

			// Assert
			Assert.IsType<TracingSomeServiceDecorator>(result);
		}

		[Fact]
		public void Composition()
		{
			// Arrange
			using var container = new Container();

			container.Register<ISpecification, CompositeSpecification>();
			container.Collection.Register<ISpecification>(typeof(Specification1), typeof(Specification2));

			// Act
			var result = container.GetInstance<ISpecification>();

			// Assert
			var composite = Assert.IsType<CompositeSpecification>(result);
			Assert.IsType<Specification1>(composite.Specifications[0]);
			Assert.IsType<Specification2>(composite.Specifications[1]);
		}

		[Fact]
		public void AutoScan()
		{
			// Arrange
			using var container = new Container();

			container.Register<ISpecification, CompositeSpecification>();
			container.Collection
				.Register<ISpecification>(GetType().Assembly.GetExportedTypes()
					.Where(x => !x.IsAbstract && !x.IsInterface)
					.Where(x => typeof(ISpecification).IsAssignableFrom(x))
					.Where(x => x != typeof(CompositeSpecification)));

			// Act
			var result = container.GetInstance<ISpecification>();

			// Assert
			var composite = Assert.IsType<CompositeSpecification>(result);
			Assert.NotEmpty(composite.Specifications);
		}

		[Fact]
		public void Conditional()
		{
			// Arrange
			using var container = new Container();

			container.Register<ISpecification, CompositeSpecification>();
			container.Collection
				.Register<ISpecification>(GetType().Assembly.GetExportedTypes()
					.Where(x => !x.IsAbstract && !x.IsInterface)
					.Where(x => typeof(ISpecification).IsAssignableFrom(x))
					.Where(x => x != typeof(CompositeSpecification)));

			// Act
			var result = container.GetInstance<ISpecification>();

			// Assert
			var composite = Assert.IsType<CompositeSpecification>(result);
			Assert.NotEmpty(composite.Specifications);
		}
	}
}