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
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using RestSharp;
using Xunit;

using EmployerFavouritesApi.Client.Client;
using EmployerFavouritesApi.Client.Api;
using EmployerFavouritesApi.Client.Model;

namespace EmployerFavouritesApi.Client.Test
{
    /// <summary>
    ///  Class for testing ApprenticeshipsApi
    /// </summary>
    /// <remarks>
    /// This file is automatically generated by OpenAPI Generator (https://openapi-generator.tech).
    /// Please update the test case below to test the API endpoint.
    /// </remarks>
    public class ApprenticeshipsApiTests : IDisposable
    {
        private ApprenticeshipsApi instance;

        public ApprenticeshipsApiTests()
        {
            instance = new ApprenticeshipsApi();
        }

        public void Dispose()
        {
            // Cleanup when everything is done.
        }

        /// <summary>
        /// Test an instance of ApprenticeshipsApi
        /// </summary>
        [Fact]
        public void InstanceTest()
        {
            // TODO uncomment below to test 'IsInstanceOfType' ApprenticeshipsApi
            //Assert.IsType(typeof(ApprenticeshipsApi), instance, "instance is a ApprenticeshipsApi");
        }

        
        /// <summary>
        /// Test Get
        /// </summary>
        [Fact]
        public void GetTest()
        {
            // TODO uncomment below to test the method and replace null with proper value
            //string employerAccountId = null;
            //var response = instance.Get(employerAccountId);
            //Assert.IsType<List<ApprenticeshipFavourite>> (response, "response is List<ApprenticeshipFavourite>");
        }
        
    }

}