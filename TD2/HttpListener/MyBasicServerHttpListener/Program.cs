using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MyBasicServerHTTPHeader
{
    internal class Header
    {
        WebHeaderCollection collection;
        public Header(HttpListenerRequest request) {this.collection = (WebHeaderCollection)request.Headers;}
        public string getHeader(HttpRequestHeader header) {return collection.Get(header.ToString());}
        public WebHeaderCollection getHeaders() {return this.collection;}
        public void printHeaders() {Console.WriteLine($"{collection}");}
        public void printHeader(HttpRequestHeader header) {Console.WriteLine($"{header}: {collection.Get(header.ToString())}");}
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }

            HttpListener listener = new HttpListener();
            if (args.Length != 0)
            {
                foreach (string s in args)
                {listener.Prefixes.Add(s);}
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                Header header = new Header(request);
                header.printHeaders();

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                Console.WriteLine($"Received request for {request.Url}");
                Console.WriteLine(documentContents);

                String HTTP_ROOT = "..\\..\\www\\pub";

                HttpListenerResponse response = context.Response;
                string responseString = null;

                if (request.HttpMethod == "GET")
                {
                    try
                    {
                        responseString = File.ReadAllText(HTTP_ROOT + request.Url.AbsolutePath.Replace('/', '\\'));
                    }
                    catch (Exception e)
                    { }
                }

                if (responseString != null)
                {
                    string entete = "http / 1.0 200 OK\n\n";
                    string resp = entete + responseString + "HEADERS : " + header.getHeader();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(resp);
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }
    }
}