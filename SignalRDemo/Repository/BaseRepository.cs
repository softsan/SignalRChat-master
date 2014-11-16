using SignalRDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRDemo.Repository
{
    public abstract class BaseRepository : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository" /> class.
        /// </summary>
        public BaseRepository()
        {
            this.DataContext = new ChatAppContext();
        }

        /// <summary>
        /// Gets the entity data context.
        /// </summary>
        protected ChatAppContext DataContext { get; private set; }

        /// <summary>
        /// Dispose the data context.
        /// </summary>
        public void Dispose()
        {
            this.DataContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}