using System.Diagnostics;

namespace Samples
{
	public class SomeServiceSettings
	{
		public SomeServiceSettings(string someServiceUrl)
		{
			SomeServiceUrl = someServiceUrl;
		}

		public string SomeServiceUrl { get; }
	}

	public interface ISomeService
	{
	}

	[DebuggerDisplay("Call to {SomeServiceSettings.SomeServiceUrl}")]
	public class SomeService : ISomeService
	{
		public SomeServiceSettings SomeServiceSettings { get; }

		public SomeService(SomeServiceSettings someServiceSettings)
		{
			SomeServiceSettings = someServiceSettings;
		}
	}

	[DebuggerDisplay("Decorator of `{Original}`")]
	public class TracingSomeServiceDecorator : ISomeService
	{
		public ISomeService Original { get; }

		public TracingSomeServiceDecorator(ISomeService original)
		{
			Original = original;
		}
	}
}