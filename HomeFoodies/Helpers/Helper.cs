using HomeFoodies.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HomeFoodies.Helpers
{
    public class SessionData
    {
        public LoginUserGetLogin LoggedInUser { get; set; }
    }
    public static class Helper
    {
        public static String SendSMSByGCM(String pName, String pNumber, Int32 pType, String pCode)
        {
            AndroidGCMSMSGateway _objGCM = new AndroidGCMSMSGateway(ConfigurationManager.AppSettings["GCMSenderID"],
                                                                    ConfigurationManager.AppSettings["GCMAPIKey"],
                                                                    ConfigurationManager.AppSettings["GCMRegistrationID"]);
            return _objGCM.SendNotification(pName, pNumber, pType, pCode);
        }

        public static Boolean SendEmail(String pTo, String pSubject, String pEmailBody)
        {
            Emails _objEmail = new Emails(ConfigurationManager.AppSettings["EmailID"],
                                          ConfigurationManager.AppSettings["EmailPassword"],
                                          ConfigurationManager.AppSettings["SMTPHost"],
                                          Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]));
            return _objEmail.SendEmail(pTo, pSubject, pEmailBody);
        }

        public static String FormatMobileNumber(String pPhone)
        {
            String number = "";
            try
            {
                pPhone = pPhone.Replace("+", "").Replace("-", "").Replace(".", "").Replace(" ", "");
                if (pPhone.Length > 10)
                {
                    if (pPhone.Substring(0, 1) == "0")
                    {
                        pPhone = pPhone.Remove(0, 1);
                        pPhone = "92" + pPhone;
                    }
                    if (pPhone.Substring(0, 3) == "923")
                    {
                        if (pPhone.Length == 12)
                        {
                            number = pPhone.Substring(0, 3).Replace("923", "0") + pPhone.Substring(2, 10);
                            //number = pPhone;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return number;
        }
    }

    public enum StatusCodes
    {
        None = 0,
        Active = 1,
        InActive = 2,
        VerificationSent = 3,
        Blacklisted = 4,
        Approved = 5,
        Delivered = 6,
        CustomerVerified = 7,
        SupplierVerified = 8,
        InProgress = 9
    }
}