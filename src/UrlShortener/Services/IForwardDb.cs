using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortener.Models;

namespace UrlShortener.Services
{
    public interface IForwardDb
    {
        /// <summary>
        /// Lists all forwards.
        /// This is *not* paginated.
        /// </summary>
        /// <returns>All forwards in the database.</returns>
        Task<IEnumerable<ForwardItem>> ListAllForwards();

        /// <summary>
        /// Gets the forward with the given ID.
        /// Throws <see cref="KeyNotFoundException"/> if ID wasn't found.
        /// </summary>
        /// <returns>The forward item.</returns>
        /// <param name="id">The forward ID.</param>
        Task<ForwardItem> GetForward(string id);

        /// <summary>
        /// Processes the forward with the given ID.
        /// Adds to the hit counter &amp; returns the destination.
        /// Throws <see cref="KeyNotFoundException"/> if ID wasn't found.
        /// </summary>
        /// <returns>The destination string.</returns>
        /// <param name="id">Forward ID.</param>
        Task<string> ProcessForward(string id);

        /// <summary>
        /// Adds a new forward to the database.
        /// Throws <see cref="InvalidOperationException"/> if ID already exists.
        /// </summary>
        /// <param name="forward">The forward item to add.</param>
        Task AddForward(ForwardItem forward);

        /// <summary>
        /// Updates the forward with the new data. Hits are ignored in the
        /// passed object.
        /// Throws <see cref="KeyNotFoundException"/> if ID wasn't found.
        /// </summary>
        /// <param name="forward">The forward item to update.</param>
        Task UpdateForward(string id, ForwardItemUpdate forward);
    }
}
