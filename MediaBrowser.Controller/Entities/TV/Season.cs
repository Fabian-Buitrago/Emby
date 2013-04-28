﻿using System.Collections.Generic;
using System.IO;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.IO;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Localization;
using MediaBrowser.Model.Entities;
using System;
using System.Runtime.Serialization;

namespace MediaBrowser.Controller.Entities.TV
{
    /// <summary>
    /// Class Season
    /// </summary>
    public class Season : Folder
    {

        /// <summary>
        /// Seasons are just containers
        /// </summary>
        /// <value><c>true</c> if [include in index]; otherwise, <c>false</c>.</value>
        [IgnoreDataMember]
        public override bool IncludeInIndex
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// We want to group into our Series
        /// </summary>
        /// <value><c>true</c> if [group in index]; otherwise, <c>false</c>.</value>
        [IgnoreDataMember]
        public override bool GroupInIndex
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Override this to return the folder that should be used to construct a container
        /// for this item in an index.  GroupInIndex should be true as well.
        /// </summary>
        /// <value>The index container.</value>
        [IgnoreDataMember]
        public override Folder IndexContainer
        {
            get
            {
                return Series;
            }
        }

        // Genre, Rating and Stuido will all be the same
        protected override Dictionary<string, Func<User, IEnumerable<BaseItem>>> GetIndexByOptions()
        {
            return new Dictionary<string, Func<User, IEnumerable<BaseItem>>> {            
                {LocalizedStrings.Instance.GetString("NoneDispPref"), null}, 
                {LocalizedStrings.Instance.GetString("PerformerDispPref"), GetIndexByPerformer},
                {LocalizedStrings.Instance.GetString("DirectorDispPref"), GetIndexByDirector},
                {LocalizedStrings.Instance.GetString("YearDispPref"), GetIndexByYear},
            };
        }

        /// <summary>
        /// Gets the user data key.
        /// </summary>
        /// <returns>System.String.</returns>
        public override string GetUserDataKey()
        {
            if (Series != null)
            {
                var seasonNo = IndexNumber ?? 0;
                return Series.GetUserDataKey() + seasonNo.ToString("000");
            }

            return base.GetUserDataKey();
        }

        /// <summary>
        /// We persist the MB Id of our series object so we can always find it no matter
        /// what context we happen to be loaded from.
        /// </summary>
        /// <value>The series item id.</value>
        public Guid SeriesItemId { get; set; }

        /// <summary>
        /// The _series
        /// </summary>
        private Series _series;
        /// <summary>
        /// This Episode's Series Instance
        /// </summary>
        /// <value>The series.</value>
        [IgnoreDataMember]
        public Series Series
        {
            get { return _series ?? (_series = FindParent<Series>()); }
        }

        /// <summary>
        /// Our rating comes from our series
        /// </summary>
        public override string OfficialRating
        {
            get { return Series != null ? Series.OfficialRating : base.OfficialRating; }
            set
            {
                base.OfficialRating = value;
            }
        }

        /// <summary>
        /// Our rating comes from our series
        /// </summary>
        public override string CustomRating
        {
            get { return Series != null ? Series.CustomRating : base.CustomRating; }
            set
            {
                base.CustomRating = value;
            }
        }

        /// <summary>
        /// Add files from the metadata folder to ResolveArgs
        /// </summary>
        /// <param name="args">The args.</param>
        public static void AddMetadataFiles(ItemResolveArgs args)
        {
            var folder = args.GetFileSystemEntryByName("metadata");

            if (folder != null)
            {
                args.AddMetadataFiles(new DirectoryInfo(folder.FullName).EnumerateFiles());
            }
        }

        /// <summary>
        /// Creates ResolveArgs on demand
        /// </summary>
        /// <param name="pathInfo">The path info.</param>
        /// <returns>ItemResolveArgs.</returns>
        protected internal override ItemResolveArgs CreateResolveArgs(FileSystemInfo pathInfo = null)
        {
            var args = base.CreateResolveArgs(pathInfo);

            AddMetadataFiles(args);

            return args;
        }

        /// <summary>
        /// Creates the name of the sort.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string CreateSortName()
        {
            return IndexNumber != null ? IndexNumber.Value.ToString("0000") : Name;
        }
    }
}
