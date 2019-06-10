// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace System.Security.Permissions 
{
    public enum MediaPermissionAudio
    {
        NoAudio,
        SiteOfOriginAudio,
        SafeAudio,
        AllAudio
    }
    public enum MediaPermissionVideo
    {
        NoVideo,
        SiteOfOriginVideo, 
        SafeVideo,
        AllVideo,
    }
    public enum MediaPermissionImage
    {
        NoImage,
        SiteOfOriginImage, 
        SafeImage,
        AllImage,
    }    
    [Serializable()]
    sealed public class MediaPermission : CodeAccessPermission, IUnrestrictedPermission 
    {
        public MediaPermission() { }    
        public MediaPermission(PermissionState state) { }        
        public MediaPermission(MediaPermissionAudio permissionAudio ) { }  
        public MediaPermission(MediaPermissionVideo permissionVideo ) { }     
        public MediaPermission(MediaPermissionImage permissionImage ) { }    
        public MediaPermission(MediaPermissionAudio permissionAudio, 
                               MediaPermissionVideo permissionVideo, 
                               MediaPermissionImage permissionImage ) { }
        public bool IsUnrestricted() { return true; }
        public override bool IsSubsetOf(IPermission target) { return false; }
        public override IPermission Intersect(IPermission target) { return default(IPermission); }
        public override IPermission Union(IPermission target) { return default(IPermission); }        
        public override IPermission Copy() { return default(IPermission); }
        public override SecurityElement ToXml() { return default(SecurityElement); }
        public override void FromXml(SecurityElement securityElement) { }
        public MediaPermissionAudio Audio { get; }
        public MediaPermissionVideo Video { get; } 
        public MediaPermissionImage Image { get; }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false )] 
    sealed public class MediaPermissionAttribute : CodeAccessSecurityAttribute 
    {      
        public MediaPermissionAttribute(SecurityAction action) : base(action) { }
        public override IPermission CreatePermission() { return default(IPermission); }     
        public MediaPermissionAudio Audio { get; set; }         
        public MediaPermissionVideo Video { get; set; }        
        public MediaPermissionImage Image { get; set; }      
    }
}
