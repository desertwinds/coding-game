/// <summary>
/// Tuples v1.1.0 by Christian Chomiak, christianchomiak@gmail.com
/// 
/// Structure that holds a certain amount of elements.
/// Similar to a Vector2/3/4 but generic.
/// 
/// Generic tuples can be created using Tuple<>, Tuple3<> and Tuple4<>.
/// However, to be able to show its contents in the Unity Inspector, a class must
/// not be generic. So, custom tuples like Tuple3I are provided to work with common
/// types of tuples. They can be copied to created new custom tuples (e.g. GameObject & Transform).
/// 
/// Also notice that each element of a tuple can have a different type than the rest.
/// 
/// This class was created based on a research that included these sources:
///     * Source 1: http://stackoverflow.com/questions/7120845/equivalent-of-tuple-net-4-for-net-framework-3-5
///     * Source 2: https://gist.github.com/michaelbartnett/5652076
/// </summary>

using System.Collections.Generic;


#region Core

/// <summary>
/// Tuple class of 2 generic elements
/// </summary>
/// <typeparam name="T1">First element type</typeparam>
/// <typeparam name="T2">Second element type</typeparam> 
[System.Serializable]
public class Tuple<T1, T2>
{
	#region Variables

	public T1 first;
	public T2 second;

	private static readonly IEqualityComparer<T1> Item1Comparer = EqualityComparer<T1>.Default;
	private static readonly IEqualityComparer<T2> Item2Comparer = EqualityComparer<T2>.Default;

	#endregion

	#region Constructors

	public Tuple(T1 _first, T2 _second) //originally was _internal_
	{
		first = _first;
		second = _second;
	}

	#endregion

	#region Public Functions

	public override string ToString()
	{
		return string.Format("<{0}, {1}>", first, second);
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 23 + first.GetHashCode();
		hash = hash * 23 + second.GetHashCode();
		return hash;
	}


	public override bool Equals(object obj)
	{
		var other = obj as Tuple<T1, T2>;
		if (object.ReferenceEquals(other, null))
			return false;
		else
			return Item1Comparer.Equals(first, other.first) && Item2Comparer.Equals(second, other.second);
	}

	#endregion

	#region Private Functions

	private static bool IsNull(object obj)
	{
		return object.ReferenceEquals(obj, null);
	}

	#endregion

	#region Operators

	public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
	{
		if (Tuple<T1, T2>.IsNull(a) && !Tuple<T1, T2>.IsNull(b))
			return false;

		if (!Tuple<T1, T2>.IsNull(a) && Tuple<T1, T2>.IsNull(b))
			return false;

		if (Tuple<T1, T2>.IsNull(a) && Tuple<T1, T2>.IsNull(b))
			return true;

		return
			a.first.Equals(b.first) &&
			a.second.Equals(b.second);
	}

	public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
	{
		return !(a == b);
	}

	#endregion
}
#endregion

#region Custom Tuples

//Removed as it's basically the same as using Vector2
/// <summary>
/// Tuple class of 2 float
/// </summary>
/// <typeparam name="First">First float</typeparam>
/// <typeparam name="Second">Second float</typeparam>
/*[System.Serializable]
public class TupleF : Tuple<float, float>
{
    public static TupleF zero
    {
        get { return new TupleF(0f, 0f); }
    }
    public TupleF(float a, float b)
        : base(a, b)
    {
    }
    public static TupleF operator +(TupleF a, TupleF b)
    {
        return new TupleF(a.first + b.first, a.second + b.second);
    }
    public static TupleF operator -(TupleF a, TupleF b)
    {
        return new TupleF(a.first - b.first, a.second - b.second);
    }
    public static TupleF operator *(TupleF a, TupleF b)
    {
        return new TupleF(a.first * b.first, a.second * b.second);
    }
    public static TupleF operator /(TupleF a, TupleF b)
    {
        return new TupleF(a.first / b.first, a.second / b.second);
    }
    public static implicit operator UnityEngine.Vector2(TupleF t)
    {
        return new UnityEngine.Vector2(t.first, t.second);
    }
}*/

/// <summary>
/// Tuple class of 2 int
/// </summary>
/// <typeparam name="First">First int</typeparam>
/// <typeparam name="Second">Second int</typeparam>
[System.Serializable]
public class TupleI : Tuple<int, int>
{
	public static TupleI zero
	{
		get { return new TupleI(0, 0); }
	}

	public static TupleI one
	{
		get { return new TupleI(1, 1); }
	}

	public TupleI(int a, int b)
		: base(a, b)
	{
	}

	public static TupleI operator +(TupleI a, TupleI b)
	{
		return new TupleI(a.first + b.first, a.second + b.second);
	}

	public static TupleI operator -(TupleI a, TupleI b)
	{
		return new TupleI(a.first - b.first, a.second - b.second);
	}

	public static TupleI operator *(TupleI a, TupleI b)
	{
		return new TupleI(a.first * b.first, a.second * b.second);
	}

	public static TupleI operator /(TupleI a, TupleI b)
	{
		return new TupleI(a.first / b.first, a.second / b.second);
	}

	public static implicit operator UnityEngine.Vector2(TupleI t)
	{
		return new UnityEngine.Vector2(t.first, t.second);
	}

	public static implicit operator TupleI(UnityEngine.Vector2 v)
	{
		return new TupleI((int) v.x, (int) v.y);
	}
}
#endregion
