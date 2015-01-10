Application Settings Extension Demo
-----------------------------------

This demo shows how MEF may be extended to provide parts with
settings from the application configuration file.

The scenario touches on two typical pieces of configuration data,
a username and a server URL.

Elements:

 * Program.cs - invokes the 'downloader' to run the demo.
 * Parts/Downloader.cs - uses two setting values, username and
   serverUrl.
 * App.config - provides the serverUrl setting value
 * Parts/AuthenticationProvider.cs - shows how a setting can be
   provided by a regular property export.
 * Extension/AppSettingsExportDescriptorProvider.cs - plugs in
   to the container to provide values from App.config.
 * Extension/SettingAttribute.cs - attribute that serves as
   both an import metadata constraint and an export metadata
   attribute for the purpose of annotating imports and exports
   with setting key names.

Points:

 * Settings are regular imports of contract 'string', with an
   addional constraint applied restricting the allowed values to
   those with { SettingKey, 'keyname' } export metadata applied.
 * Once a setting has been retrieved for the first time, it is
   cached as a function-to-constant-value; see:

       (c, o) => converted

   in the AppSettingsExportDescriptorProvider.