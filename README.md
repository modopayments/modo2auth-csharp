## modo2auth-csharp

> A .NET Core solution to generate authentication tokens to communicate with the Modo servers

### Prerequesites

**Credentials** that are created and shared by Modo. These will be different for each environment (`int`, `prod`, `local` etc...).

- `apiId` - API id/key from Modo
- `apiSecret` - API secret from Modo

These values will be used when instantiating the Modo2Auth class.

**API URL** targeting an appropriate Modo environment.

- `fullUri` - URL of a given Modo environment.

### Example Usage - .NET Core 3.1
Here's an example using .NET Core 3.1 libraries to make a GET and POST request. You can use your preferred method or library.

See this code in action by running the modo2auth-csharp.csproj from command line by running `dotnet run .` in the root of this project, or using your preferred method.

#### `POST` Example
```
# 1 - Instantiate the Modo2Auth class with a given apiSecret and apiId
Modo2Auth auth = new Modo2Auth(apiSecret, apiId);

# 2 - Define the Request URL and Request body
String apiUri = "/v2/reports";
String fullUri = "https://checkout.mtktest.modopayments.net/v2/reports";
String requestBody = "{\"start_date\": \"2020-07-13T00:00:00Z\",\"end_date\": \"2020-07-13T23:59:59Z\"}";

# 3 - Generate an HttpWebRequest (POST Method) to our given endpoint
HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUri);
request.ContentType = "application/json";
request.Method = WebRequestMethods.Http.Post;

# 4 - Create an auth token from the apiUri and requestBody, then add it to the Auth header
## 4 Note: apiUri is the request path after the server name/IP address
request.Headers.Add("Authorization", auth.createModoToken(apiUri, requestBody));

# 5 - Send the HTTP Request
using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
{
    streamWriter.Write(requestBody);
    streamWriter.Flush();
    streamWriter.Close();
}

#6 - Read in the HTTP Response
HttpWebResponse response = (HttpWebResponse)request.GetResponse();
```

#### `GET` Example
```
# 1 - Instantiate the Modo2Auth class with a given apiSecret and apiId
Modo2Auth auth = new Modo2Auth(apiSecret, apiId);

# 2 - Define the Request URL
apiUri = "/v2/vault/public_key";
fullUri = "https://checkout.mtktest.modopayments.net/v2/vault/public_key";

# 3 - Generate an HttpWebRequest (GET Method) to our given endpoint
HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUri);
request.ContentType = "application/json";
request.Method = WebRequestMethods.Http.Get;

# 4 - Create an auth token with just the apiUri, then add it to the Auth header
## 4 Note: apiUri is the request path after the server name/IP address
request.Headers.Add("Authorization", auth.createModoToken(apiUri));

#6 - Send the GET Request and read in the HTTP Response
HttpWebResponse response = (HttpWebResponse)request.GetResponse();
```

### Modo2Auth.cs

#### `public Modo2Auth(String apiSecret, String apiId)`

The constructor for Modo2Auth takes in the apiId and apiSecret and sets up the appropriate hashing algorithms to generate a Modo2 Auth Token.

#### `public String createModoToken(String uri, String body="")`

Returns a Modo2 Auth Token string for a given request uri and potential request body. If second parameter is omitted, body will be set to an empty string. This scenario is applicable to GET requests, or other request types without a request body.

No exceptions expected during the execution of this function

- `uri` (string) - The request path targeted by this request
- `body` (string) (optional) - The exact request body to be sent out with this token in the header. Null or omitted parameter will result in an empty string

### Additional Information

The Modo2Auth library has been tested for the following .NET libraries:
- .NET Core 3.1

- Test
