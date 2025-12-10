namespace ComputorV2.Core.Types
{
	public interface IMatrix : IComparable<IMatrix>, IEquatable<IMatrix>
	{
		public int Rows { get; }
		public int Columns { get; }
		public bool IsSquare { get; }
		public MathValue this[int row, int col] { get; set; }
	}
}