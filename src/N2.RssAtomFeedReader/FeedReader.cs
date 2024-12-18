﻿using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace N2.RssAtomFeedReader
{
    /// <summary>
    /// Retrieves <see cref="SyndicationFeed"/>s and normalizes the items from the feed into <see cref="FeedItem"/>s.
    /// </summary>
    public class FeedReader : IFeedReader
    {
        /// <summary>
        /// The default feeds for the <see cref="FeedReader"/>.
        /// </summary>
        private readonly Dictionary<string, string> feeds = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        public FeedReader()
            : this(new DefaultFeedItemNormalizer()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        /// <param name="options">Initialization options.</param>
        public FeedReader(FeedReaderOptions options)
        {
            DefaultNormalizer = options.DefaultNormalizer;
            ThrowOnError = options.ThrowOnError;
            feeds.Clear();
            foreach (var feed in options.Feeds)
            {
                feeds.Add(feed.Key, feed.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        /// <param name="throwOnError">
        /// When true, the <see cref="FeedReader"/> will throw on errors, when false the <see cref="FeedReader"/> will
        /// suppress exceptions and return empty results.
        /// </param>
        public FeedReader(bool throwOnError)
            : this(new DefaultFeedItemNormalizer(), throwOnError) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        /// <param name="defaultFeedItemNormalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="SyndicationItem"/>s.
        /// </param>
        public FeedReader(IFeedItemNormalizer defaultFeedItemNormalizer)
            : this(defaultFeedItemNormalizer, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        /// <param name="defaultFeedItemNormalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="SyndicationItem"/>s.
        /// </param>
        /// <param name="throwOnError">
        /// When true, the <see cref="FeedReader"/> will throw on errors, when false the <see cref="FeedReader"/> will
        /// suppress exceptions and return empty results.
        /// </param>
        public FeedReader(IFeedItemNormalizer defaultFeedItemNormalizer, bool throwOnError)
        {
            DefaultNormalizer = defaultFeedItemNormalizer ?? throw new ArgumentNullException(nameof(defaultFeedItemNormalizer));
            ThrowOnError = throwOnError;
        }

        /// <summary>
        /// Gets the default FeedItemNormalizer the <see cref="FeedReader"/> will use when normalizing
        /// <see cref="SyndicationItem"/>s.
        /// </summary>
        public IFeedItemNormalizer DefaultNormalizer { get; private set; }

        /// <summary>
        /// Gets wether the FeedReader will throw on exceptions or suppress exceptions and return empty results on
        /// errors.
        /// </summary>
        public bool ThrowOnError { get; private set; }

        /// <summary>
        /// Retrieves the specified feed.
        /// </summary>
        /// <param name="uri">The uri of the feed to retrieve.</param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeed(string uri)
        {
            if (feeds.TryGetValue(uri, out var value))
            {
                return RetrieveFeed(value, DefaultNormalizer);
            }
            return RetrieveFeed(uri, DefaultNormalizer);
        }

        /// <summary>
        /// Retrieves the specified feed.
        /// </summary>
        /// <param name="uri">The uri of the feed to retrieve.</param>
        /// <param name="normalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeed(string uri, IFeedItemNormalizer normalizer)
        {
            try
            {
                using var reader = XmlReader.Create(uri);
                return RetrieveFeed(reader, normalizer);
            }
            catch
            {
                if (ThrowOnError)
                    throw;
            }
            return [];
        }

        /// <summary>
        /// Retrieves the specified feed.
        /// </summary>
        /// <param name="xmlReader">The <see cref="XmlReader"/> to use to read the items from.</param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeed(XmlReader xmlReader)
        {
            return RetrieveFeed(xmlReader, DefaultNormalizer);
        }

        /// <summary>
        /// Retrieves the specified feed.
        /// </summary>
        /// <param name="xmlReader">The <see cref="XmlReader"/> to use to read the items from.</param>
        /// <param name="normalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        public IEnumerable<FeedItem> RetrieveFeed(XmlReader xmlReader, IFeedItemNormalizer normalizer)
        {
            if (xmlReader == null)
            {
                throw new ArgumentNullException(nameof(xmlReader));
            }

            if (normalizer == null)
            {
                throw new ArgumentNullException(nameof(normalizer));
            }

            var items = new List<FeedItem>();
            try
            {
                var feed = SyndicationFeed.Load(xmlReader);
                foreach (var item in feed.Items)
                    items.Add(normalizer.Normalize(feed, item));
            }
            catch
            {
                if (ThrowOnError)
                    throw;
            }
            return items;
        }

        /// <summary>
        /// Retrieves the specified feeds.
        /// </summary>
        /// <param name="uris">The uri's of the feeds to retrieve.</param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        /// <remarks>This is a convenience method.</remarks>
        public IEnumerable<FeedItem> RetrieveFeeds(IEnumerable<string> uris)
        {
            return RetrieveFeeds(uris, DefaultNormalizer);
        }

        /// <summary>
        /// Retrieves the specified feeds.
        /// </summary>
        /// <param name="uris">The uri's of the feeds to retrieve.</param>
        /// <param name="normalizer">
        /// The <see cref="IFeedItemNormalizer"/> to use when normalizing <see cref="FeedItem"/>s.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;FeedItem&gt;"/> of retrieved <see cref="FeedItem"/>s.
        /// </returns>
        /// <remarks>This is a convenience method.</remarks>
        public IEnumerable<FeedItem> RetrieveFeeds(IEnumerable<string> uris, IFeedItemNormalizer normalizer)
        {
            List<FeedItem> items = [];
            foreach (var u in uris)
            {
                items.AddRange(RetrieveFeed(u, normalizer));
            }
            return items;
        }
    }
}