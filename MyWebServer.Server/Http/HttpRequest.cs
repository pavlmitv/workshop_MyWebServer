﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWebServer.Server.Http
{
    public class HttpRequest
    {
        //http messages --> https://developer.mozilla.org/en-US/docs/Web/HTTP/Messages#http_requests

        private const string NewLine = "\r\n";
        public HttpMethod Method { get; private set; }
        public string Path { get; private set; }
        public Dictionary<string, string> Query { get; private set; }   //query strings ще ги пазим в речник
        public HttpHeaderCollection Headers { get; private set; }
        public string Body { get; private set; }

        public static HttpRequest Parse(string request)
        {
            string[] lines = request.Split(NewLine);

            var startLine = lines.First().Split(" ");

            var method = ParseHttpMethod(startLine[0]);

            var url = startLine[1];

            var (path, query) = ParseUrl(url);      //tuple 

            var headers = ParseHttpHeaders(lines.Skip(1));

            var bodyLines = lines.Skip(headers.Count + 2).ToArray();

            var body = string.Join(NewLine, bodyLines);

            return new HttpRequest
            {
                Method = method,
                Path = path,
                Query = query,
                Headers = headers,
                Body = body,
            };
        }



        private static HttpHeaderCollection ParseHttpHeaders(IEnumerable<string> headerLines)
        {
            var headerCollection = new HttpHeaderCollection();

            foreach (var headerLine in headerLines)
            {
                if (headerLine == string.Empty)
                {
                    break;
                }
                var headerParts = headerLine.Split(":", 2);
                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Request isn't valid");
                }
                var headerName = headerParts[0]; ;
                var headerValue = headerParts[1].Trim();

                headerCollection.Add(headerName, headerValue);
            }

            return headerCollection;
        }

        private static HttpMethod ParseHttpMethod(string method)
            => method.ToUpper() switch
            {
                "GET" => HttpMethod.Get,
                "DELETE" => HttpMethod.Delete,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                _ => throw new InvalidOperationException($"Method {method} is not supported.")
            };

        //пр. за query string: /Cats?name=Ivan&Age=5
        private static (string, Dictionary<string, string>) ParseUrl(string url)    //tuple --> едновременно можем да върнем 2 променливи от метода, вместо да правим клас и да използваме property-тата му; 
        {
            var urlParts = url.Split("?");
            var path = urlParts[0].ToLower();
            var query = urlParts.Length > 1
                ? ParseQuery(urlParts[1])
                : new Dictionary<string, string>();

            return (path, query);
        }

        //[0] -> /Cats
        //[1] -> name=Ivan&Age=5
        private static Dictionary<string, string> ParseQuery(string queryString)    //tuple
        {
            return queryString
                    .Split("&")
                    .Select(part => part.Split("="))
                    .Where(part => part.Length == 2)
                    .ToDictionary(part => part[0], part => part[1]);

        }
        //private static string[] GetStartLine (string request)
        //{

        //}
    }
}
