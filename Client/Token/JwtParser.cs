using System.Security.Claims;
using System.Text.Json;

namespace Client.Token
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    // HA A KULCS "role", AKKOR ClaimTypes.Role-t HASZNÁLUNK
                    if (kvp.Key == "role")
                    {
                        if (kvp.Value.ToString().StartsWith("["))
                        {
                            var roles = JsonSerializer.Deserialize<string[]>(kvp.Value.ToString());
                            foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, kvp.Value.ToString()));
                        }
                    }
                    // HA A KULCS "unique_name", AKKOR ClaimTypes.Name-t HASZNÁLUNK
                    else if (kvp.Key == "unique_name")
                    {
                        claims.Add(new Claim(ClaimTypes.Name, kvp.Value.ToString()));
                    }
                    // MINDEN MÁS ESETBEN
                    else
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
                    }
                }
            }
            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
