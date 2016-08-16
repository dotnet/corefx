// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that an entity member represents a data relationship, such as a foreign key relationship.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = true)]
    [System.ObsoleteAttribute("This attribute is no longer in use and will be ignored if applied.")]
    public sealed partial class AssociationAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationAttribute" />
        /// class.
        /// </summary>
        /// <param name="name">The name of the association.</param>
        /// <param name="thisKey">
        /// A comma-separated list of the property names of the key values on the <paramref name="thisKey" />
        /// side of the association.
        /// </param>
        /// <param name="otherKey">
        /// A comma-separated list of the property names of the key values on the <paramref name="otherKey" />
        /// side of the association.
        /// </param>
        public AssociationAttribute(string name, string thisKey, string otherKey) { }
        /// <summary>
        /// Gets or sets a value that indicates whether the association member represents a foreign key.
        /// </summary>
        /// <returns>
        /// true if the association represents a foreign key; otherwise, false.
        /// </returns>
        public bool IsForeignKey { get { return default(bool); } set { } }
        /// <summary>
        /// Gets the name of the association.
        /// </summary>
        /// <returns>
        /// The name of the association.
        /// </returns>
        public string Name { get { return default(string); } }
        /// <summary>
        /// Gets the property names of the key values on the OtherKey side of the association.
        /// </summary>
        /// <returns>
        /// A comma-separated list of the property names that represent the key values on the OtherKey
        /// side of the association.
        /// </returns>
        public string OtherKey { get { return default(string); } }
        /// <summary>
        /// Gets a collection of individual key members that are specified in the
        /// <see cref="OtherKey" /> property.
        /// </summary>
        /// <returns>
        /// A collection of individual key members that are specified in the
        /// <see cref="OtherKey" /> property.
        /// </returns>
        public System.Collections.Generic.IEnumerable<string> OtherKeyMembers { get { return default(System.Collections.Generic.IEnumerable<string>); } }
        /// <summary>
        /// Gets the property names of the key values on the ThisKey side of the association.
        /// </summary>
        /// <returns>
        /// A comma-separated list of the property names that represent the key values on the ThisKey side
        /// of the association.
        /// </returns>
        public string ThisKey { get { return default(string); } }
        /// <summary>
        /// Gets a collection of individual key members that are specified in the
        /// <see cref="ThisKey" /> property.
        /// </summary>
        /// <returns>
        /// A collection of individual key members that are specified in the
        /// <see cref="ThisKey" /> property.
        /// </returns>
        public System.Collections.Generic.IEnumerable<string> ThisKeyMembers { get { return default(System.Collections.Generic.IEnumerable<string>); } }
    }
    /// <summary>
    /// Provides an attribute that compares two properties.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(128), AllowMultiple = false)]
    public partial class CompareAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareAttribute" />
        /// class.
        /// </summary>
        /// <param name="otherProperty">The property to compare with the current property.</param>
        public CompareAttribute(string otherProperty) { }
        /// <summary>
        /// Gets the property to compare with the current property.
        /// </summary>
        /// <returns>
        /// The other property.
        /// </returns>
        public string OtherProperty { get { return default(string); } }
        /// <summary>
        /// Gets the display name of the other property.
        /// </summary>
        /// <returns>
        /// The display name of the other property.
        /// </returns>
        public string OtherPropertyDisplayName { get { return default(string); } }
        /// <summary>
        /// Gets a value that indicates whether the attribute requires validation context.
        /// </summary>
        /// <returns>
        /// true if the attribute requires validation context; otherwise, false.
        /// </returns>
        public override bool RequiresValidationContext { get { return default(bool); } }
        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>
        /// The formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name) { return default(string); }
        /// <summary>
        /// Determines whether a specified object is valid.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <param name="validationContext">An object that contains information about the validation request.</param>
        /// <returns>
        /// true if <paramref name="value" /> is valid; otherwise, false.
        /// </returns>
        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext) { return default(System.ComponentModel.DataAnnotations.ValidationResult); }
    }
    /// <summary>
    /// Specifies that a property participates in optimistic concurrency checks.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = true)]
    public sealed partial class ConcurrencyCheckAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ConcurrencyCheckAttribute" /> class.
        /// </summary>
        public ConcurrencyCheckAttribute() { }
    }
    /// <summary>
    /// Specifies that a data field value is a credit card number.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public sealed partial class CreditCardAttribute : System.ComponentModel.DataAnnotations.DataTypeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardAttribute" />
        /// class.
        /// </summary>
        public CreditCardAttribute() : base(default(System.ComponentModel.DataAnnotations.DataType)) { }
        /// <summary>
        /// Determines whether the specified credit card number is valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>
        /// true if the credit card number is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Specifies a custom validation method that is used to validate a property or class instance.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2500), AllowMultiple = true)]
    public sealed partial class CustomValidationAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CustomValidationAttribute" /> class.
        /// </summary>
        /// <param name="validatorType">The type that contains the method that performs custom validation.</param>
        /// <param name="method">The method that performs custom validation.</param>
        public CustomValidationAttribute(System.Type validatorType, string method) { }
        /// <summary>
        /// Gets the validation method.
        /// </summary>
        /// <returns>
        /// The name of the validation method.
        /// </returns>
        public string Method { get { return default(string); } }
        /// <summary>
        /// Gets the type that performs custom validation.
        /// </summary>
        /// <returns>
        /// The type that performs custom validation.
        /// </returns>
        public System.Type ValidatorType { get { return default(System.Type); } }
        /// <summary>
        /// Formats a validation error message.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>
        /// An instance of the formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name) { return default(string); }
        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext) { return default(System.ComponentModel.DataAnnotations.ValidationResult); }
    }
    /// <summary>
    /// Represents an enumeration of the data types associated with data fields and parameters.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Represents a credit card number.
        /// </summary>
        CreditCard = 14,
        /// <summary>
        /// Represents a currency value.
        /// </summary>
        Currency = 6,
        /// <summary>
        /// Represents a custom data type.
        /// </summary>
        Custom = 0,
        /// <summary>
        /// Represents a date value.
        /// </summary>
        Date = 2,
        /// <summary>
        /// Represents an instant in time, expressed as a date and time of day.
        /// </summary>
        DateTime = 1,
        /// <summary>
        /// Represents a continuous time during which an object exists.
        /// </summary>
        Duration = 4,
        /// <summary>
        /// Represents an e-mail address.
        /// </summary>
        EmailAddress = 10,
        /// <summary>
        /// Represents an HTML file.
        /// </summary>
        Html = 8,
        /// <summary>
        /// Represents a URL to an image.
        /// </summary>
        ImageUrl = 13,
        /// <summary>
        /// Represents multi-line text.
        /// </summary>
        MultilineText = 9,
        /// <summary>
        /// Represent a password value.
        /// </summary>
        Password = 11,
        /// <summary>
        /// Represents a phone number value.
        /// </summary>
        PhoneNumber = 5,
        /// <summary>
        /// Represents a postal code.
        /// </summary>
        PostalCode = 15,
        /// <summary>
        /// Represents text that is displayed.
        /// </summary>
        Text = 7,
        /// <summary>
        /// Represents a time value.
        /// </summary>
        Time = 3,
        /// <summary>
        /// Represents file upload data type.
        /// </summary>
        Upload = 16,
        /// <summary>
        /// Represents a URL value.
        /// </summary>
        Url = 12,
    }
    /// <summary>
    /// Specifies the name of an additional type to associate with a data field.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2496), AllowMultiple = false)]
    public partial class DataTypeAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeAttribute" />
        /// class by using the specified type name.
        /// </summary>
        /// <param name="dataType">The name of the type to associate with the data field.</param>
        public DataTypeAttribute(System.ComponentModel.DataAnnotations.DataType dataType) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeAttribute" />
        /// class by using the specified field template name.
        /// </summary>
        /// <param name="customDataType">The name of the custom field template to associate with the data field.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="customDataType" /> is null or an empty string ("").
        /// </exception>
        public DataTypeAttribute(string customDataType) { }
        /// <summary>
        /// Gets the name of custom field template that is associated with the data field.
        /// </summary>
        /// <returns>
        /// The name of the custom field template that is associated with the data field.
        /// </returns>
        public string CustomDataType { get { return default(string); } }
        /// <summary>
        /// Gets the type that is associated with the data field.
        /// </summary>
        /// <returns>
        /// One of the <see cref="DataAnnotations.DataType" /> values.
        /// </returns>
        public System.ComponentModel.DataAnnotations.DataType DataType { get { return default(System.ComponentModel.DataAnnotations.DataType); } }
        /// <summary>
        /// Gets a data-field display format.
        /// </summary>
        /// <returns>
        /// The data-field display format.
        /// </returns>
        public System.ComponentModel.DataAnnotations.DisplayFormatAttribute DisplayFormat { get { return default(System.ComponentModel.DataAnnotations.DisplayFormatAttribute); } protected set { } }
        /// <summary>
        /// Returns the name of the type that is associated with the data field.
        /// </summary>
        /// <returns>
        /// The name of the type associated with the data field.
        /// </returns>
        public virtual string GetDataTypeName() { return default(string); }
        /// <summary>
        /// Checks that the value of the data field is valid.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>
        /// true always.
        /// </returns>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Provides a general-purpose attribute that lets you specify localizable strings for types and
    /// members of entity partial classes.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2496), AllowMultiple = false)]
    public sealed partial class DisplayAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAttribute" />
        /// class.
        /// </summary>
        public DisplayAttribute() { }
        /// <summary>
        /// Gets or sets a value that indicates whether UI should be generated automatically in order to
        /// display this field.
        /// </summary>
        /// <returns>
        /// true if UI should be generated automatically to display this field; otherwise, false.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// An attempt was made to get the property value before it was set.
        /// </exception>
        public bool AutoGenerateField { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets a value that indicates whether filtering UI is automatically displayed for this
        /// field.
        /// </summary>
        /// <returns>
        /// true if UI should be generated automatically to display filtering for this field; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// An attempt was made to get the property value before it was set.
        /// </exception>
        public bool AutoGenerateFilter { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets a value that is used to display a description in the UI.
        /// </summary>
        /// <returns>
        /// The value that is used to display a description in the UI.
        /// </returns>
        public string Description { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value that is used to group fields in the UI.
        /// </summary>
        /// <returns>
        /// A value that is used to group fields in the UI.
        /// </returns>
        public string GroupName { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value that is used for display in the UI.
        /// </summary>
        /// <returns>
        /// A value that is used for display in the UI.
        /// </returns>
        public string Name { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the order weight of the column.
        /// </summary>
        /// <returns>
        /// The order weight of the column.
        /// </returns>
        public int Order { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets a value that will be used to set the watermark for prompts in the UI.
        /// </summary>
        /// <returns>
        /// A value that will be used to display a watermark in the UI.
        /// </returns>
        public string Prompt { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the type that contains the resources for the
        /// <see cref="ShortName" />, <see cref="Name" />,
        /// <see cref="Prompt" />, and <see cref="Description" />
        /// properties.
        /// </summary>
        /// <returns>
        /// The type of the resource that contains the
        /// <see cref="ShortName" />, <see cref="Name" />,
        /// <see cref="Prompt" />, and <see cref="Description" />
        /// properties.
        /// </returns>
        public System.Type ResourceType { get { return default(System.Type); } set { } }
        /// <summary>
        /// Gets or sets a value that is used for the grid column label.
        /// </summary>
        /// <returns>
        /// A value that is for the grid column label.
        /// </returns>
        public string ShortName { get { return default(string); } set { } }
        /// <summary>
        /// Returns the value of the
        /// <see cref="AutoGenerateField" /> property.
        /// </summary>
        /// <returns>
        /// The value of <see cref="AutoGenerateField" />
        /// if the property has been initialized; otherwise, null.
        /// </returns>
        public System.Nullable<bool> GetAutoGenerateField() { return default(System.Nullable<bool>); }
        /// <summary>
        /// Returns a value that indicates whether UI should be generated automatically in order to display
        /// filtering for this field.
        /// </summary>
        /// <returns>
        /// The value of <see cref="AutoGenerateFilter" />
        /// if the property has been initialized; otherwise, null.
        /// </returns>
        public System.Nullable<bool> GetAutoGenerateFilter() { return default(System.Nullable<bool>); }
        /// <summary>
        /// Returns the value of the <see cref="Description" />
        /// property.
        /// </summary>
        /// <returns>
        /// The localized description, if the
        /// <see cref="ResourceType" /> has been specified and the
        /// <see cref="Description" /> property represents a resource key; otherwise, the non-localized value of the
        /// <see cref="Description" /> property.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="ResourceType" /> property
        /// and the <see cref="Description" />
        /// property are initialized, but a public static property that has a name that matches the
        /// <see cref="Description" /> value could
        /// not be found for the <see cref="ResourceType" />
        /// property.
        /// </exception>
        public string GetDescription() { return default(string); }
        /// <summary>
        /// Returns the value of the <see cref="GroupName" />
        /// property.
        /// </summary>
        /// <returns>
        /// A value that will be used for grouping fields in the UI, if
        /// <see cref="GroupName" /> has been initialized; otherwise, null. If the
        /// <see cref="ResourceType" /> property has been specified and the
        /// <see cref="GroupName" /> property represents a resource key, a localized string is returned; otherwise, a non-localized
        /// string is returned.
        /// </returns>
        public string GetGroupName() { return default(string); }
        /// <summary>
        /// Returns a value that is used for field display in the UI.
        /// </summary>
        /// <returns>
        /// The localized string for the <see cref="Name" />
        /// property, if the <see cref="ResourceType" />
        /// property has been specified and the
        /// <see cref="Name" /> property represents a resource key; otherwise, the non-localized value of the
        /// <see cref="Name" /> property.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="ResourceType" /> property
        /// and the <see cref="Name" /> property
        /// are initialized, but a public static property that has a name that matches the
        /// <see cref="Name" /> value could not be found for the
        /// <see cref="ResourceType" /> property.
        /// </exception>
        public string GetName() { return default(string); }
        /// <summary>
        /// Returns the value of the <see cref="Order" />
        /// property.
        /// </summary>
        /// <returns>
        /// The value of the <see cref="Order" />
        /// property, if it has been set; otherwise, null.
        /// </returns>
        public System.Nullable<int> GetOrder() { return default(System.Nullable<int>); }
        /// <summary>
        /// Returns the value of the <see cref="Prompt" />
        /// property.
        /// </summary>
        /// <returns>
        /// Gets the localized string for the
        /// <see cref="Prompt" /> property if the
        /// <see cref="ResourceType" /> property has been specified and if the
        /// <see cref="Prompt" /> property represents a resource key; otherwise, the non-localized value of the
        /// <see cref="Prompt" /> property.
        /// </returns>
        public string GetPrompt() { return default(string); }
        /// <summary>
        /// Returns the value of the <see cref="ShortName" />
        /// property.
        /// </summary>
        /// <returns>
        /// The localized string for the <see cref="ShortName" />
        /// property if the <see cref="ResourceType" />
        /// property has been specified and if the
        /// <see cref="ShortName" /> property represents a resource key; otherwise, the non-localized value of the
        /// <see cref="ShortName" /> value property.
        /// </returns>
        public string GetShortName() { return default(string); }
    }
    /// <summary>
    /// Specifies the column that is displayed in the referred table as a foreign-key column.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4), Inherited = true, AllowMultiple = false)]
    public partial class DisplayColumnAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayColumnAttribute" />
        /// class by using the specified column.
        /// </summary>
        /// <param name="displayColumn">The name of the column to use as the display column.</param>
        public DisplayColumnAttribute(string displayColumn) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayColumnAttribute" />
        /// class by using the specified display and sort columns.
        /// </summary>
        /// <param name="displayColumn">The name of the column to use as the display column.</param>
        /// <param name="sortColumn">The name of the column to use for sorting.</param>
        public DisplayColumnAttribute(string displayColumn, string sortColumn) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayColumnAttribute" />
        /// class by using the specified display column, and the specified sort column and sort order.
        /// </summary>
        /// <param name="displayColumn">The name of the column to use as the display column.</param>
        /// <param name="sortColumn">The name of the column to use for sorting.</param>
        /// <param name="sortDescending">true to sort in descending order; otherwise, false. The default is false.</param>
        public DisplayColumnAttribute(string displayColumn, string sortColumn, bool sortDescending) { }
        /// <summary>
        /// Gets the name of the column to use as the display field.
        /// </summary>
        /// <returns>
        /// The name of the display column.
        /// </returns>
        public string DisplayColumn { get { return default(string); } }
        /// <summary>
        /// Gets the name of the column to use for sorting.
        /// </summary>
        /// <returns>
        /// The name of the sort column.
        /// </returns>
        public string SortColumn { get { return default(string); } }
        /// <summary>
        /// Gets a value that indicates whether to sort in descending or ascending order.
        /// </summary>
        /// <returns>
        /// true if the column will be sorted in descending order; otherwise, false.
        /// </returns>
        public bool SortDescending { get { return default(bool); } }
    }
    /// <summary>
    /// Specifies how data fields are displayed and formatted by ASP.NET Dynamic Data.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public partial class DisplayFormatAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayFormatAttribute" />
        /// class.
        /// </summary>
        public DisplayFormatAttribute() { }
        /// <summary>
        /// Gets or sets a value that indicates whether the formatting string that is specified by the
        /// <see cref="DataFormatString" />
        /// property is applied to the field value when the data field is in edit mode.
        /// </summary>
        /// <returns>
        /// true if the formatting string applies to the field value in edit mode; otherwise, false. The
        /// default is false.
        /// </returns>
        public bool ApplyFormatInEditMode { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets a value that indicates whether empty string values ("") are automatically converted
        /// to null when the data field is updated in the data source.
        /// </summary>
        /// <returns>
        /// true if empty string values are automatically converted to null; otherwise, false. The default
        /// is true.
        /// </returns>
        public bool ConvertEmptyStringToNull { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the display format for the field value.
        /// </summary>
        /// <returns>
        /// A formatting string that specifies the display format for the value of the data field. The
        /// default is an empty string (""), which indicates that no special formatting is applied to the field
        /// value.
        /// </returns>
        public string DataFormatString { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value that indicates whether the field should be HTML-encoded.
        /// </summary>
        /// <returns>
        /// true if the field should be HTML-encoded; otherwise, false.
        /// </returns>
        public bool HtmlEncode { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the text that is displayed for a field when the field's value is null.
        /// </summary>
        /// <returns>
        /// The text that is displayed for a field when the field's value is null. The default is an empty
        /// string (""), which indicates that this property is not set.
        /// </returns>
        public string NullDisplayText { get { return default(string); } set { } }
    }
    /// <summary>
    /// Indicates whether a data field is editable.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = true)]
    public sealed partial class EditableAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditableAttribute" />
        /// class.
        /// </summary>
        /// <param name="allowEdit">true to specify that field is editable; otherwise, false.</param>
        public EditableAttribute(bool allowEdit) { }
        /// <summary>
        /// Gets a value that indicates whether a field is editable.
        /// </summary>
        /// <returns>
        /// true if the field is editable; otherwise, false.
        /// </returns>
        public bool AllowEdit { get { return default(bool); } }
        /// <summary>
        /// Gets or sets a value that indicates whether an initial value is enabled.
        /// </summary>
        /// <returns>
        /// true if an initial value is enabled; otherwise, false.
        /// </returns>
        public bool AllowInitialValue { get { return default(bool); } set { } }
    }
    /// <summary>
    /// Validates an email address.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public sealed partial class EmailAddressAttribute : System.ComponentModel.DataAnnotations.DataTypeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressAttribute" />
        /// class.
        /// </summary>
        public EmailAddressAttribute() : base(default(System.ComponentModel.DataAnnotations.DataType)) { }
        /// <summary>
        /// Determines whether the specified value matches the pattern of a valid email address.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>
        /// true if the specified value is valid or null; otherwise, false.
        /// </returns>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Enables a .NET Framework enumeration to be mapped to a data column.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2496), AllowMultiple = false)]
    public sealed partial class EnumDataTypeAttribute : System.ComponentModel.DataAnnotations.DataTypeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumDataTypeAttribute" />
        /// class.
        /// </summary>
        /// <param name="enumType">The type of the enumeration.</param>
        public EnumDataTypeAttribute(System.Type enumType) : base(default(System.ComponentModel.DataAnnotations.DataType)) { }
        /// <summary>
        /// Gets or sets the enumeration type.
        /// </summary>
        /// <returns>
        /// The enumeration type.
        /// </returns>
        public System.Type EnumType { get { return default(System.Type); } }
        /// <summary>
        /// Checks that the value of the data field is valid.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>
        /// true if the data field value is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Validates file name extensions.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public sealed partial class FileExtensionsAttribute : System.ComponentModel.DataAnnotations.DataTypeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="FileExtensionsAttribute" /> class.
        /// </summary>
        public FileExtensionsAttribute() : base(default(System.ComponentModel.DataAnnotations.DataType)) { }
        /// <summary>
        /// Gets or sets the file name extensions.
        /// </summary>
        /// <returns>
        /// The file name extensions, or the default file extensions (".png", ".jpg", ".jpeg", and ".gif")
        /// if the property is not set.
        /// </returns>
        public string Extensions { get { return default(string); } set { } }
        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>
        /// The formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name) { return default(string); }
        /// <summary>
        /// Checks that the specified file name extension or extensions is valid.
        /// </summary>
        /// <param name="value">A comma delimited list of valid file extensions.</param>
        /// <returns>
        /// true if the file name extension is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Represents an attribute that is used to specify the filtering behavior for a column.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    [System.ObsoleteAttribute("This attribute is no longer in use and will be ignored if applied.")]
    public sealed partial class FilterUIHintAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterUIHintAttribute" />
        /// class by using the filter UI hint.
        /// </summary>
        /// <param name="filterUIHint">The name of the control to use for filtering.</param>
        public FilterUIHintAttribute(string filterUIHint) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterUIHintAttribute" />
        /// class by using the filter UI hint and presentation layer name.
        /// </summary>
        /// <param name="filterUIHint">The name of the control to use for filtering.</param>
        /// <param name="presentationLayer">The name of the presentation layer that supports this control.</param>
        public FilterUIHintAttribute(string filterUIHint, string presentationLayer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterUIHintAttribute" />
        /// class by using the filter UI hint, presentation layer name, and control parameters.
        /// </summary>
        /// <param name="filterUIHint">The name of the control to use for filtering.</param>
        /// <param name="presentationLayer">The name of the presentation layer that supports this control.</param>
        /// <param name="controlParameters">The list of parameters for the control.</param>
        public FilterUIHintAttribute(string filterUIHint, string presentationLayer, params object[] controlParameters) { }
        /// <summary>
        /// Gets the name/value pairs that are used as parameters in the control's constructor.
        /// </summary>
        /// <returns>
        /// The name/value pairs that are used as parameters in the control's constructor.
        /// </returns>
        public System.Collections.Generic.IDictionary<string, object> ControlParameters { get { return default(System.Collections.Generic.IDictionary<string, object>); } }
        /// <summary>
        /// Gets the name of the control to use for filtering.
        /// </summary>
        /// <returns>
        /// The name of the control to use for filtering.
        /// </returns>
        public string FilterUIHint { get { return default(string); } }
        /// <summary>
        /// Gets the name of the presentation layer that supports this control.
        /// </summary>
        /// <returns>
        /// The name of the presentation layer that supports this control.
        /// </returns>
        public string PresentationLayer { get { return default(string); } }
        /// <summary>
        /// Returns a value that indicates whether this attribute instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this attribute instance.</param>
        /// <returns>
        /// True if the passed object is equal to this attribute instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this attribute instance.
        /// </summary>
        /// <returns>
        /// This attribute insatnce hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
    }
    /// <summary>
    /// Provides a way for an object to be invalidated.
    /// </summary>
    public partial interface IValidatableObject
    {
        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        System.Collections.Generic.IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext);
    }
    /// <summary>
    /// Denotes one or more properties that uniquely identify an entity.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = true)]
    public sealed partial class KeyAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAttribute" />
        /// class.
        /// </summary>
        public KeyAttribute() { }
    }
    /// <summary>
    /// Specifies the maximum length of array or string data allowed in a property.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public partial class MaxLengthAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLengthAttribute" />
        /// class.
        /// </summary>
        public MaxLengthAttribute() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLengthAttribute" />
        /// class based on the <paramref name="length" /> parameter.
        /// </summary>
        /// <param name="length">The maximum allowable length of array or string data.</param>
        public MaxLengthAttribute(int length) { }
        /// <summary>
        /// Gets the maximum allowable length of the array or string data.
        /// </summary>
        /// <returns>
        /// The maximum allowable length of the array or string data.
        /// </returns>
        public int Length { get { return default(int); } }
        /// <summary>
        /// Applies formatting to a specified error message.
        /// </summary>
        /// <param name="name">The name to include in the formatted string.</param>
        /// <returns>
        /// A localized string to describe the maximum acceptable length.
        /// </returns>
        public override string FormatErrorMessage(string name) { return default(string); }
        /// <summary>
        /// Determines whether a specified object is valid.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns>
        /// true if the value is null, or if the value is less than or equal to the specified maximum length;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="InvalidOperationException">Length is zero or less than negative one.</exception>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Specifies the minimum length of array or string data allowed in a property.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public partial class MinLengthAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinLengthAttribute" />
        /// class.
        /// </summary>
        /// <param name="length">The length of the array or string data.</param>
        public MinLengthAttribute(int length) { }
        /// <summary>
        /// Gets or sets the minimum allowable length of the array or string data.
        /// </summary>
        /// <returns>
        /// The minimum allowable length of the array or string data.
        /// </returns>
        public int Length { get { return default(int); } }
        /// <summary>
        /// Applies formatting to a specified error message.
        /// </summary>
        /// <param name="name">The name to include in the formatted string.</param>
        /// <returns>
        /// A localized string to describe the minimum acceptable length.
        /// </returns>
        public override string FormatErrorMessage(string name) { return default(string); }
        /// <summary>
        /// Determines whether a specified object is valid.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns>
        /// true if the specified object is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Specifies that a data field value is a  well-formed phone number using a regular expression
    /// for phone numbers.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public sealed partial class PhoneAttribute : System.ComponentModel.DataAnnotations.DataTypeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneAttribute" />
        /// class.
        /// </summary>
        public PhoneAttribute() : base(default(System.ComponentModel.DataAnnotations.DataType)) { }
        /// <summary>
        /// Determines whether the specified phone number is in a valid phone number format.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>
        /// true if the phone number is valid; otherwise, false.
        /// </returns>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Specifies the numeric range constraints for the value of a data field.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public partial class RangeAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute" />
        /// class by using the specified minimum and maximum values.
        /// </summary>
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
        public RangeAttribute(double minimum, double maximum) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute" />
        /// class by using the specified minimum and maximum values.
        /// </summary>
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
        public RangeAttribute(int minimum, int maximum) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeAttribute" />
        /// class by using the specified minimum and maximum values and the specific type.
        /// </summary>
        /// <param name="type">Specifies the type of the object to test.</param>
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type" /> is null.</exception>
        public RangeAttribute(System.Type type, string minimum, string maximum) { }
        /// <summary>
        /// Gets the maximum allowed field value.
        /// </summary>
        /// <returns>
        /// The maximum value that is allowed for the data field.
        /// </returns>
        public object Maximum { get { return default(object); } }
        /// <summary>
        /// Gets the minimum allowed field value.
        /// </summary>
        /// <returns>
        /// The minimu value that is allowed for the data field.
        /// </returns>
        public object Minimum { get { return default(object); } }
        /// <summary>
        /// Gets the type of the data field whose value must be validated.
        /// </summary>
        /// <returns>
        /// The type of the data field whose value must be validated.
        /// </returns>
        public System.Type OperandType { get { return default(System.Type); } }
        /// <summary>
        /// Formats the error message that is displayed when range validation fails.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>
        /// The formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name) { return default(string); }
        /// <summary>
        /// Checks that the value of the data field is in the specified range.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>
        /// true if the specified value is in the range; otherwise, false.
        /// </returns>
        /// <exception cref="ValidationException">
        /// The data field value was outside the allowed range.
        /// </exception>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Specifies that a data field value in ASP.NET Dynamic Data must match the specified regular
    /// expression.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public partial class RegularExpressionAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RegularExpressionAttribute" /> class.
        /// </summary>
        /// <param name="pattern">The regular expression that is used to validate the data field value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern" /> is null.</exception>
        public RegularExpressionAttribute(string pattern) { }
        /// <summary>
        /// Gets or set the amount of time in milliseconds to execute a single matching operation before
        /// the operation times out.
        /// </summary>
        /// <returns>
        /// The amount of time in milliseconds to execute a single matching operation.
        /// </returns>
        public int MatchTimeoutInMilliseconds { get { return default(int); } set { } }
        /// <summary>
        /// Gets the regular expression pattern.
        /// </summary>
        /// <returns>
        /// The pattern to match.
        /// </returns>
        public string Pattern { get { return default(string); } }
        /// <summary>
        /// Formats the error message to display if the regular expression validation fails.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>
        /// The formatted error message.
        /// </returns>
        public override string FormatErrorMessage(string name) { return default(string); }
        /// <summary>
        /// Checks whether the value entered by the user matches the regular expression pattern.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>
        /// true if validation is successful; otherwise, false.
        /// </returns>
        /// <exception cref="ValidationException">
        /// The data field value did not match the regular expression pattern.
        /// </exception>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Specifies that a data field value is required.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public partial class RequiredAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredAttribute" />
        /// class.
        /// </summary>
        public RequiredAttribute() { }
        /// <summary>
        /// Gets or sets a value that indicates whether an empty string is allowed.
        /// </summary>
        /// <returns>
        /// true if an empty string is allowed; otherwise, false. The default value is false.
        /// </returns>
        public bool AllowEmptyStrings { get { return default(bool); } set { } }
        /// <summary>
        /// Checks that the value of the required data field is not empty.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>
        /// true if validation is successful; otherwise, false.
        /// </returns>
        /// <exception cref="ValidationException">
        /// The data field value was null.
        /// </exception>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Specifies whether a class or data column uses scaffolding.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public partial class ScaffoldColumnAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ScaffoldColumnAttribute" />
        /// using the <see cref="Scaffold" />
        /// property.
        /// </summary>
        /// <param name="scaffold">The value that specifies whether scaffolding is enabled.</param>
        public ScaffoldColumnAttribute(bool scaffold) { }
        /// <summary>
        /// Gets or sets the value that specifies whether scaffolding is enabled.
        /// </summary>
        /// <returns>
        /// true, if scaffolding is enabled; otherwise false.
        /// </returns>
        public bool Scaffold { get { return default(bool); } }
    }
    /// <summary>
    /// Specifies the minimum and maximum length of characters that are allowed in a data field.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public partial class StringLengthAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthAttribute" />
        /// class by using a specified maximum length.
        /// </summary>
        /// <param name="maximumLength">The maximum length of a string.</param>
        public StringLengthAttribute(int maximumLength) { }
        /// <summary>
        /// Gets or sets the maximum length of a string.
        /// </summary>
        /// <returns>
        /// The maximum length a string.
        /// </returns>
        public int MaximumLength { get { return default(int); } }
        /// <summary>
        /// Gets or sets the minimum length of a string.
        /// </summary>
        /// <returns>
        /// The minimum length of a string.
        /// </returns>
        public int MinimumLength { get { return default(int); } set { } }
        /// <summary>
        /// Applies formatting to a specified error message.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>
        /// The formatted error message.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maximumLength" /> is negative. -or-<paramref name="maximumLength" /> is less
        /// than <paramref name="minimumLength" />.
        /// </exception>
        public override string FormatErrorMessage(string name) { return default(string); }
        /// <summary>
        /// Determines whether a specified object is valid.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns>
        /// true if the specified object is valid; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maximumLength" /> is negative.-or-<paramref name="maximumLength" /> is less
        /// than <see cref="MinimumLength" />.
        /// </exception>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Specifies the data type of the column as a row version.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = true)]
    public sealed partial class TimestampAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimestampAttribute" />
        /// class.
        /// </summary>
        public TimestampAttribute() { }
    }
    /// <summary>
    /// Specifies the template or user control that Dynamic Data uses to display a data field.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = true)]
    public partial class UIHintAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIHintAttribute" />
        /// class by using a specified user control.
        /// </summary>
        /// <param name="uiHint">The user control to use to display the data field.</param>
        public UIHintAttribute(string uiHint) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UIHintAttribute" />
        /// class using the specified user control and specified presentation layer.
        /// </summary>
        /// <param name="uiHint">The user control (field template) to use to display the data field.</param>
        /// <param name="presentationLayer">
        /// The presentation layer that uses the class. Can be set to "HTML", "Silverlight", "WPF", or
        /// "WinForms".
        /// </param>
        public UIHintAttribute(string uiHint, string presentationLayer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UIHintAttribute" />
        /// class by using the specified user control, presentation layer, and control parameters.
        /// </summary>
        /// <param name="uiHint">The user control (field template) to use to display the data field.</param>
        /// <param name="presentationLayer">
        /// The presentation layer that uses the class. Can be set to "HTML", "Silverlight", "WPF", or
        /// "WinForms".
        /// </param>
        /// <param name="controlParameters">The object to use to retrieve values from any data sources.</param>
        /// <exception cref="ArgumentException">
        /// <see cref="ControlParameters" /> is
        /// null or it is a constraint key.-or-The value of
        /// <see cref="ControlParameters" /> is not a string.
        /// </exception>
        public UIHintAttribute(string uiHint, string presentationLayer, params object[] controlParameters) { }
        /// <summary>
        /// Gets or sets the <see cref="System.Web.DynamicData.DynamicControlParameter" /> object to
        /// use to retrieve values from any data source.
        /// </summary>
        /// <returns>
        /// A collection of key/value pairs.
        /// </returns>
        public System.Collections.Generic.IDictionary<string, object> ControlParameters { get { return default(System.Collections.Generic.IDictionary<string, object>); } }
        /// <summary>
        /// Gets or sets the presentation layer that uses the
        /// <see cref="UIHintAttribute" /> class.
        /// </summary>
        /// <returns>
        /// The presentation layer that is used by this class.
        /// </returns>
        public string PresentationLayer { get { return default(string); } }
        /// <summary>
        /// Gets or sets the name of the field template to use to display the data field.
        /// </summary>
        /// <returns>
        /// The name of the field template that displays the data field.
        /// </returns>
        public string UIHint { get { return default(string); } }
        /// <summary>
        /// Gets a value that indicates whether this instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance, or a null reference.</param>
        /// <returns>
        /// true if the specified object is equal to this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Gets the hash code for the current instance of the attribute.
        /// </summary>
        /// <returns>
        /// The attribute instance hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
    }
    /// <summary>
    /// Provides URL validation.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(2432), AllowMultiple = false)]
    public sealed partial class UrlAttribute : System.ComponentModel.DataAnnotations.DataTypeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlAttribute" />
        /// class.
        /// </summary>
        public UrlAttribute() : base(default(System.ComponentModel.DataAnnotations.DataType)) { }
        /// <summary>
        /// Validates the format of the specified URL.
        /// </summary>
        /// <param name="value">The URL to validate.</param>
        /// <returns>
        /// true if the URL format is valid or null; otherwise, false.
        /// </returns>
        public override bool IsValid(object value) { return default(bool); }
    }
    /// <summary>
    /// Serves as the base class for all validation attributes.
    /// </summary>
    /// <exception cref="ValidationException">
    /// The <see cref="ErrorMessageResourceType" />
    /// and <see cref="ErrorMessageResourceName" />
    /// properties for localized error message are set at the same time that the non-localized
    /// <see cref="ErrorMessage" /> property
    /// error message is set.
    /// </exception>
    public abstract partial class ValidationAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttribute" />
        /// class.
        /// </summary>
        protected ValidationAttribute() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttribute" />
        /// class by using the function that enables access to validation resources.
        /// </summary>
        /// <param name="errorMessageAccessor">The function that enables access to validation resources.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessageAccessor" /> is null.
        /// </exception>
        protected ValidationAttribute(System.Func<string> errorMessageAccessor) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttribute" />
        /// class by using the error message to associate with a validation control.
        /// </summary>
        /// <param name="errorMessage">The error message to associate with a validation control.</param>
        protected ValidationAttribute(string errorMessage) { }
        /// <summary>
        /// Gets or sets an error message to associate with a validation control if validation fails.
        /// </summary>
        /// <returns>
        /// The error message that is associated with the validation control.
        /// </returns>
        public string ErrorMessage { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the error message resource name to use in order to look up the
        /// <see cref="ErrorMessageResourceType" /> property value if validation fails.
        /// </summary>
        /// <returns>
        /// The error message resource that is associated with a validation control.
        /// </returns>
        public string ErrorMessageResourceName { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the resource type to use for error-message lookup if validation fails.
        /// </summary>
        /// <returns>
        /// The type of error message that is associated with a validation control.
        /// </returns>
        public System.Type ErrorMessageResourceType { get { return default(System.Type); } set { } }
        /// <summary>
        /// Gets the localized validation error message.
        /// </summary>
        /// <returns>
        /// The localized validation error message.
        /// </returns>
        protected string ErrorMessageString { get { return default(string); } }
        /// <summary>
        /// Gets a value that indicates whether the attribute requires validation context.
        /// </summary>
        /// <returns>
        /// true if the attribute requires validation context; otherwise, false.
        /// </returns>
        public virtual bool RequiresValidationContext { get { return default(bool); } }
        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>
        /// An instance of the formatted error message.
        /// </returns>
        public virtual string FormatErrorMessage(string name) { return default(string); }
        /// <summary>
        /// Checks whether the specified value is valid with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="ValidationResult" />
        /// class.
        /// </returns>
        public System.ComponentModel.DataAnnotations.ValidationResult GetValidationResult(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext) { return default(System.ComponentModel.DataAnnotations.ValidationResult); }
        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        public virtual bool IsValid(object value) { return default(bool); }
        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="ValidationResult" />
        /// class.
        /// </returns>
        protected virtual System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext) { return default(System.ComponentModel.DataAnnotations.ValidationResult); }
        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <param name="validationContext">
        /// The <see cref="ValidationContext" /> object that describes
        /// the context where the validation checks are performed. This parameter cannot be null.
        /// </param>
        /// <exception cref="ValidationException">Validation failed.</exception>
        public void Validate(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext) { }
        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="name">The name to include in the error message.</param>
        /// <exception cref="ValidationException">
        /// <paramref name="value" /> is not valid.
        /// </exception>
        public void Validate(object value, string name) { }
    }
    /// <summary>
    /// Describes the context in which a validation check is performed.
    /// </summary>
    public sealed partial class ValidationContext : System.IServiceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext" />
        /// class using the specified object instance
        /// </summary>
        /// <param name="instance">The object instance to validate. It cannot be null.</param>
        public ValidationContext(object instance) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext" />
        /// class using the specified object and an optional property bag.
        /// </summary>
        /// <param name="instance">The object instance to validate.  It cannot be null</param>
        /// <param name="items">An optional set of key/value pairs to make available to consumers.</param>
        public ValidationContext(object instance, System.Collections.Generic.IDictionary<object, object> items) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext" />
        /// class using the service provider and dictionary of service consumers.
        /// </summary>
        /// <param name="instance">The object to validate. This parameter is required.</param>
        /// <param name="serviceProvider">
        /// The object that implements the <see cref="IServiceProvider" /> interface. This parameter
        /// is optional.
        /// </param>
        /// <param name="items">
        /// A dictionary of key/value pairs to make available to the service consumers. This parameter
        /// is optional.
        /// </param>
        public ValidationContext(object instance, System.IServiceProvider serviceProvider, System.Collections.Generic.IDictionary<object, object> items) { }
        /// <summary>
        /// Gets or sets the name of the member to validate.
        /// </summary>
        /// <returns>
        /// The name of the member to validate.
        /// </returns>
        public string DisplayName { get { return default(string); } set { } }
        /// <summary>
        /// Gets the dictionary of key/value pairs that is associated with this context.
        /// </summary>
        /// <returns>
        /// The dictionary of the key/value pairs for this context.
        /// </returns>
        public System.Collections.Generic.IDictionary<object, object> Items { get { return default(System.Collections.Generic.IDictionary<object, object>); } }
        /// <summary>
        /// Gets or sets the name of the member to validate.
        /// </summary>
        /// <returns>
        /// The name of the member to validate.
        /// </returns>
        public string MemberName { get { return default(string); } set { } }
        /// <summary>
        /// Gets the object to validate.
        /// </summary>
        /// <returns>
        /// The object to validate.
        /// </returns>
        public object ObjectInstance { get { return default(object); } }
        /// <summary>
        /// Gets the type of the object to validate.
        /// </summary>
        /// <returns>
        /// The type of the object to validate.
        /// </returns>
        public System.Type ObjectType { get { return default(System.Type); } }
        /// <summary>
        /// Returns the service that provides custom validation.
        /// </summary>
        /// <param name="serviceType">The type of the service to use for validation.</param>
        /// <returns>
        /// An instance of the service, or null if the service is not available.
        /// </returns>
        public object GetService(System.Type serviceType) { return default(object); }
        /// <summary>
        /// Initializes the <see cref="ValidationContext" /> using
        /// a service provider that can return service instances by type when GetService is called.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public void InitializeServiceProvider(System.Func<System.Type, object> serviceProvider) { }
    }
    /// <summary>
    /// Represents the exception that occurs during validation of a data field when the
    /// <see cref="DataAnnotations.ValidationAttribute" /> class is used.
    /// </summary>
    public partial class ValidationException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" />
        /// class using an error message generated by the system.
        /// </summary>
        public ValidationException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" />
        /// class by using a validation result, a validation attribute, and the value of the current
        /// exception.
        /// </summary>
        /// <param name="validationResult">The list of validation results.</param>
        /// <param name="validatingAttribute">The attribute that caused the current exception.</param>
        /// <param name="value">The value of the object that caused the attribute to trigger the validation error.</param>
        public ValidationException(System.ComponentModel.DataAnnotations.ValidationResult validationResult, System.ComponentModel.DataAnnotations.ValidationAttribute validatingAttribute, object value) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" />
        /// class using a specified error message.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        public ValidationException(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" />
        /// class using a specified error message, a validation attribute, and the value of the current
        /// exception.
        /// </summary>
        /// <param name="errorMessage">The message that states the error.</param>
        /// <param name="validatingAttribute">The attribute that caused the current exception.</param>
        /// <param name="value">The value of the object that caused the attribute to trigger validation error.</param>
        public ValidationException(string errorMessage, System.ComponentModel.DataAnnotations.ValidationAttribute validatingAttribute, object value) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" />
        /// class using a specified error message and a collection of inner exception instances.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The collection of validation exceptions.</param>
        public ValidationException(string message, System.Exception innerException) { }
        /// <summary>
        /// Gets the instance of the <see cref="DataAnnotations.ValidationAttribute" />
        /// class that triggered this exception.
        /// </summary>
        /// <returns>
        /// An instance of the validation attribute type that triggered this exception.
        /// </returns>
        public System.ComponentModel.DataAnnotations.ValidationAttribute ValidationAttribute { get { return default(System.ComponentModel.DataAnnotations.ValidationAttribute); } }
        /// <summary>
        /// Gets the <see cref="ValidationResult" />
        /// instance that describes the validation error.
        /// </summary>
        /// <returns>
        /// The <see cref="ValidationResult" />
        /// instance that describes the validation error.
        /// </returns>
        public System.ComponentModel.DataAnnotations.ValidationResult ValidationResult { get { return default(System.ComponentModel.DataAnnotations.ValidationResult); } }
        /// <summary>
        /// Gets the value of the object that causes the
        /// <see cref="DataAnnotations.ValidationAttribute" /> class to trigger this exception.
        /// </summary>
        /// <returns>
        /// The value of the object that caused the
        /// <see cref="DataAnnotations.ValidationAttribute" /> class to trigger the validation error.
        /// </returns>
        public object Value { get { return default(object); } }
    }
    /// <summary>
    /// Represents a container for the results of a validation request.
    /// </summary>
    public partial class ValidationResult
    {
        /// <summary>
        /// Represents the success of the validation (true if validation was successful; otherwise, false).
        /// </summary>
        public static readonly System.ComponentModel.DataAnnotations.ValidationResult Success;
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" />
        /// class by using a <see cref="ValidationResult" />
        /// object.
        /// </summary>
        /// <param name="validationResult">The validation result object.</param>
        protected ValidationResult(System.ComponentModel.DataAnnotations.ValidationResult validationResult) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" />
        /// class by using an error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public ValidationResult(string errorMessage) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult" />
        /// class by using an error message and a list of members that have validation errors.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="memberNames">The list of member names that have validation errors.</param>
        public ValidationResult(string errorMessage, System.Collections.Generic.IEnumerable<string> memberNames) { }
        /// <summary>
        /// Gets the error message for the validation.
        /// </summary>
        /// <returns>
        /// The error message for the validation.
        /// </returns>
        public string ErrorMessage { get { return default(string); } set { } }
        /// <summary>
        /// Gets the collection of member names that indicate which fields have validation errors.
        /// </summary>
        /// <returns>
        /// The collection of member names that indicate which fields have validation errors.
        /// </returns>
        public System.Collections.Generic.IEnumerable<string> MemberNames { get { return default(System.Collections.Generic.IEnumerable<string>); } }
        /// <summary>
        /// Returns a string representation of the current validation result.
        /// </summary>
        /// <returns>
        /// The current validation result.
        /// </returns>
        public override string ToString() { return default(string); }
    }
    /// <summary>
    /// Defines a helper class that can be used to validate objects, properties, and methods when
    /// it is included in their associated <see cref="ValidationAttribute" />
    /// attributes.
    /// </summary>
    public static partial class Validator
    {
        /// <summary>
        /// Determines whether the specified object is valid using the validation context and validation
        /// results collection.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <returns>
        /// true if the object validates; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance" /> is null.</exception>
        public static bool TryValidateObject(object instance, System.ComponentModel.DataAnnotations.ValidationContext validationContext, System.Collections.Generic.ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults) { return default(bool); }
        /// <summary>
        /// Determines whether the specified object is valid using the validation context, validation results
        /// collection, and a value that specifies whether to validate all properties.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <param name="validateAllProperties">
        /// true to validate all properties; if false, only required attributes are validated..
        /// </param>
        /// <returns>
        /// true if the object validates; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance" /> is null.</exception>
        public static bool TryValidateObject(object instance, System.ComponentModel.DataAnnotations.ValidationContext validationContext, System.Collections.Generic.ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults, bool validateAllProperties) { return default(bool); }
        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <returns>
        /// true if the property validates; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value" /> cannot be assigned to the property.-or-<paramref name="value " />is
        /// null.
        /// </exception>
        public static bool TryValidateProperty(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext, System.Collections.Generic.ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults) { return default(bool); }
        /// <summary>
        /// Returns a value that indicates whether the specified value is valid with the specified attributes.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold failed validations.</param>
        /// <param name="validationAttributes">The validation attributes.</param>
        /// <returns>
        /// true if the object validates; otherwise, false.
        /// </returns>
        public static bool TryValidateValue(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext, System.Collections.Generic.ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults, System.Collections.Generic.IEnumerable<System.ComponentModel.DataAnnotations.ValidationAttribute> validationAttributes) { return default(bool); }
        /// <summary>
        /// Determines whether the specified object is valid using the validation context.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <exception cref="ValidationException">The object is not valid.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="instance" /> is null.</exception>
        public static void ValidateObject(object instance, System.ComponentModel.DataAnnotations.ValidationContext validationContext) { }
        /// <summary>
        /// Determines whether the specified object is valid using the validation context, and a value
        /// that specifies whether to validate all properties.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validateAllProperties">true to validate all properties; otherwise, false.</param>
        /// <exception cref="ValidationException">
        /// <paramref name="instance" /> is not valid.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="instance" /> is null.</exception>
        public static void ValidateObject(object instance, System.ComponentModel.DataAnnotations.ValidationContext validationContext, bool validateAllProperties) { }
        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value" /> cannot be assigned to the property.
        /// </exception>
        /// <exception cref="ValidationException">
        /// The <paramref name="value" /> parameter is not valid.
        /// </exception>
        public static void ValidateProperty(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext) { }
        /// <summary>
        /// Validates the specified attributes.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationAttributes">The validation attributes.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="validationContext" /> parameter is null.
        /// </exception>
        /// <exception cref="ValidationException">
        /// The <paramref name="value" /> parameter does not validate with the <paramref name="validationAttributes" />
        /// parameter.
        /// </exception>
        public static void ValidateValue(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext, System.Collections.Generic.IEnumerable<System.ComponentModel.DataAnnotations.ValidationAttribute> validationAttributes) { }
    }
}
namespace System.ComponentModel.DataAnnotations.Schema
{
    /// <summary>
    /// Represents the database column that a property is mapped to.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public partial class ColumnAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute" />
        /// class.
        /// </summary>
        public ColumnAttribute() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute" />
        /// class.
        /// </summary>
        /// <param name="name">The name of the column the property is mapped to.</param>
        public ColumnAttribute(string name) { }
        /// <summary>
        /// Gets the name of the column the property is mapped to.
        /// </summary>
        /// <returns>
        /// The name of the column the property is mapped to.
        /// </returns>
        public string Name { get { return default(string); } }
        /// <summary>
        /// Gets or sets the zero-based order of the column the property is mapped to.
        /// </summary>
        /// <returns>
        /// The order of the column.
        /// </returns>
        public int Order { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the database provider specific data type of the column the property is mapped
        /// to.
        /// </summary>
        /// <returns>
        /// The database provider specific data type of the column the property is mapped to.
        /// </returns>
        public string TypeName { get { return default(string); } set { } }
    }
    /// <summary>
    /// Denotes that the class is a complex type. Complex types are non-scalar properties of entity
    /// types that enable scalar properties to be organized within entities. Complex types do not have keys
    /// and cannot be managed by the Entity Framework apart from the parent object.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4), AllowMultiple = false)]
    public partial class ComplexTypeAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ComplexTypeAttribute" /> class.
        /// </summary>
        public ComplexTypeAttribute() { }
    }
    /// <summary>
    /// Specifies how the database generates values for a property.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public partial class DatabaseGeneratedAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DatabaseGeneratedAttribute" /> class.
        /// </summary>
        /// <param name="databaseGeneratedOption">The database generated option.</param>
        public DatabaseGeneratedAttribute(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption databaseGeneratedOption) { }
        /// <summary>
        /// Gets or sets the pattern used to generate values for the property in the database.
        /// </summary>
        /// <returns>
        /// The database generated option.
        /// </returns>
        public System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption DatabaseGeneratedOption { get { return default(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption); } }
    }
    /// <summary>
    /// Represents the pattern used to generate values for a property in the database.
    /// </summary>
    public enum DatabaseGeneratedOption
    {
        /// <summary>
        /// The database generates a value when a row is inserted or updated.
        /// </summary>
        Computed = 2,
        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        Identity = 1,
        /// <summary>
        /// The database does not generate values.
        /// </summary>
        None = 0,
    }
    /// <summary>
    /// Denotes a property used as a foreign key in a relationship. The annotation may be placed on
    /// the foreign key property and specify the associated navigation property name, or placed on a navigation
    /// property and specify the associated foreign key name.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public partial class ForeignKeyAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ForeignKeyAttribute" /> class.
        /// </summary>
        /// <param name="name">
        /// If you add the ForeigKey attribute to a foreign key property, you should specify the name of
        /// the associated navigation property. If you add the ForeigKey attribute to a navigation property, you
        /// should specify the name of the associated foreign key(s). If a navigation property has multiple foreign
        /// keys, use comma to separate the list of foreign key names. For more information, see Code First
        /// Data Annotations.
        /// </param>
        public ForeignKeyAttribute(string name) { }
        /// <summary>
        /// If you add the ForeigKey attribute to a foreign key property, you should specify the name of
        /// the associated navigation property. If you add the ForeigKey attribute to a navigation property, you
        /// should specify the name of the associated foreign key(s). If a navigation property has multiple foreign
        /// keys, use comma to separate the list of foreign key names. For more information, see Code First
        /// Data Annotations.
        /// </summary>
        /// <returns>
        /// The name of the associated navigation property or the associated foreign key property.
        /// </returns>
        public string Name { get { return default(string); } }
    }
    /// <summary>
    /// Specifies the inverse of a navigation property that represents the other end of the same relationship.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public partial class InversePropertyAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="InversePropertyAttribute" /> class using the specified property.
        /// </summary>
        /// <param name="property">The navigation property representing the other end of the same relationship.</param>
        public InversePropertyAttribute(string property) { }
        /// <summary>
        /// Gets the navigation property representing the other end of the same relationship.
        /// </summary>
        /// <returns>
        /// The property of the attribute.
        /// </returns>
        public string Property { get { return default(string); } }
    }
    /// <summary>
    /// Denotes that a property or class should be excluded from database mapping.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(388), AllowMultiple = false)]
    public partial class NotMappedAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NotMappedAttribute" /> class.
        /// </summary>
        public NotMappedAttribute() { }
    }
    /// <summary>
    /// Specifies the database table that a class is mapped to.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4), AllowMultiple = false)]
    public partial class TableAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAttribute" />
        /// class using the specified name of the table.
        /// </summary>
        /// <param name="name">The name of the table the class is mapped to.</param>
        public TableAttribute(string name) { }
        /// <summary>
        /// Gets the name of the table the class is mapped to.
        /// </summary>
        /// <returns>
        /// The name of the table the class is mapped to.
        /// </returns>
        public string Name { get { return default(string); } }
        /// <summary>
        /// Gets or sets the schema of the table the class is mapped to.
        /// </summary>
        /// <returns>
        /// The schema of the table the class is mapped to.
        /// </returns>
        public string Schema { get { return default(string); } set { } }
    }
}
