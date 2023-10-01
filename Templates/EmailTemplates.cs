using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Templates
{
    public class EmailTemplates
    {
        public static string GetVerificationEmail(string userName, string otpCode)
        {
            return $@"
            <html>
            <head>
                <style>
                    /* Define your CSS styles here */
                    body {{ font-family: Arial, sans-serif; }}
                    p {{ font-size: 16px; }}
                    strong {{ color: #007bff; }}
                    /* Add more styles as needed */
                </style>
            </head>
            <body>
                <p>Hi {userName},</p>
                <p>Thank you for registering. Please verify your email by entering the following OTP:</p>
                <p><strong>{otpCode}</strong></p>
                <p>Regards,<br />AuthFilterProj</p>
            </body>
            </html>";
        }

        // forgot password email template
        public static string GetForgotPasswordEmail(string userName, string otpCode)
        {
            return $@"
            <html>
            <head>
                <style>
                    /* Define your CSS styles here */
                    body {{ font-family: Arial, sans-serif; }}
                    p {{ font-size: 16px; }}
                    strong {{ color: #007bff; }}
                    /* Add more styles as needed */
                </style>
            </head>
            <body>
                <p>Hi {userName},</p>
                <p>Thank you for registering. Please verify your email by entering the following OTP:</p>
                <p><strong>{otpCode}</strong></p>
                <p>Regards,<br />AuthFilterProj</p>
            </body>
            </html>";
        }



    }



}