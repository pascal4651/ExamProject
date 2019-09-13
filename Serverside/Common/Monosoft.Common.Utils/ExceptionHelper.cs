// <copyright file="ExceptionHelper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.Utils
{
    using System;
    using System.Text;

    /// <summary>
    /// Exception helper, to log stacktrace and exception-stack
    /// </summary>
    public class ExceptionHelper
    {
        /// <summary>
        /// Gets exception as string
        /// </summary>
        /// <param name="ex">Exception to tranform into text</param>
        /// <returns>exception string</returns>
        public static string GetExceptionAsReportText(Exception ex)
        {
            StringBuilder exception = new StringBuilder();
            StringBuilder trace = new StringBuilder();
            Exception e = ex;
            while (e != null)
            {
                exception.AppendLine(e.Message);
                trace.AppendLine(e.StackTrace);
                e = e.InnerException;
            }

            string reporttxt = string.Format("Exception: {0} \r\n Trace: {1}", exception, trace);
            return reporttxt;
        }
    }
}