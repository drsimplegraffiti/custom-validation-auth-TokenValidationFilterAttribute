using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthFilterProj.Models;

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

        //GetNewDeviceLoginEmail
        public static string GetNewDeviceLoginEmail(string userName, string IpAddress, string UserAgent)
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
                <p>A new device has been used to login to your account.</p>
                <p><strong>IP Address: {IpAddress}</strong></p>
                <p><strong>User Agent: {UserAgent}</strong></p>
                <p>If this was you, you can safely ignore this email.</p>
                <p>If you suspect unauthorized activity on your account, please change your password.</p>
                
                <p>Regards,<br />AuthFilterProj</p>
            </body>
            </html>";
        }
        



    }



}