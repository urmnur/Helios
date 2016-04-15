﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;

namespace Helios.Logging
{
    /// <summary>
    /// Factory class for acquiring <see cref="ILogger"/> instances.
    /// </summary>
    public abstract class LoggingFactory
    {
        private static LoggingFactory _defaultFactory;

        static LoggingFactory NewDefaultFactory(string name)
        {
            var loggingFactory = new StandardOutLoggerFactory();
            loggingFactory.NewInstance(name, typeof(LoggingFactory)).Debug("Using Standard Out as the default logging system.");
            return loggingFactory;
        }

        /// <summary>
        /// Gets or sets the default <see cref="LoggingFactory"/> used by Helios. Defaults to <see cref="StandardOutLoggerFactory"/>.
        /// </summary>
        public static LoggingFactory DefaultFactory
        {
            get
            {
                var factory = Volatile.Read(ref _defaultFactory);
                if (factory == null)
                {
                    factory = NewDefaultFactory(typeof (LoggingFactory).FullName);
                    var current = Interlocked.CompareExchange(ref _defaultFactory, factory, null);
                    if (current != null)
                        return current;
                }
                return factory;
            }
            set
            {
                Contract.Requires(value != null);
                Volatile.Write(ref _defaultFactory, value);
            }
        }

        protected abstract ILogger NewInstance(string name, Type source);

        public static ILogger GetLogger<T>()
        {
            return GetInstance(typeof (T));
        }

        public static ILogger GetInstance(Type t)
        {
            return GetInstance(t.FullName, t);
        }

        public static ILogger GetInstance(string name, Type t)
        {
            return DefaultFactory.NewInstance(name, t);
        }
    }
}
