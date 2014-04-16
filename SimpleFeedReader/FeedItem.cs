﻿using System;
using System.ServiceModel.Syndication;

namespace SimpleFeedReader
{
    /// <summary>
    /// Represents an item from a <see cref="SyndicationFeed"/>.
    /// </summary>
    public class FeedItem
    {
        /// <summary>
        /// The Title of the <see cref="FeedItem"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The Content of the <see cref="FeedItem"/>.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The Summary of the <see cref="FeedItem"/>.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The Uri of the <see cref="FeedItem"/>.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The Date of the <see cref="FeedItem"/>.
        /// </summary>
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Initializes a new <see cref="FeedItem"/>.
        /// </summary>
        public FeedItem() { }

        /// <summary>
        /// Initializes a new <see cref="FeedItem"/> by copying the passed item's properties into the new instance.
        /// </summary>
        /// <param name="item">The <see cref="FeedItem"/> to copy.</param>
        /// <remarks>This is a copy-constructor.</remarks>
        public FeedItem(FeedItem item)
        {
            this.Title = item.Title;
            this.Content = item.Content;
            this.Summary = item.Summary;
            this.Uri = item.Uri;
            this.Date = item.Date;
        }

        /// <summary>
        /// Returns content, if any, otherwise returns the summary as content.
        /// </summary>
        /// <returns>Returns content, if any, otherwise returns the summary as content.</returns>
        /// <remarks>This method is intended as conveinience-method.</remarks>
        public string GetContent()
        {
            return !string.IsNullOrEmpty(this.Content) ? this.Content : this.Summary;
        }

        /// <summary>
        /// Returns the summary, if any, otherwise returns the content as the summary.
        /// </summary>
        /// <returns>Returns the summary, if any, otherwise returns the content as the summary.</returns>
        /// <remarks>This method is intended as conveinience-method.</remarks>
        public string GetSummary()
        {
            return !string.IsNullOrEmpty(this.Summary) ? this.Summary : this.Content;
        }
    }
}