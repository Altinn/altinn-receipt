using System;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Receipt.Services.Interfaces
{
    /// <summary>
    /// Interface for the Altinn Platform Storage services
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Gets an instances based onthe properties of the instanceId
        /// </summary>
        /// <param name="instanceOwnerId">The instance owner id</param>
        /// <param name="instanceGuid">Unique id to identify the instance</param>
        /// <returns></returns>
        public Task<Instance> GetInstance(int instanceOwnerId, Guid instanceGuid);

        /// <summary>
        /// Gets the application metadata for an app
        /// </summary>
        /// <param name="org">The short name of the app owner</param>
        /// <param name="app">The name of the app</param>
        /// <returns>The application metadata</returns>
        public Task<Application> GetApplication(string org, string app);

        /// <summary>
        /// Gets the text resources of an app for a given language
        /// </summary>
        /// <param name="org">The short name of the app owner</param>
        /// <param name="app">The name of the app</param>
        /// <param name="language">The two letter language code</param>
        /// <returns>The text resources, or null if the app has no texts for the language</returns>
        public Task<TextResource> GetTextResource(string org, string app, string language);
    }
}
