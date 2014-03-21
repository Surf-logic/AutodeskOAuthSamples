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


using OAuthAspnetMVC.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OAuthAspnetMVC.Controllers
{
    public class AuthenticateController : Controller
    {

        // Hard coded consumer and secret keys and base URL.
        // In real world Apps, these values need to secured and
        // preferably not hardcoded.
        private const string m_ConsumerKey = "your consumer key";
        private const string m_ConsumerSecret = "your secret key";
        private const string m_OAuthBaseURL = "https://your.oauth.server.com";

        private static RestClient m_Client;
        private static string m_oAuthReqToken;
        private static string m_oAuthReqTokenSecret;

        private  string m_oAuthAccessToken;
        private  string m_oAuthAccessTokenSecret;
        private  string m_sessionHandle;



        //
        // GET: /Authenticate/

        public ActionResult Index()
        {
            return View();
        }

        public RedirectResult Start()
        {


            // Instantiate the RestSharp library object RestClient. RestSharp is free and makes it
            // very easy to build apps that use the OAuth and OpenID protocols with a provider supporting
            // these protocols
            m_Client = new RestClient(m_OAuthBaseURL);

            // for in-band, you can specify a callback url,  which will be redirect to 
            // after the user is authenticated 
            // the callback url parameter can be ommited if you do not want to use callback 
            string scheme = this.Url.RequestContext.HttpContext.Request.Url.Scheme;
            string callbackUrl = this.Url.Action("AuthCallback", "Authenticate",null, scheme);

            m_Client.Authenticator = OAuth1Authenticator.ForRequestToken(
            m_ConsumerKey, m_ConsumerSecret, callbackUrl);


            // Build the HTTP request for a Request token and execute it against the OAuth provider
            var request = new RestRequest("OAuth/RequestToken", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var response = m_Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                m_Client = null;
                //NetLog.WriteTextLog("</br>Oops! Something went wrong - couldn't request token Autodesk oxygen provider" + "</br>" + "-----------------------" + "</br>");

                string errorUrl = this.Url.Action("AuthenticationFailed", "Authenticate", new { reason = "Couldn't request token Autodesk oxygen provider" },this.Request.Url.Scheme);
                return new RedirectResult(errorUrl);
            }
            else
            {
                //get the request token successfully
                //Get the request token and associated parameters.
                var qs = HttpUtility.ParseQueryString(response.Content);
                m_oAuthReqToken = qs["oauth_token"];
                m_oAuthReqTokenSecret = qs["oauth_token_secret"];


                //build URL for Authorization HTTP request
                RestRequest authorizeRequest = new RestRequest
                {
                    Resource = "OAuth/Authorize",
                    Method = Method.GET  //must be GET, POST will cause "500 - Internal server error."
                };

                authorizeRequest.AddParameter("viewmode", "full");
                authorizeRequest.AddParameter("oauth_token", m_oAuthReqToken);
                authorizeRequest.AddHeader("Content-Type", "application/json");

                Uri authorizeUri = m_Client.BuildUri(authorizeRequest);
                var url = authorizeUri.ToString();

                // navigate to the Authorization URL

                return new RedirectResult(url);
            }

        }

        public RedirectToRouteResult AuthCallback()
        {

            string qry = System.Web.HttpContext.Current.Request.Url.Query;
            var qstr = HttpUtility.ParseQueryString(qry);
 
            string verifier = qstr["oauth_verifier"];

            if (null == verifier || verifier.Length == 0)
            {
                //NetLog.WriteTextLog("</br>Invalid PIN." + "</br>" + "-----------------------" + "</br>");
                return RedirectToAction("AuthenticationFailed", new { reason = "Invalid PIN." });
            }

            // Build the HTTP request for an access token
            var request = new RestRequest("OAuth/AccessToken", Method.POST);

            m_Client.Authenticator = OAuth1Authenticator.ForAccessToken(
               m_ConsumerKey, m_ConsumerSecret, m_oAuthReqToken, m_oAuthReqTokenSecret,
               verifier);

            // Execute the access token request
            var response = m_Client.Execute(request);

            // The request for access token is successful. Parse the response and store token,token secret and session handle
            var qs = HttpUtility.ParseQueryString(response.Content);
            m_oAuthAccessToken = qs["oauth_token"];
            m_oAuthAccessTokenSecret = qs["oauth_token_secret"];
            var x_oauth_user_name = qs["x_oauth_user_name"];
            var x_oauth_user_guid = qs["x_oauth_user_guid"];
            var x_scope = qs["x_scope"];
            var xoauth_problem = qs["xoauth_problem"];
            var oauth_error_message = qs["oauth_error_message"];
            m_sessionHandle = qs["oauth_session_handle"];

            OAuthResult result = new OAuthResult();
            result.AccessToken = m_oAuthAccessToken;
            result.AccessTokenSecret = m_oAuthAccessTokenSecret;
            result.oauth_user_name = x_oauth_user_name;
            result.oauth_user_guid = x_oauth_user_guid;
            result.scope = x_scope;
            result.oauth_problem = xoauth_problem;
            result.oauth_error_message = oauth_error_message;
            result.sessionHandle = m_sessionHandle;

            return RedirectToAction("AuthenticationSucceed", "Authenticate", result);
        }

        public ActionResult AuthenticationSucceed(OAuthResult result)
        {
            ViewBag.userId = result.oauth_user_guid;
            return View(result);
        }

        public ActionResult AuthenticationFailed(string reason)
        {
            ViewBag.Reason = reason;
            return View();
        }


    }
}
