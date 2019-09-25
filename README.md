# das-employer-favourites

## Build Status

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/Future%20Engagement/das-employer-favourites?branchName=master)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=1511&branchName=master)

## Requirements

DotNet Core 2.2 and any supported IDE for DEV running.

## About

The employer-favourites website is responsible for viewing the apprenticeship programmes and associated training providers that the Employer users have chosen to shortlist elsewhere in the service (Initially from Campaigns site).

The employer-favourites api gives programatic is both consumed by the website but also gives other parts of the service programmatic access to save/retrieve the employer favourite informtion. 

## Local running

You are able to run the website by doing the following:

### Clone and deploy

- Clone repository
- Create table in Azure Storage called Configuration using https://github.com/SkillsFundingAgency/das-employer-config-updater
- By default the configuration of the website is setup to use the Api hosted in the AT environment. Update the table storage config for the website to change this if required.
- Run the website project (Start EmployerFavouritesWeb(Kestral) project as opposed to IISExpress).
- Navigate to https://localhost:5040/account/HASHED-ACCOUNT-ID/apprenticeships where HASHED-ACCOUNT-ID is substited for the one you see when on the Dashboard of the AT Employer site. (Note credentials will be for the AT version of Employer Idams)

## API Authorization

The API uses AzureAD for authentication. When running in Development mode, the authentication filteris not added so no auth is required. If you do enable authentication you will need to add the ```Authorization Bearer [TOKEN]``` header attribute to all requests. 

