using System.Runtime.CompilerServices;

namespace FrozenCollections;

/// <summary>
/// Utilities to manipulate byref values.
/// </summary>
internal static class ByReference
{
    /// <summary>
    /// Returns a reference to a value of type <typeparamref name="T"/> that is a null reference.
    /// </summary>
    /// <typeparam name="T">The type of the reference.</typeparam>
    /// <returns>A reference to a value of type <typeparamref name="T"/> that is a null reference.</returns>
    public static ref T Null<T>()
    {
#if NET5_0_OR_GREATER
        return ref Unsafe.NullRef<T>();
#else
#pragma warning disable S2333 // Redundant modifiers should not be used
        unsafe
        {
            return ref Unsafe.AsRef<T>(null);
        }
#pragma warning restore S2333 // Redundant modifiers should not be used
#endif
    }

    /// <summary>
    /// Determines if a given reference to a value of type <typeparamref name="T"/> is a null reference.
    /// </summary>
    /// <typeparam name="T">The type of the reference.</typeparam>
    /// <param name="source">The reference to check.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> is a null reference; otherwise, <see langword="false"/>.</returns>
    public static bool IsNull<T>(in T source)
    {
#if NET5_0_OR_GREATER
        return Unsafe.IsNullRef(ref Unsafe.AsRef(source));
#else
        unsafe
        {
            return Unsafe.AsPointer(ref Unsafe.AsRef(source)) == null;
        }
#endif
    }

    /// <summary>
    /// Determines whether the specified references point to the same location.
    /// </summary>
    /// <typeparam name="T">The type of reference.</typeparam>
    /// <param name="left">The first reference to compare.</param>
    /// <param name="right">The second reference to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> point to the same location; otherwise, <see langword="false"/>.</returns>
    public static bool AreSame<T>(in T left, in T right) => Unsafe.AreSame(ref Unsafe.AsRef(left), ref Unsafe.AsRef(right));
}
