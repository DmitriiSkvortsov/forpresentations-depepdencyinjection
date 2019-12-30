namespace Samples
{
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
		public ISpecification[] Specifications { get; }

		public CompositeSpecification(ISpecification[] specifications)
		{
			Specifications = specifications;
		}
	}
}