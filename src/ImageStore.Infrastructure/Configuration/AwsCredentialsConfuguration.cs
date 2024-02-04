using Amazon;
using Amazon.Runtime;

namespace ImageStore.Infrastructure.Configuration
{
    public class AwsCredentialsConfuguration
    {
        public BasicAWSCredentials Credentials { get; }
        public RegionEndpoint Region { get; }

        public AwsCredentialsConfuguration(string accessKey, string secretKey, string region)
        {
            // No need to check for null values because it is handled internally
            Credentials = new BasicAWSCredentials(accessKey, secretKey); 
            Region = Amazon.RegionEndpoint.GetBySystemName(region);
        }
    }
}
