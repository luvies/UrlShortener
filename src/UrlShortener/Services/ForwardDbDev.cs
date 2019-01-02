using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortener.Models;

namespace UrlShortener.Services
{
    public class ForwardDbDev : IForwardDb
    {
        private static List<ForwardItem> forwardItems =
        new List<ForwardItem> {
            new ForwardItem("fwd1", "https://google.com"),
            new ForwardItem("fwd2", "https://example.com"),
            new ForwardItem("fwd3", "https://bing.com", "Bad search engine"),
            new ForwardItem("fwd4", "https://youtube.com", hits: 12),
            new ForwardItem("fwd5", "https://devsprime.com", "Good homepage", 123),
            new ForwardItem("fwd6", "https://luvies.io", "Fancy", 321),
            new ForwardItem("fwd7", "https://twitter.com", hits: 987),
            new ForwardItem("fwd8", "https://tumblr.com", "Not very good anymore")
        };

        public Task<IEnumerable<ForwardItem>> ListAllForwards()
        {
            return Task.FromResult<IEnumerable<ForwardItem>>(forwardItems);
        }

        public Task<ForwardItem> GetForward(string id)
        {
            ForwardItem item = forwardItems.FirstOrDefault(f => f.Id == id);

            // If not found, throw a key error.
            if (item == null)
            {
                throw new KeyNotFoundException("A forward with that ID was not found");
            }

            return Task.FromResult(item);
        }

        public async Task<string> ProcessForward(string id)
        {
            ForwardItem item = await GetForward(id);

            // If found, inc the hit counter & return the destination.
            item.Hits++;
            return item.Dest;
        }

        public Task AddForward(ForwardItem forward)
        {
            forward.Validate();

            // Ensure no duplicate ID.
            if (forwardItems.Any(f => f.Id == forward.Id))
            {
                throw new InvalidOperationException("Cannot add a forward with a duplicate ID");
            }

            // Add forward to list.
            forward.Notes = forward.Notes ?? "";
            forward.Hits = 0;
            forwardItems.Add(forward);
            return Task.CompletedTask;
        }

        public async Task UpdateForward(string id, ForwardItemUpdate forward)
        {
            forward.Validate();

            ForwardItem item = await GetForward(id);

            // Update forward, preserving existing hit counter.
            item.Dest = forward.Dest;
            item.Notes = forward.Notes;
        }
    }
}
