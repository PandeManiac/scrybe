// SPDX-License-Identifier: LGPL-3.0-only

namespace Scrybe.Project;

/// <summary>
/// Represents an authored narrative graph.
/// </summary>
public sealed class ScryGraph
{
	public Guid Id { get; }
	public string Name { get; }

	/// <summary>
	/// Initializes a graph with an explicit identity and name.
	/// </summary>
	/// <param name="id">The stable graph identifier.</param>
	/// <param name="name">The graph name.</param>
	/// <exception cref="ArgumentException">
	/// The identifier is empty or the name is blank.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	/// The name is null.
	/// </exception>
	public ScryGraph(Guid id, string name)
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException(
				"Graph identity cannot be empty.",
				nameof(id));
		}

		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		Id = id;
		Name = name;
	}
}
