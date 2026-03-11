using System.Net;
using System.Text;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

const string html = "<html><body>test<div>test</div>test</body></html>";

var htmlBytes = Encoding.UTF8.GetBytes(html);
var htmlStream = new MemoryStream(htmlBytes);

var jsonReader = new StreamingUtf8JsonReader(new MemoryStream(htmlBytes));
var jsonUnmarshallerContext = new JsonUnmarshallerContext(htmlStream, true, new TestWebResponseData(htmlStream), true);
var jsonUnmarshaller = new JsonErrorResponseUnmarshaller();

var buffer = new byte[htmlBytes.Length];
jsonUnmarshallerContext.Stream.ReadExactly(buffer, 0, buffer.Length);

Console.WriteLine("Unmarshalling...");

jsonUnmarshaller.Unmarshall(jsonUnmarshallerContext, ref jsonReader);

Console.WriteLine("Unmarshalling complete");

class TestWebResponseData(Stream stream) : IWebResponseData
{
    public long ContentLength => stream.Length;

    public IHttpResponseBody ResponseBody { get; } = new TestHttpResponseBody(stream);

    public string ContentType => "text/html";

    public HttpStatusCode StatusCode => HttpStatusCode.ServiceUnavailable;

    public bool IsSuccessStatusCode => false;

    public string[] GetHeaderNames() => [];

    public string? GetHeaderValue(string headerName) => null;

    public bool IsHeaderPresent(string headerName) => false;
}

class TestHttpResponseBody(Stream stream) : IHttpResponseBody
{
    public void Dispose() { }

    public Stream OpenResponse() => stream;

    public Task<Stream> OpenResponseAsync() => Task.FromResult(stream);
}