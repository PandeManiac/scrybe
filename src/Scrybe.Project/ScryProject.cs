// SPDX-License-Identifier: LGPL-3.0-only

namespace Scrybe.Project;

/// <summary>
/// Represents an authored SCRYBE project.
/// </summary>
public sealed class ScryProject
{
    public Guid Id { get; }
    public string Title { get; }

    /// <summary>
    /// Initializes a project with an explicit identity and title.
    /// </summary>
    /// <param name="id">The stable project identifier.</param>
    /// <param name="title">The project title.</param>
    /// <exception cref="ArgumentException">
    /// The identifier is empty or the title is blank.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// The title is null.
    /// </exception>
    public ScryProject(Guid id, string title)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Project identity cannot be empty.",
                nameof(id));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Id = id;
        Title = title;
    }
}
