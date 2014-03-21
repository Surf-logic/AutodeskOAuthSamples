////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Daniel Du 2014 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////////////////

This sample illustrates how to connect to Autodesk OAuth webservice 
from a ASP.NET MVC application.

How to run this sample: 

1. Open OAuthAspnetMVC.sln in Visual Studio 2012
2. Replace the necessary place holders in code
3. Menu "Build" - "Clean Solution", then "Build" - "Rebuild Solution"
4. Run the sample, click "Start Autodesk Authenticiation " or "login with autodesk ID" on the index page.

In the samples, you will need to replace the following place holders with 
appropriate values and credentials:

        private const string m_ConsumerKey = "your.consumer.key.here";
        private const string m_ConsumerSecret = "your.comsumer.secret.key.here";
        private const string m_baseURL = "https://your.oauth.server.here";


The controller you want to look into is AuthenticateController.cs

Here some actions: 

public RedirectResult Start()  --- http://hostname:port/authenticate/start
-----------------------------
Start the autodesk OAuth process

If everything goes fine, you will be redirected to the Autodesk login page, login with your 
valid AutodeskID


public RedirectToRouteResult AuthCallback() --http://hostname:port/authenticate/AuthCallback
-----------------------------------
This is a callback action. if user input his correct Autodesk Id and password, it will be 
redirect to this page to get the verifier, and then get the access token


public ActionResult AuthenticationSucceed(OAuthResult result)
---------------------------------
Once the access token is retrieved, the OAuth process is completed, print the access token for demo, of cause now you can go ahead to get the protected resources with this access token





This sample uses the RestSharp API in order to perform REST requests and OAuth authentication. You will find information on the the RestSharp API here: http://restsharp.org/.
It's available here for download: https://github.com/restsharp/RestSharp. 
If you are using Visual Studio, you can use the NuGet Package Manager to 
download and install the latest version of the RestSharp API into your 
solution: http://www.nuget.org/




