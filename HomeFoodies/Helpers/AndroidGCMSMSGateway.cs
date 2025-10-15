using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace HomeFoodies.Helpers
{
    public class AndroidGCMSMSGateway
    {
        private String SENDER_ID;
        private String API_KEY;
        private String REGISTRATION_ID;

        public AndroidGCMSMSGateway(String pSenderID, String pAPIKey, String pRegistrationID)
        {
            SENDER_ID = pSenderID;
            API_KEY = pAPIKey;
            REGISTRATION_ID = pRegistrationID;
        }

        public string SendNotification(String pName, String pNumber, Int32 pType, String pMessage)
        {
            string sResponseFromServer = "";
            try
            {
                pNumber = FormatMobileNumber(pNumber);

                if (pNumber.Length != 11)
                    return "";

                var value = "";
                if (pType == 1)//Login Verification 
                    value = "Hi " + pName + "," + System.Environment.NewLine + "Your verification code is " + pMessage;
                else if (pType == 2) //Notification
                    value = "Hi " + pName + "," + System.Environment.NewLine + pMessage + System.Environment.NewLine + "For more details, visit the portal!";
                else if (pType == 3) //Customer Order Notification
                    value = "Hi " + pName + "," + System.Environment.NewLine + "Your Order No. " + pMessage + " has been placed!";
                else if (pType == 4) //Supplier Order Notification
                    value = "Hi " + pName + "," + System.Environment.NewLine + pMessage;
                else if (pType == 5) //Order Verification
                    value = "Hi" + pName +"," +System.Environment.NewLine +pMessage;
                else if (pType == 10) //General
                    value = pMessage;

                WebRequest tRequest;
                tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", API_KEY));

                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                string postData = "collapse_key=score_update&time_to_live=108& delay_while_idle=1&data.message=" + value + "&data.number=" + pNumber + "&registration_id=" + REGISTRATION_ID + "";
                Console.WriteLine(postData);
                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;

                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tResponse.Close();
            }
            catch (Exception)
            {

                throw;
            }
            return sResponseFromServer;
        }

        private String FormatMobileNumber(String pPhone)
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
}