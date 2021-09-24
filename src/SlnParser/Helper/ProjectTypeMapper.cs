using SlnParser.Contracts;
using SlnParser.Contracts.Helper;
using System;
using System.Collections.Generic;

namespace SlnParser.Helper
{
    internal class ProjectTypeMapper : IProjectTypeMapper
    {
        private readonly IDictionary<Guid, ProjectType> _mapping;

        public ProjectTypeMapper()
        {
            _mapping = GetMapping();
        }

        public ProjectType Map(Guid typeGuid)
        {
            return _mapping.ContainsKey(typeGuid)
                ? _mapping[typeGuid]
                : ProjectType.Unknown;
        }

        private static IDictionary<Guid, ProjectType> GetMapping()
        {
            // c.f.: https://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
            return new Dictionary<Guid, ProjectType>
            {
                // NOTE: this list is sorted by the Guid's, so it's easier to see when there are duplicates
                { new Guid("06A35CCD-C46D-44D5-987B-CF40FF872267"), ProjectType.DeploymentMergeModule },
                { new Guid("14822709-B5A1-4724-98CA-57A101D1B079"), ProjectType.WorkflowCSharp },
                { new Guid("20D4826A-C6FA-45DB-90F4-C717570B9F32"), ProjectType.LegacySmartDeviceCSharp },
                { new Guid("2150E333-8FDC-42A3-9474-1A3956D46DE8"), ProjectType.SolutionFolder },
                { new Guid("2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2"), ProjectType.XnaXbox },
                { new Guid("32F31D43-81CC-4C15-9DE6-3FC5453562B6"), ProjectType.WorkflowFoundation },
                { new Guid("349C5851-65DF-11DA-9384-00065B846F21"), ProjectType.AspNetMvc5 },
                { new Guid("3AC096D0-A1C2-E12C-1390-A8335801FDAB"), ProjectType.Test },
                { new Guid("3D9AD99F-2412-4246-B90B-4EAA41C64699"), ProjectType.Wcf },
                { new Guid("3EA9E505-35AC-4774-B492-AD1749C4943A"), ProjectType.DeploymentCab },
                { new Guid("4D628B5B-2FBC-4AA6-8C16-197242AEB884"), ProjectType.SmartDeviceCSharp },
                { new Guid("4F174C21-8C12-11D0-8340-0000F80270F8"), ProjectType.Database },
                { new Guid("54435603-DBB4-11D2-8724-00A0C9A8B90C"), ProjectType.VisualStudioInstallerProjectExtension },
                { new Guid("593B0543-81F6-4436-BA1E-4747859CAAE2"), ProjectType.SharePointCSharp },
                { new Guid("603C0E0B-DB56-11DC-BE95-000D561079B0"), ProjectType.AspNetMvc1 },
                { new Guid("60DC8134-EBA5-43B8-BCC9-BB4BC16C2548"), ProjectType.Wpf },
                { new Guid("68B1623D-7FB9-47D8-8664-7ECEA3297D4F"), ProjectType.SmartDeviceVb },
                { new Guid("66A26720-8FB5-11D2-AA7E-00C04F688DDE"), ProjectType.ProjectFolders },
                { new Guid("6BC8ED88-2882-458C-8E55-DFD12B67127B"), ProjectType.MonoTouch },
                { new Guid("6D335F3A-9D43-41b4-9D22-F6F17C4BE596"), ProjectType.XnaWindows },
                { new Guid("76F1466A-8B6D-4E39-A767-685A06062A39"), ProjectType.WindowsPhoneWebView },
                { new Guid("786C830F-07A1-408B-BD7F-6EE04809D6DB"), ProjectType.PortableClassLibrary },
                { new Guid("8BB2217D-0F2D-49D1-97BC-3654ED321F3B"), ProjectType.AspNet5 },
                { new Guid("8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942"), ProjectType.CPlusPlus },
                { new Guid("978C614F-708E-4E1A-B201-565925725DBA"), ProjectType.DeploymentSetup },
                { new Guid("9A19103F-16F7-4668-BE54-9A1E7A4F7556"), ProjectType.CSharpClassLibrary },
                { new Guid("A1591282-1198-4647-A2B1-27E5FF5F6F3B"), ProjectType.SilverLight },
                { new Guid("A5A43C5B-DE2A-4C0C-9213-0A381AF9435A"), ProjectType.UniversalWindowsClassLibrary },
                { new Guid("A860303F-1F3F-4691-B57E-529FC101A107"), ProjectType.Vsta },
                { new Guid("A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124"), ProjectType.Database },
                { new Guid("AB322303-2255-48EF-A496-5904EB18DA55"), ProjectType.DeploymentSmartDeviceCab },
                { new Guid("B69E3092-B931-443C-ABE7-7E7B65F2A37F"), ProjectType.MicroFramework },
                { new Guid("BAA0C2D2-18E2-41B9-852F-F413020CAA33"), ProjectType.Vsto },
                { new Guid("BC8A1FFA-BEE3-4634-8014-F334798102B3"), ProjectType.WindowsStoreApps },
                { new Guid("BF6F8E12-879D-49E7-ADF0-5503146B24B8"), ProjectType.CSharpDynamicsAxAot },
                { new Guid("C089C8C0-30E0-4E22-80C0-CE093F111A43"), ProjectType.WindowsPhoneAppCSharp },
                { new Guid("C252FEB5-A946-4202-B1D4-9916A0590387"), ProjectType.VisualDatabaseTools },
                { new Guid("CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8"), ProjectType.LegacySmartDeviceVb },
                { new Guid("D399B71A-8929-442a-A9AC-8BEC78BB2433"), ProjectType.XnaZune },
                { new Guid("D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8"), ProjectType.WorkflowVb },
                { new Guid("DB03555F-0C8B-43BE-9FF9-57896B3C5E56"), ProjectType.WindowsPhoneAppVb },
                { new Guid("E24C65DC-7377-472B-9ABA-BC803B73C61A"), ProjectType.WebSite },
                { new Guid("E3E379DF-F4C6-4180-9B81-6769533ABE47"), ProjectType.AspNetMvc4 },
                { new Guid("E53F8FEA-EAE0-44A6-8774-FFD645390401"), ProjectType.AspNetMvc3 },
                { new Guid("E6FDF86B-F3D1-11D4-8576-0002A516ECE8"), ProjectType.JSharp },
                { new Guid("EC05E597-79D4-47f3-ADA0-324C4F7C7484"), ProjectType.SharePointVb },
                { new Guid("EFBA0AD7-5A72-4C68-AF49-83D382785DCF"), ProjectType.XamarinAndroid },
                { new Guid("F135691A-BF7E-435D-8960-F99683D2D49C"), ProjectType.DistributedSystem },
                { new Guid("F184B08F-C81C-45F6-A57F-5ABD9991F28F"), ProjectType.VbNet },
                { new Guid("F2A71F9B-5D33-465A-A702-920D77279786"), ProjectType.FSharp },
                { new Guid("F5B4F3BC-B597-4E2B-B552-EF5D8A32436F"), ProjectType.MonoTouchBinding },
                { new Guid("F85E285D-A4E0-4152-9332-AB1D724D3325"), ProjectType.AspNetMvc2 },
                { new Guid("F8810EC1-6754-47FC-A15F-DFABD2E3FA90"), ProjectType.SharePointWorkflow },
                { new Guid("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"), ProjectType.CSharpClassLibrary }
            };
        }
    }
}
