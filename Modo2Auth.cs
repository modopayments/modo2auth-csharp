using System;
using System.Text;
using System.Security.Cryptography;

namespace Modo
{
    public class Modo2Auth
    {
        private const String cHeaderPlain = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
        private String _header;
        private HMAC _hmac;
        private SHA256 _shaGenerator;
        private String _apiId;
        private DateTime _epochDate = new DateTime(1970, 1, 1);

        public Modo2Auth(String apiSecret, String apiId)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(apiSecret);
            _hmac = new HMACSHA256(keyBytes);
            _shaGenerator = SHA256.Create();

            _apiId = apiId;
            _header = toBase64UrlSafeString( Encoding.ASCII.GetBytes(cHeaderPlain) );
        }

        private String toBase64UrlSafeString(byte[] bytes) {
            String base64String = Convert.ToBase64String(bytes);
            base64String = base64String.TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return base64String;
        }

        private String createSignature(String payload)
        {
            String message = String.Format("{0}.{1}", _header, payload);
            byte[] message_bytes = Encoding.ASCII.GetBytes(message);

            byte[] hashBytes;
            lock(_hmac) {
                hashBytes = _hmac.ComputeHash(message_bytes);
            }
            String signature = toBase64UrlSafeString(hashBytes);
            return signature;
        }

        public String createModoToken(String uri, byte[] body) {

            byte[] hashBytes;
            lock(_shaGenerator) {
                hashBytes = _shaGenerator.ComputeHash(body);
            }
            String bodySha = BitConverter.ToString(hashBytes).Replace("-","").ToLower();    // also remove the - chars that c# adds in the conversion

            // Current time in seconds since Epoch
            TimeSpan t = DateTime.UtcNow - _epochDate;
            int secondsSinceEpoch = (int)t.TotalSeconds;

            // create the payload base64 string (trimmed)
            String payloadPlain = String.Format("{{\"iat\":{0},\"api_identifier\":\"{1}\",\"api_uri\":\"{2}\",\"body_hash\":\"{3}\"}}", secondsSinceEpoch, _apiId, uri, bodySha);
            String payload = toBase64UrlSafeString( Encoding.ASCII.GetBytes(payloadPlain) );

            // Sign it with the API secret
            String signature = createSignature(payload);

            // put it all together and create a token
            String modoAuthToken = String.Format("MODO2 {0}.{1}.{2}", _header, payload, signature);

            return modoAuthToken;
        }

        public String createModoToken(String uri) {
            return createModoToken(uri, "", Encoding.UTF8);
        }

        public String createModoToken(String uri, String body, Encoding encodingScheme)
        {
            if (body == null)
                body = "";

            // Get a SHA of the request body 
            byte[] bodyBytes = encodingScheme.GetBytes(body);

            return createModoToken(uri, bodyBytes);
        }

    }
}