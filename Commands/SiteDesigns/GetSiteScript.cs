﻿#if !ONPREMISES
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base;
using SharePointPnP.PowerShell.Commands.Base.PipeBinds;
using System.Linq;
using System.Management.Automation;

namespace SharePointPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "PnPSiteScript", SupportsShouldProcess = true)]
    [CmdletHelp(@"Retrieve Site Scripts that have been registered on the current tenant.",
        Category = CmdletHelpCategory.TenantAdmin,
        SupportedPlatform = CmdletSupportedPlatform.Online)]
    [CmdletExample(
        Code = @"PS:> Get-PnPSiteScript",
        Remarks = "Returns all registered site scripts",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-PnPSiteScript -Identity 5c73382d-9643-4aa0-9160-d0cba35e40fd",
        Remarks = "Returns a specific registered site script",
        SortOrder = 2)]
    public class GetSiteScript : PnPAdminCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, HelpMessage = "If specified will retrieve the specified site script")]
        public GuidPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (MyInvocation.BoundParameters.ContainsKey("Identity"))
            {
                var script = Tenant.GetSiteScript(ClientContext, Identity.Id);
                script.EnsureProperties(s => s.Content, s => s.Title, s => s.Id, s => s.Version);
                WriteObject(script);
            }
            else
            {
                var scripts = Tenant.GetSiteScripts();
                ClientContext.Load(scripts, s => s.Include(sc => sc.Id, sc => sc.Title, sc => sc.Version, sc => sc.Content));
                ClientContext.ExecuteQueryRetry();
                WriteObject(scripts.ToList(), true);
            }
        }
    }
}
#endif