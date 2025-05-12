#nullable enable

using System;

namespace SolidSilnique.Core.Diagnostics;

/// <summary>
/// Exception for indicating that the target is not set (in other words: is equal to <c>null</c>).
/// </summary>
/// <param name="message">Message that will be displayed when the exception is thrown but not caught.</param>
public class TargetNotSetException(string? message) : Exception(message);