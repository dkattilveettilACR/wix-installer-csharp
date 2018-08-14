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
                InstallMessage.Error,
                record);
        }

        [CustomAction]
        public static ActionResult Install(Session session)
        {
            var installer = new Acr.SiteServer.InstallUtil.Installer();
            var installed = installer.Install(
            session["WIXUI_INSTALLDIR"],
            $"{session["WIXUI_INSTALLDIR"]}\\Acr.SiteServer.Shell.+ {session["VERSION"]}.zip",
            session["VERSION"],
            "Acr.SiteServer.Shell.exe",
            "Shell",
            $"http://localhost:{ session["VERSION"]}/",
            "CN=acr.org",
            new Acr.SiteServer.InstallUtil.Logger(session));
            return (installed ? ActionResult.Success : ActionResult.Failure);
        }

        [CustomAction]
        public static ActionResult Uninstall(Session session)
        {
            var installer = new Acr.SiteServer.InstallUtil.Installer();
            var installed = installer.Uninstall(
            session["WIXUI_INSTALLDIR"],
            session["VERSION"],
            "Acr.SiteServer.Shell.exe",
            $"http://localhost:{ session["VERSION"]}/",
            new Acr.SiteServer.InstallUtil.Logger(session));
            return (installed ? ActionResult.Success : ActionResult.Failure);
        }
    }
}
