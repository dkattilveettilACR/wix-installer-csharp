using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using Acr.Delivery.Packaging;

namespace SiteServerCustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult VerifyCredentials(Session session)
        {
            var userName = session["USERNAME"];
            if (string.IsNullOrEmpty(userName))
            {
                ShowMessage("User name cannot be empty", session);
                return ActionResult.Failure;
            }

            var password = session["PASSWORD"];
            var confirmPassword = session["CONFIRMPASSWORD"];
            if (string.IsNullOrEmpty(password))
            {
                ShowMessage("Password cannot be empty", session);
                return ActionResult.Failure;
            }

            if (!password.Equals(confirmPassword))
            {
                ShowMessage("Passwords do not match", session);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }
        private static void ShowMessage(string message, Session session)
        {
            Record record = new Record() { FormatString = message };
            session.Message(
                InstallMessage.Error | (InstallMessage)(System.Windows.Forms.MessageBoxIcon.Error) |
                (InstallMessage)System.Windows.Forms.MessageBoxButtons.OK,
                record);
        }

        [CustomAction]
        public static ActionResult Install(Session session)
        {
            var packager = new Packager("c:\test", "test");
            return ActionResult.Success;
        }

    }
}
