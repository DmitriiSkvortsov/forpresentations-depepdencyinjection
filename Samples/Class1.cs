using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleInjector;
using Xunit;

namespace Samples
{
	public class SimpleSamples
	{
		[Fact]
		public void PassConfiguration()
		{
			// Arrange
			using var container = new Container();

			container.RegisterInstance(new EndpointSettings("catalogUrl", "arbitrationUrl"));
			container.RegisterSingleton<ICatalogService, CatalogService>();

			// Act
			var result = container.GetInstance<ICatalogService>();

			// Assert
			Assert.IsType<CatalogService>(result);
		}

		[Fact]
		public void Verification()
		{
			// Arrange
			using var container = new Container();

			container.Register<ICatalogService, CatalogService>();

			// Act
			var result = Assert.Throws<InvalidOperationException>(() => container.Verify(VerificationOption.VerifyAndDiagnose));

			// Assert
			var activationException = Assert.IsType<ActivationException>(result.InnerException);
			Assert.Contains(nameof(CatalogService), activationException.Message);
		}

		[Fact]
		public void Decorate()
		{
			// Arrange
			using var container = new Container();

			container.RegisterInstance(new EndpointSettings("catalogUrl", "arbitrationUrl"));
			container.RegisterSingleton<ICatalogService, CatalogService>();
			container.RegisterDecorator<ICatalogService, TracingCatalogServiceDecorator>();

			// Act
			var result = container.GetInstance<ICatalogService>();

			// Assert
			Assert.IsType<TracingCatalogServiceDecorator>(result);
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
	}

	public class EndpointSettings
	{
		public EndpointSettings(string catalogServiceUrl, string arbitrationServiceUrl)
		{
			CatalogServiceUrl = catalogServiceUrl;
			ArbitrationServiceUrl = arbitrationServiceUrl;
		}

		public string CatalogServiceUrl { get; }
		public string ArbitrationServiceUrl { get; }
	}

	public interface ICatalogService
	{
	}

	[DebuggerDisplay("Call to {EndpointSettings.CatalogServiceUrl}")]
	public class CatalogService : ICatalogService
	{
		public EndpointSettings EndpointSettings { get; }

		public CatalogService(EndpointSettings endpointSettings)
		{
			EndpointSettings = endpointSettings;
		}
	}

	[DebuggerDisplay("Decorator of `{Original}`")]
	public class TracingCatalogServiceDecorator : ICatalogService
	{
		public ICatalogService Original { get; }

		public TracingCatalogServiceDecorator(ICatalogService original)
		{
			Original = original;
		}
	}

	public interface ISpecification
	{
		
	}

	public class Specification1 : ISpecification
	{
		
	}
	public class Specification2 : ISpecification
	{
		
	}

	public class CompositeSpecification : ISpecification
	{
		public IReadOnlyList<ISpecification> Specifications { get; }

		public CompositeSpecification(IReadOnlyList<ISpecification> specifications)
		{
			Specifications = specifications;
		}
	}
}