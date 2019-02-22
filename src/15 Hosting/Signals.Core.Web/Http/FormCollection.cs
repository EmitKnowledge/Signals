﻿using Newtonsoft.Json;
using Signals.Core.Common.Instance;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Signals.Core.Common.Serialization;
using Signals.Core.Processing.Input.Http;
using IFormCollection = Signals.Core.Processing.Input.Http.IFormCollection;

namespace Signals.Core.Web.Http
{
    /// <summary>
    /// Wrapper around real cookie collection
    /// </summary>
    public class FormCollection : Dictionary<string, object>, Signals.Core.Processing.Input.Http.IFormCollection
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Return all of the available keys</summary>
        public new ICollection<string> Keys => base.Keys;

#if (NET461)

        /// <summary>
        /// Real http context
        /// </summary>
        private System.Web.HttpContext _context;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="context"></param>
        public FormCollection(System.Web.HttpContext context)
        {
            _context = context;
            
            foreach (var key in _context.Request.Form.AllKeys)
            {
                var value = _context.Request.Form[key];
                this.Add(key, value);
            }
        }

#else
        /// <summary>
        /// Real http context
        /// </summary>
        private Microsoft.AspNetCore.Http.HttpContext _context;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="context"></param>
        public FormCollection(Microsoft.AspNetCore.Http.HttpContext context)
        {
            _context = context;
            
            foreach (var key in _context.Request.Form.Keys)
            {
                var value = _context.Request.Form[key];
                this.Add(key, value);
            }
        }

        #endif
    }
}
