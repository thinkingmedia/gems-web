﻿using System;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using GemsWeb.Responses;

namespace GemsWeb.Readers
{
    /// <summary>
    /// Handles reading bitmaps from the input stream.
    /// </summary>
    public class BitmapReader : iStreamReader
    {
        /// <summary>
        /// Creates responses.
        /// </summary>
        private readonly iResponseFactory _responseFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        public BitmapReader(iResponseFactory pResponseFactory)
        {
            if (pResponseFactory == null)
            {
                throw new NullReferenceException("Argument is null.");
            }
            _responseFactory = pResponseFactory;
        }

        /// <summary>
        /// Creates a bitmap from the input stream.
        /// </summary>
        public iResponse Read(ContentType pContentType, Stream pStream)
        {
            using (Image img = Image.FromStream(pStream))
            {
                return _responseFactory.Create(pContentType, new Bitmap(img));
            }
        }
    }
}