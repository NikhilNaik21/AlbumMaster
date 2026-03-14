using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace AWSHelper
{
    public class AWSWrapper
    {
        IAmazonS3 s3Client;
        public AWSWrapper(string accesskey, string secretkey)
        {
            BasicAWSCredentials credentials = new BasicAWSCredentials(accesskey, secretkey);
            s3Client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
        }

        public bool BucketExists(string bucketName)
        {
            try
            {
                return s3Client.DoesS3BucketExistAsync(bucketName).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool CreateBucket(string bucketName)
        {
            try
            {
                PutBucketResponse response = s3Client.PutBucketAsync(bucketName).Result;
                 if( response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                    return true;
                }
                else
                {
                    throw new Exception($"Failed to create bucket. Status code: {response.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"AWS Error: {ex.Message}");
            }
        }

        public void UploadImage(Stream imgData, string albumName, string fileName)
        {
            if (imgData == null || imgData.Length == 0)
                throw new ArgumentException("Invalid stream");

            try
            {
                var transferUtility = new TransferUtility(s3Client);
                string bucket = albumName.Replace(" ", "").ToLower();
                imgData.Position = 0;
                transferUtility.Upload(imgData, bucket, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"S3 Upload failed for {fileName}: {ex.Message}");
            }
        }

    }
}