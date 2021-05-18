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

        public static string BASESECRETURI = "https://ssn-key-vaults-20210224.vault.azure.net/"; 
        public static string CLIENT_ID = "4bf72f5c-fab6-43e2-948c-18990b1e8fe1";
        public static string CLIENT_SECRET = "l_3fR4z2T4okcFVb8-LU3A.F.-xRlL7y2n";
    
        static KeyVaultClient kvc = null;

        public static async void CheckInAsync()
        {

           await DoVault();

            Console.WriteLine("Checked in OK");
        }


        public static async Task<string>  GetToken ( string authority , string resource, string scope )
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCredential = new ClientCredential(CLIENT_ID, CLIENT_SECRET);

            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCredential);

            if ( result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;

        }


        public static async Task   DoVault()
        {

            kvc = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));



            //SecretBundle secret = Task.Run(() =>

            //     kvc.GetSecretAsync(BASESECRETURI + @"secrets/" + secretName).ConfigureAwait(false).GetAwaiter().GetResult()
            //);


            //SecretBundle secret =

            //   kvc.GetSecretAsync(BASESECRETURI + @"secrets/" + secretName).ConfigureAwait(false).GetAwaiter().GetResult();

            string secretName = "ssn-secret-test-pc-20210224";
            SecretBundle secret = await kvc.GetSecretAsync(BASESECRETURI + @"secrets/" + secretName); //.ConfigureAwait(false).GetAwaiter().GetResult();

            writeSecrets();
        }

        private static async Task writeSecrets()
        {
            SecretAttributes attrs = new SecretAttributes
            {
                Enabled = true,
                Expires = DateTime.UtcNow.AddDays(2),
                NotBefore = DateTime.UtcNow.AddMinutes(5)
            };


            IDictionary<string, string> tags= new Dictionary<string, string>();

            // attrs.Add("PS-312-AzureStorageTable", "__value__PS-312-AzureStorageTable__value");
 
            string contentType = null;

            string secretName = "PS-312-AzureStorageTable";
            string secretValue = "__value__PS-312-AzureStorageTable__value";

            //SecretBundle bundle = await kvc.SetSecretAsync(BASESECRETURI, secretName, secretValue, tags, contentType, attrs);
            await kvc.DeleteSecretAsync(BASESECRETURI, secretName);
             
        }

    }
}
