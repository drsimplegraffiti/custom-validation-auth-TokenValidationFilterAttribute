using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;

namespace AuthFilterProj.Utils
{
    public class CloudinaryHelper
    {
         public static Cloudinary CreateCloudinaryInstance(IConfiguration configuration)
        {
            Account cloudinaryCredentials = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            return new Cloudinary(cloudinaryCredentials);
        }
        
    }
}