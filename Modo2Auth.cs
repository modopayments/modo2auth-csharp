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
            _header = Convert.ToBase64String(Encoding.ASCII.GetBytes(cHeaderPlain)).TrimEnd('=');
        }

        private String createSignature(String payload)
        {
            String message = String.Format("{0}.{1}", _header, payload);
            byte[] message_bytes = Encoding.ASCII.GetBytes(message);

            byte[] hashBytes = _hmac.ComputeHash(message_bytes);

            String signature = Convert.ToBase64String(hashBytes);
            signature = signature.TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return signature;
        }

        public String createModoToken(String uri, String body="")
        {
            if (body == null)
                body = "";

            // Get a SHA of the request body 
            byte[] hashBytes = _shaGenerator.ComputeHash(Encoding.ASCII.GetBytes(body));
            String bodySha = BitConverter.ToString(hashBytes).Replace("-","").ToLower();

            // Current time in seconds since Epoch
            TimeSpan t = DateTime.UtcNow - _epochDate;
            int secondsSinceEpoch = (int)t.TotalSeconds;

            // create the payload base64 string (trimmed)
            String payloadPlain = String.Format("{{\"iat\":{0},\"api_identifier\":\"{1}\",\"api_uri\":\"{2}\",\"body_hash\":\"{3}\"}}", secondsSinceEpoch, _apiId, uri, bodySha);
            String payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(payloadPlain)).TrimEnd('=');

            // Sign it with the API secret
            String signature = createSignature(payload);

            // put it all toegether and create a token
            String modoAuthToken = String.Format("MODO2 {0}.{1}.{2}", _header, payload, signature);

            return modoAuthToken;
        }

    }
}