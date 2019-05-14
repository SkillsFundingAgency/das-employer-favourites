/* 
 * Employer-Favourites-Api
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mime;
using EmployerFavouritesApi.Client.Client;
using EmployerFavouritesApi.Client.Model;

namespace EmployerFavouritesApi.Client.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IApprenticeshipsApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="EmployerFavouritesApi.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="employerAccountId"></param>
        /// <returns>List&lt;ApprenticeshipFavourite&gt;</returns>
        List<ApprenticeshipFavourite> Get (string employerAccountId);

        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="EmployerFavouritesApi.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="employerAccountId"></param>
        /// <returns>ApiResponse of List&lt;ApprenticeshipFavourite&gt;</returns>
        ApiResponse<List<ApprenticeshipFavourite>> GetWithHttpInfo (string employerAccountId);
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IApprenticeshipsApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="EmployerFavouritesApi.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="employerAccountId"></param>
        /// <returns>Task of List&lt;ApprenticeshipFavourite&gt;</returns>
        System.Threading.Tasks.Task<List<ApprenticeshipFavourite>> GetAsync (string employerAccountId);

        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="EmployerFavouritesApi.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="employerAccountId"></param>
        /// <returns>Task of ApiResponse (List&lt;ApprenticeshipFavourite&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<ApprenticeshipFavourite>>> GetAsyncWithHttpInfo (string employerAccountId);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IApprenticeshipsApi : IApprenticeshipsApiSync, IApprenticeshipsApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class ApprenticeshipsApi : IApprenticeshipsApi
    {
        private EmployerFavouritesApi.Client.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprenticeshipsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public ApprenticeshipsApi() : this((string) null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprenticeshipsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public ApprenticeshipsApi(String basePath)
        {
            this.Configuration = EmployerFavouritesApi.Client.Client.Configuration.MergeConfigurations(
                EmployerFavouritesApi.Client.Client.GlobalConfiguration.Instance,
                new EmployerFavouritesApi.Client.Client.Configuration { BasePath = basePath }
            );
            this.Client = new EmployerFavouritesApi.Client.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new EmployerFavouritesApi.Client.Client.ApiClient(this.Configuration.BasePath);
            this.ExceptionFactory = EmployerFavouritesApi.Client.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprenticeshipsApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public ApprenticeshipsApi(EmployerFavouritesApi.Client.Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Configuration = EmployerFavouritesApi.Client.Client.Configuration.MergeConfigurations(
                EmployerFavouritesApi.Client.Client.GlobalConfiguration.Instance,
                configuration
            );
            this.Client = new EmployerFavouritesApi.Client.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new EmployerFavouritesApi.Client.Client.ApiClient(this.Configuration.BasePath);
            ExceptionFactory = EmployerFavouritesApi.Client.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PetApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public ApprenticeshipsApi(EmployerFavouritesApi.Client.Client.ISynchronousClient client,EmployerFavouritesApi.Client.Client.IAsynchronousClient asyncClient, EmployerFavouritesApi.Client.Client.IReadableConfiguration configuration)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(asyncClient == null) throw new ArgumentNullException("asyncClient");
            if(configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = EmployerFavouritesApi.Client.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// The client for accessing this underlying API asynchronously.
        /// </summary>
        public EmployerFavouritesApi.Client.Client.IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public EmployerFavouritesApi.Client.Client.ISynchronousClient Client { get; set; }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public EmployerFavouritesApi.Client.Client.IReadableConfiguration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public EmployerFavouritesApi.Client.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided 
        /// </summary>
        /// <exception cref="EmployerFavouritesApi.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="employerAccountId"></param>
        /// <returns>List&lt;ApprenticeshipFavourite&gt;</returns>
        public List<ApprenticeshipFavourite> Get (string employerAccountId)
        {
             EmployerFavouritesApi.Client.Client.ApiResponse<List<ApprenticeshipFavourite>> localVarResponse = GetWithHttpInfo(employerAccountId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided 
        /// </summary>
        /// <exception cref="EmployerFavouritesApi.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="employerAccountId"></param>
        /// <returns>ApiResponse of List&lt;ApprenticeshipFavourite&gt;</returns>
        public EmployerFavouritesApi.Client.Client.ApiResponse< List<ApprenticeshipFavourite> > GetWithHttpInfo (string employerAccountId)
        {
            // verify the required parameter 'employerAccountId' is set
            if (employerAccountId == null)
                throw new EmployerFavouritesApi.Client.Client.ApiException(400, "Missing required parameter 'employerAccountId' when calling ApprenticeshipsApi->Get");

            EmployerFavouritesApi.Client.Client.RequestOptions requestOptions = new EmployerFavouritesApi.Client.Client.RequestOptions();

            String[] @contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] @accepts = new String[] {
                "application/json"
            };

            var localVarContentType = EmployerFavouritesApi.Client.Client.ClientUtils.SelectHeaderContentType(@contentTypes);
            if (localVarContentType != null) requestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = EmployerFavouritesApi.Client.Client.ClientUtils.SelectHeaderAccept(@accepts);
            if (localVarAccept != null) requestOptions.HeaderParameters.Add("Accept", localVarAccept);

            if (employerAccountId != null)
                requestOptions.PathParameters.Add("employerAccountId", EmployerFavouritesApi.Client.Client.ClientUtils.ParameterToString(employerAccountId)); // path parameter


            // make the HTTP request

            var response = this.Client.Get< List<ApprenticeshipFavourite> >("/api/Apprenticeships/{employerAccountId}", requestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("Get", response);
                if (exception != null) throw exception;
            }

            return response;
        }

        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided 
        /// </summary>
        /// <exception cref="EmployerFavouritesApi.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="employerAccountId"></param>
        /// <returns>Task of List&lt;ApprenticeshipFavourite&gt;</returns>
        public async System.Threading.Tasks.Task<List<ApprenticeshipFavourite>> GetAsync (string employerAccountId)
        {
             EmployerFavouritesApi.Client.Client.ApiResponse<List<ApprenticeshipFavourite>> localVarResponse = await GetAsyncWithHttpInfo(employerAccountId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided 
        /// </summary>
        /// <exception cref="EmployerFavouritesApi.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="employerAccountId"></param>
        /// <returns>Task of ApiResponse (List&lt;ApprenticeshipFavourite&gt;)</returns>
        public async System.Threading.Tasks.Task<EmployerFavouritesApi.Client.Client.ApiResponse<List<ApprenticeshipFavourite>>> GetAsyncWithHttpInfo (string employerAccountId)
        {
            // verify the required parameter 'employerAccountId' is set
            if (employerAccountId == null)
                throw new EmployerFavouritesApi.Client.Client.ApiException(400, "Missing required parameter 'employerAccountId' when calling ApprenticeshipsApi->Get");


            EmployerFavouritesApi.Client.Client.RequestOptions requestOptions = new EmployerFavouritesApi.Client.Client.RequestOptions();

            String[] @contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] @accepts = new String[] {
                "application/json"
            };
            
            foreach (var contentType in @contentTypes)
                requestOptions.HeaderParameters.Add("Content-Type", contentType);
            
            foreach (var accept in @accepts)
                requestOptions.HeaderParameters.Add("Accept", accept);
            
            if (employerAccountId != null)
                requestOptions.PathParameters.Add("employerAccountId", EmployerFavouritesApi.Client.Client.ClientUtils.ParameterToString(employerAccountId)); // path parameter


            // make the HTTP request

            var response = await this.AsynchronousClient.GetAsync<List<ApprenticeshipFavourite>>("/api/Apprenticeships/{employerAccountId}", requestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("Get", response);
                if (exception != null) throw exception;
            }

            return response;
        }

    }
}