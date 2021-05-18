using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 05/17/2021 12:44 am - SSN - 
// install-package Microsoft.Azure.KeyVault
// install-package Microsoft.IdentityModel.Clients.ActiveDirectory

namespace AzureKeyVault
{
    public class VaultDataAccess
    {

        // https://www.codeproject.com/Tips/1430794/Using-Csharp-NET-to-Read-and-Write-from-Azure-Key

        // public static string BASESECRETURI = "https://ssn-key-vaults-20210224.vault.azure.net/";

        //public static string CLIENT_ID = "91eb575a-bf1a-40fc-af6c-a617beeee7c1";
        //public static string CLIENT_SECRET = "9xEe.4pgoXk24ea-_gB41OrL60vx4hW~BF";

        public static string BASESECRETURI = Environment.GetEnvironmentVariable("ssn-key-vault-url");

        public static string CLIENT_ID = Environment.GetEnvironmentVariable("AzureVaultAccess-20210518_CLIENT_ID");
        public static string CLIENT_SECRET = Environment.GetEnvironmentVariable("AzureVaultAccess-20210518_CLIENT_SECRET");


        static KeyVaultClient kvc = null;



        public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCredential = new ClientCredential(CLIENT_ID, CLIENT_SECRET);

            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCredential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;

        }


        public static async Task<string> getSecret(string secretName)
        {

            kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));


            //SecretBundle secret = Task.Run(() =>

            //     kvc.GetSecretAsync(BASESECRETURI + @"secrets/" + secretName).ConfigureAwait(false).GetAwaiter().GetResult()
            //);


            //SecretBundle secret =

            //   kvc.GetSecretAsync(BASESECRETURI + @"secrets/" + secretName).ConfigureAwait(false).GetAwaiter().GetResult();

            try
            {
                SecretBundle secret = await kvc.GetSecretAsync(BASESECRETURI + @"secrets/" + secretName); //.ConfigureAwait(false).GetAwaiter().GetResult();
                if (secret == null) return null;
                return secret.Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;

        }

        public static async Task writeSecrets_Incomplete_But_Functional(string secretName, string secretValue)
        {


            kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));



            SecretAttributes attrs = new SecretAttributes
            {
                Enabled = true,
                Expires = DateTime.UtcNow.AddDays(2),
                NotBefore = DateTime.UtcNow.AddMinutes(5)
            };


            IDictionary<string, string> tags = new Dictionary<string, string>();

            // attrs.Add("PS-312-AzureStorageTable", "__value__PS-312-AzureStorageTable__value");

            string contentType = null;

            //string secretName = "PS-312-AzureStorageTable";
            //string secretValue = "__value__PS-312-AzureStorageTable__value";

            SecretBundle bundle = await kvc.SetSecretAsync(BASESECRETURI, secretName, secretValue, tags, contentType, attrs);
            await kvc.DeleteSecretAsync(BASESECRETURI, secretName);

        }

    }
}
