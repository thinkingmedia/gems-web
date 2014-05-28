﻿using System;
using System.Drawing;
using System.Net;
using System.Threading;
using GemsWeb.Interfaces;

namespace GemsWeb.Requesters
{
    /// <summary>
    /// Handles multiple attempts at downloading web resource.
    /// </summary>
    public class GetRetry : iRequest
    {
        /// <summary>
        /// Max number of retries.
        /// </summary>
        private readonly int _maxRetries;

        /// <summary>
        /// The object that will perform the request.
        /// </summary>
        private readonly iRequest _childRequester;

        /// <summary>
        /// The delay between retries (min is 1 second).
        /// </summary>
        private readonly TimeSpan _retryDelay;

        /// <summary>
        /// The URL to the image to download.
        /// </summary>
        public GetRetry(int pMaxRetries, TimeSpan pRetryDelay, iRequest pChildRequester)
        {
            _childRequester = pChildRequester;
            _maxRetries = Math.Min(5, Math.Max(1, pMaxRetries));
            _retryDelay = (pRetryDelay.TotalSeconds < 1) ? new TimeSpan(0, 0, 1) : pRetryDelay;
        }

        /// <summary>
        /// Attempts to download the image.
        /// </summary>
        /// <param name="pUrl">The URL to get.</param>
        public iResponse Get(Uri pUrl)
        {
            int i = 0;
            bool retry = true;

            // TODO: Implement a domain specific delay
            Thread.Sleep(1000);

            do
            {
                iResponse resp = _childRequester.Get(pUrl);
                switch (resp.getStatus())
                {
                    case WebExceptionStatus.Success:
                        if ((resp.getStatusCode() == HttpStatusCode.OK ||
                             resp.getStatusCode() == HttpStatusCode.Moved ||
                             resp.getStatusCode() == HttpStatusCode.Redirect) &&
                            resp.getContentType().StartsWith("image", StringComparison.OrdinalIgnoreCase))
                        {
                            Bitmap bit = resp.ReadBitmap();
                            if (bit != null)
                            {
                                return resp;
                            }
                        }
                        retry = false;
                        break;
                    case WebExceptionStatus.ConnectFailure:
                    case WebExceptionStatus.ReceiveFailure:
                    case WebExceptionStatus.RequestCanceled:
                    case WebExceptionStatus.SendFailure:
                    case WebExceptionStatus.Timeout:
                        Thread.Sleep(_retryDelay);
                        break;
                    default:
                        retry = false;
                        break;
                }

                i++;
            } while (retry && i < _maxRetries);

            return null;
        }
    }
}