# EmployerFavouritesApi.Client.Api.ApprenticeshipsApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**Get**](ApprenticeshipsApi.md#get) | **GET** /api/Apprenticeships/{employerAccountId} | Gets all apprenticeship favourites for the Employer account Id provided


<a name="get"></a>
# **Get**
> List<ApprenticeshipFavourite> Get (string employerAccountId)

Gets all apprenticeship favourites for the Employer account Id provided

### Example
```csharp
using System;
using System.Diagnostics;
using EmployerFavouritesApi.Client.Api;
using EmployerFavouritesApi.Client.Client;
using EmployerFavouritesApi.Client.Model;

namespace Example
{
    public class GetExample
    {
        public void main()
        {
            var apiInstance = new ApprenticeshipsApi();
            var employerAccountId = employerAccountId_example;  // string | 

            try
            {
                // Gets all apprenticeship favourites for the Employer account Id provided
                List&lt;ApprenticeshipFavourite&gt; result = apiInstance.Get(employerAccountId);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ApprenticeshipsApi.Get: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **employerAccountId** | **string**|  | 

### Return type

[**List<ApprenticeshipFavourite>**](ApprenticeshipFavourite.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

