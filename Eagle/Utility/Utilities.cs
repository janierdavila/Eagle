﻿using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Eagle.Model;

namespace Eagle.Utility
{
    public class Utilities
    {
        static bool _invalid;

        public static bool IsValidEmail(string strIn)
        {
            _invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (_invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                _invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        public static void SendEmail(string to, string subject, string body, SmtpInfo smtpInfo)
        {
            try
            {
                using (var client = new SmtpClient(smtpInfo.Host, smtpInfo.Port))
                {
                    client.UseDefaultCredentials = string.IsNullOrWhiteSpace(smtpInfo.UserName);
                    client.EnableSsl = smtpInfo.EnableSsl;
                    
                    if (!string.IsNullOrWhiteSpace(smtpInfo.UserName))
                    {
                        client.Credentials = new NetworkCredential(smtpInfo.UserName, smtpInfo.Password);
                    }

                    var msg = new MailMessage("noreply@qpaynet.com", to, subject, body);
                    msg.IsBodyHtml = true;

                    client.Send(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

